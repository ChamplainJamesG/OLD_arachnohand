using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This goes on a skeleton. A skeleton will have multiple animation controllers.
/// </summary>
public class BlendController : MonoBehaviour
{
    public enum BLEND_TYPE
    {
        ADD = 0,
        LERP,
        SCALE,
        AVG
    }

    public Transform UpperDelimiter;
    public Transform LowerDelimiter;

    /*ASK DAN ABOUT BODY BLENDING.
     * MASKING
     */
    public class BlendCtrlOp
    {
        public TestBlendBase.BlendTransform ctrl1;
        public TestBlendBase.BlendTransform ctrl2;
        public BLEND_TYPE blendType = BLEND_TYPE.ADD;

        TestBlendBase.BlendTransform Blend(float param, float param1)
        {
            switch(blendType)
            {
                case BLEND_TYPE.ADD:
                    return new TestBlendAdd().Blend(ctrl1, ctrl2);
                case BLEND_TYPE.AVG:
                    return new TestBlendAvg().Blend(ctrl1, ctrl2, param, param1);
                case BLEND_TYPE.LERP:
                    return new TestBlendLerp().Blend(ctrl1, ctrl2, param);
                case BLEND_TYPE.SCALE:
                    return new TestBlendScale().Blend(ctrl1, ctrl2, param);

                default:
                    return new TestBlendBase.BlendTransform();
            }
        }

        public List<TestBlendBase.BlendTransform> Blend(Transform param0, Transform param1, BlendController ctrl, Transform delimiter)
        {
            switch (blendType)
            {
                case BLEND_TYPE.ADD:
                    ctrl.GetComponent<TestBlendAdd>().pose0 = param0;
                    ctrl.GetComponent<TestBlendAdd>().pose1 = param1;
                    ctrl.GetComponent<TestBlendAdd>().boneLength = 2.0f;
                    ctrl.GetComponent<TestBlendAdd>().BlendAllChildren2(param0, param1, delimiter);
                    break;
                case BLEND_TYPE.AVG:
                    new TestBlendAvg().BlendAllChildren(param0, param1);
                    break;
                case BLEND_TYPE.LERP:
                    ctrl.GetComponent<TestBlendLerp>().pose0 = param0;
                    ctrl.GetComponent<TestBlendLerp>().pose1 = param1;
                    ctrl.GetComponent<TestBlendLerp>().boneLength = 2.0f;
                    ctrl.GetComponent<TestBlendLerp>().BlendAllChildren2(param0, param1, delimiter);
                    break;
                case BLEND_TYPE.SCALE:
                    new TestBlendScale().Blend(param0, param1);
                    break;

                default:
                    break;
            }

            return new List<TestBlendBase.BlendTransform>();
        }

        public List<AnimationController.ObjAndTransform> Blend(List<AnimationController.ObjAndTransform> param0, 
            List<AnimationController.ObjAndTransform> param1, BlendController ctrl)
        {
            var list = new List<AnimationController.ObjAndTransform>();
            switch (blendType)
            {
                case BLEND_TYPE.ADD:
                    ctrl.GetComponent<TestBlendAdd>().boneLength = 2.0f;
                    list = ctrl.GetComponent<TestBlendAdd>().BlendAllChildren3TheThirdOne(param0, param1);
                    break;
                case BLEND_TYPE.AVG:
                    new TestBlendAvg().BlendAllChildren3TheThirdOne(param0, param1);
                    break;
                case BLEND_TYPE.LERP:
                    ctrl.GetComponent<TestBlendLerp>().boneLength = 2.0f;
                    list = ctrl.GetComponent<TestBlendLerp>().BlendAllChildren3TheThirdOne(param0, param1);
                    break;
                case BLEND_TYPE.SCALE:
                    new TestBlendScale().BlendAllChildren3TheThirdOne(param0, param1);
                    break;

                default:
                    break;
            }
            return list;
        }

        public List<AnimationController.ObjAndTransform> Blend(List<AnimationController.ObjAndTransform> param0,
                    List<AnimationController.ObjAndTransform> param1, BlendController ctrl, float f)
        {
            var list = new List<AnimationController.ObjAndTransform>();
            ctrl.GetComponent<TestBlendLerp>().boneLength = 1.0f;
            ctrl.GetComponent<TestBlendLerp>().param = f;
            list = ctrl.GetComponent<TestBlendLerp>().BlendAllChildren4TheShrekoning(param0, param1);
            return list;
        }
    }


    public AnimationController UpperWalkCtrl;
    public AnimationController LowerWalkCtrl;
    public AnimationController WobbleCtrl;
    public AnimationController IKCtrl;

    public IKSolver Solver1, Solver2;

    // Start is called before the first frame update
    void Start()
    {
        var loader = GetComponent<KeyframeLoader>();
        UpperWalkCtrl.LoadKeyFrames(loader.LoadKeyframe(KeyframeLoader.KEY_MASK.UPPER, "walking_upper"));
        LowerWalkCtrl.LoadKeyFrames(loader.LoadKeyframe(KeyframeLoader.KEY_MASK.LOWER, "walking_lower"));
        WobbleCtrl.LoadKeyFrames(loader.LoadKeyframe(KeyframeLoader.KEY_MASK.UPPER, "bodywiggle_upper"));

        UpperDelimiter = KeyframeLoader.FindDeepChild("Spine1", transform);
        LowerDelimiter = KeyframeLoader.FindDeepChild("Pelvis", transform);
    }

    // Update is called once per frame
    void Update()
    {
        BlendCtrlOp o1 = new BlendCtrlOp();
        o1.blendType = BLEND_TYPE.ADD;
        // Get op 1 for walkings.

        BlendCtrlOp o2 = new BlendCtrlOp();
        o2.blendType = BLEND_TYPE.ADD;
        // Get op2, blending in what we got from 1 w/ the wobble.

        BlendCtrlOp o3 = new BlendCtrlOp();
        // Get op3, blending in what we got from 2 w/ the IK, param of distance to target.

        /* TODO:
         * LOAD KEYFRAMES
         * MAKE BLENDITALL RETURN A LIST OF BLENDTRANSFORMS
         * ???
         * PROFIT
         */
        var v = UpperWalkCtrl.DoClip(Time.deltaTime);
        var v1 = LowerWalkCtrl.DoClip(Time.deltaTime);
        var v2 = WobbleCtrl.DoClip(Time.deltaTime);
        //var v3 = IKCtrl.DoClip(Time.deltaTime);

        //var b1 = o2.Blend(v, v1, this);

        var final1 = o1.Blend(v, v2, this);
        var IkFinalList = Solver1.GetArrBetter();

        final1 = o3.Blend(final1, IkFinalList, this, Solver1.GetDist());

        //var extraSpicyFinal = DoForwardKinematics(IkFinalList);

        //for(int i = 0; i < IkFinalList.Count; ++i)
        //{
        //    //extraSpicyFinal[i].SetPositionToTForm();
        //    IkFinalList[i].SetPositionToTForm();
        //}

        foreach (var f in final1)
            f.SetPositionToTForm();

        //o1.Blend(v[0].objRef.transform, v1[0].objRef.transform, this);
        //o2.Blend(v[0].objRef.transform, v2[0].objRef.transform);
        //o3.Blend(v[0].objRef.transform, v3[0].objRef.transform);
    }

    private List<AnimationController.ObjAndTransform> DoForwardKinematics(List<AnimationController.ObjAndTransform> brehs)
    {
        List<AnimationController.ObjAndTransform> newTransforms = new List<AnimationController.ObjAndTransform>();
        for (int i = 0; i < brehs.Count; ++i)
        {
            //var trs = Matrix4x4.TRS(brehs[i].tForm.translation, brehs[i].tForm.rotation, brehs[i].tForm.scale);
            //var parentTrs = Matrix4x4.TRS(brehs[i].objRef.transform.parent.localPosition, brehs[i].objRef.transform.parent.localRotation,
            //    brehs[i].objRef.transform.parent.localScale);
            //var newTRS = parentTrs.inverse * trs;
            //// https://answers.unity.com/questions/402280/how-to-decompose-a-trs-matrix.html
            //brehs[i].tForm.translation = newTRS.GetColumn(3);
            //brehs[i].tForm.rotation = Quaternion.LookRotation(newTRS.GetColumn(2), newTRS.GetColumn(1));
            //brehs[i].tForm.scale = Vector3.one;
            brehs[i].tForm.translation = brehs[i].objRef.transform.parent.InverseTransformPoint(brehs[i].tForm.translation);
            newTransforms.Add(brehs[i]);
        }

        return newTransforms;
    }
}


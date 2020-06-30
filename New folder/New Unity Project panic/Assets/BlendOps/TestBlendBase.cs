using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendBase : MonoBehaviour
{
    public struct BlendTransform
    {
        public Vector3 translation;
        public Vector3 scale;
        public Quaternion rotation;
        public Vector3 rotationEuler;
    }

    protected Transform pose_result;
    public bool usingQuaternionRotation = true;
    public float boneLength;

    protected static Transform pose_identity;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        pose_result = gameObject.transform;
        pose_identity = GameObject.Find("pose_identity").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 multVectors(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
    }

    public Vector3 divVectors(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
    }

    public void BlendAllChildren(Transform pose1, Transform pose2)
    {
        pose_result = gameObject.transform.GetChild(0);

        DoItALot(pose_result, pose1.GetChild(0), pose2.GetChild(0));
    }

    public void BlendAllChildren2(Transform pose1, Transform pose2, Transform transform)
    {
        pose_result = transform;

        DoItALot(pose_result.GetChild(0), pose1.GetChild(0), pose2.GetChild(0));
    }

    public List<AnimationController.ObjAndTransform> BlendAllChildren3TheThirdOne(List<AnimationController.ObjAndTransform> t0,
        List<AnimationController.ObjAndTransform> t1)
    {
        List<AnimationController.ObjAndTransform> objAnds = new List<AnimationController.ObjAndTransform>();
        for(int i = 0; i < t0.Count; ++i)
        {
            AnimationController.ObjAndTransform obj = new AnimationController.ObjAndTransform();
            obj.objRef = t0[i].objRef;
            obj.tForm = Blend(t0[i].tForm, t1[i].tForm);
            objAnds.Add(obj);
        }

        return objAnds;
    }

    public List<AnimationController.ObjAndTransform> BlendAllChildren4TheShrekoning(List<AnimationController.ObjAndTransform> t0,
        List<AnimationController.ObjAndTransform> t1)
    {
        List<AnimationController.ObjAndTransform> objAnds = new List<AnimationController.ObjAndTransform>();

        int count = Mathf.Min(t0.Count, t1.Count);

        List<AnimationController.ObjAndTransform> otherList = new List<AnimationController.ObjAndTransform>();
        List<AnimationController.ObjAndTransform> otherBIGGERANDBETTERList = new List<AnimationController.ObjAndTransform>();
        if (count == t0.Count)
        {
            otherList = t0;
            otherBIGGERANDBETTERList = t1;
        }
        else
        {
            otherList = t1;
            otherBIGGERANDBETTERList = t0;
        }



        for (int i = 0; i < count; ++i)
        {
            AnimationController.ObjAndTransform oat = new AnimationController.ObjAndTransform();
            var obj = otherList[i];
            int indexer = otherBIGGERANDBETTERList.FindIndex(x => x.objRef == obj.objRef);
            oat.objRef = obj.objRef;
            oat.tForm = Blend(otherList[i].tForm, otherBIGGERANDBETTERList[indexer].tForm);
            objAnds.Add(oat);
        }

        return objAnds;
    }
    /*
     * Pseudo code for BlendAllChildren4
     * Take in the transform of the current joint node and the keyframe data for the two keyframes we are tring to blend
     * create a new ObjAndTransform object and have it equal to the blended keyframe data
     * add that fella to a list
     * do this recursively for all of the children of the transform that was passed it
     * */

    void DoItALot(Transform pose_result_child, Transform p1, Transform p2)
    {
        pose_result = pose_result_child;
        WillItBlend(p1, p2);

        for (int i = 0; i < pose_result_child.childCount; i++)
        {
            DoItALot(pose_result_child.GetChild(i), p1.GetChild(i), p2.GetChild(i));
        }
    }

    void WillItBlend(Transform p1, Transform p2)
    {
        BlendTransform newPose = Blend(p1, p2);
        pose_result.localScale = newPose.scale;
        pose_result.localPosition = newPose.translation;
        if (usingQuaternionRotation)
            pose_result.localRotation = newPose.rotation;
        else
            pose_result.localEulerAngles = newPose.rotationEuler;
    }

    BlendTransform WillItBlend2(BlendTransform t1, BlendTransform t2)
    {
        var newAuto = Blend(t1, t2);
        return newAuto;
    }

    public virtual BlendTransform Blend(Transform p1, Transform p2)
    {
        return new BlendTransform();
    }

    public virtual BlendTransform Blend(BlendTransform d1, BlendTransform d2)
    {
        return new BlendTransform();
    }

    public virtual BlendTransform Blend(BlendTransform d1, BlendTransform d2, float param)
    {
        return new BlendTransform();
    }
}

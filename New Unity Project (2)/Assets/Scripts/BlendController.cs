using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class BlendController : MonoBehaviour
{
    public enum BLEND_OP
    {
        ADD = 0,
        LERP,
    }
    // Find all the animation controllers to blend together. 
    public AnimationController controller1;
    public AnimationController controller2;
    
    public class BlendBoys
    {
        private BLEND_OP blendOp;
        public BlendBoys(BLEND_OP o)
        {
            blendOp = o;
        }

        public void BlendBones(List<BlendTransform> transforms)
        {
            switch(blendOp)
            {
                case BLEND_OP.ADD:
                    // add
                    break;
                case BLEND_OP.LERP:
                    // Lerp
                    break;
            }
        }

        public List<BlendTransform> BlendBonesAdd(List<BlendTransform> t1, List<BlendTransform> t2)
        {
            AddBlendControlOp adder = new AddBlendControlOp();
            return adder.BlendAllChildren(t1, t2);
        }

        public List<BlendTransform> BlendBonesLerp(List<BlendTransform> t1, List<BlendTransform> t2, float t)
        {
            LerpBlendControlOp adder = new LerpBlendControlOp();
            adder.t = t;
            return adder.BlendAllChildren(t1, t2);
        }
    }

    private void Start()
    {
        controller1.LoadKeyFrames("clip1");
        controller2.LoadKeyFrames("clip2");
    }

    // Update is called once per frame
    void Update()
    {
        var formsThisFrame = controller1.ControlUpdate(Time.deltaTime);
        var secondFormsThisFrame = controller2.ControlUpdate(Time.deltaTime);

        var v = InputManager.ActiveDevice.LeftStick.Vector.magnitude;

        var blender1 = new BlendBoys(BLEND_OP.LERP);
        var absoluteFinalFrames = blender1.BlendBonesLerp(formsThisFrame, secondFormsThisFrame, v);

        SetTransformsAtEndBlend(absoluteFinalFrames);


    }

    private void SetTransformsAtEndBlend(List<BlendTransform> finalPos)
    {
        foreach(BlendTransform f in finalPos)
        {
            int ind = f.boneIndex;

            JointManager.joints[ind].SetJoint(f);
        }
    }
}

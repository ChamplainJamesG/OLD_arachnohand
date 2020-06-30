using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint
{
    public AnimationController.Keyframe[] keyframes;
    public AnimationController.Keyframe basePosition;
    public Joint parentJoint;
    public string jointName;

    Matrix4x4 localtrs;
    Matrix4x4 worldTrs;

    public void Init(int keyframeAmount, string name)
    {
        keyframes = new AnimationController.Keyframe[keyframeAmount];
        basePosition = new AnimationController.Keyframe();
        jointName = name;
    }

    public void SetTRS(Matrix4x4 m)
    {
        localtrs = m;
    }

    public Matrix4x4 getTRS()
    {
        return localtrs;
    }

    public void setWTRS(Matrix4x4 m)
    {
        worldTrs = m;
    }

    public Matrix4x4 getWTRS()
    {
        return worldTrs;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendAdd : TestBlendBase
{
    // We have to figure out what add means, because it might not just be a + operation.
    // Add as a blend operation means concatenate all the parts of the transform.
    public Transform pose0, pose1;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //BlendAllChildren(pose0, pose1);
    }

    // Update is called once per frame
    void Update()
    {
        // Translation concatenation is literally just addition
        // Scale concatenation is a component-wise multiplication
        // Rotation concatenation is an add if not using Quaternion. Quat concatenation if we are using quats (should be in the slides)
    }

    public override BlendTransform Blend(Transform p1, Transform p2)
    {
        BlendTransform newPose = new BlendTransform();
        newPose.translation = p1.localPosition + p2.localPosition;
        newPose.translation = newPose.translation.normalized * boneLength;
        newPose.scale = multVectors(p1.localScale, p2.localScale);

        if (usingQuaternionRotation)
            newPose.rotation = p1.localRotation * p2.localRotation;
        else
            newPose.rotationEuler = p1.localEulerAngles + p2.localEulerAngles;

        return newPose;
    }

    public override BlendTransform Blend(BlendTransform d1, BlendTransform d2)
    {
        BlendTransform newPose = new BlendTransform();
        newPose.translation = d1.translation + d2.translation;
        newPose.translation = newPose.translation.normalized * boneLength;
        newPose.scale = multVectors(d1.scale, d2.scale);

        if (usingQuaternionRotation)
            newPose.rotation = d1.rotation * d2.rotation;
        else
            newPose.rotationEuler = d1.rotationEuler + d2.rotationEuler;

        return newPose;

    }
}

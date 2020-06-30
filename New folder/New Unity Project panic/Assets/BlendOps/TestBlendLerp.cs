using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendLerp : TestBlendBase
{
    public Transform pose0, pose1;

    [Range(0.0f, 1.0f)]
    public float param;

    // Update is called once per frame
    void Update()
    {

        //BlendAllChildren(pose0, pose1);
    }

    public override BlendTransform Blend(Transform p1, Transform p2)
    {
        BlendTransform newPose = new BlendTransform();

        // translation: just a regular old LERP
        newPose.translation = Vector3.Lerp(p1.localPosition, p2.localPosition, param);

        // scale: regular old LERP
        newPose.scale = Vector3.Lerp(p1.localScale, p2.localScale, param);

        // rotation: quat SLREP (or, NLERP // Optimization) or LERP
        if (usingQuaternionRotation)
            newPose.rotation = Quaternion.Slerp(p1.localRotation, p2.localRotation, param);
        else
            newPose.rotationEuler = Vector3.Lerp(p1.localEulerAngles, p2.localEulerAngles, param);

        return newPose;
    }

    public override BlendTransform Blend(BlendTransform d1, BlendTransform d2)
    {
        BlendTransform newPose = new BlendTransform();

        // translation: just a regular old LERP
        newPose.translation = Vector3.Lerp(d1.translation, d2.translation, param);

        // scale: regular old LERP
        newPose.scale = Vector3.Lerp(d1.scale, d2.scale, param);

        // rotation: quat SLREP (or, NLERP // Optimization) or LERP
        if (usingQuaternionRotation)
            newPose.rotation = Quaternion.Slerp(d1.rotation, d2.rotation, param);
        else
            newPose.rotationEuler = Vector3.Lerp(d1.rotationEuler, d2.rotationEuler, param);

        return newPose;
    }

    public override BlendTransform Blend(BlendTransform d1, BlendTransform d2, float param)
    {
        BlendTransform newPose = new BlendTransform();

        // translation: just a regular old LERP
        newPose.translation = Vector3.Lerp(d1.translation, d2.translation, param);

        // scale: regular old LERP
        newPose.scale = Vector3.Lerp(d1.scale, d2.scale, param);

        // rotation: quat SLREP (or, NLERP // Optimization) or LERP
        if (usingQuaternionRotation)
            newPose.rotation = Quaternion.Slerp(d1.rotation, d2.rotation, param);
        else
            newPose.rotationEuler = Vector3.Lerp(d1.rotationEuler, d2.rotationEuler, param);

        return newPose;
    }
}

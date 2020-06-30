using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendAvg : TestBlendBase
{

    public Transform pose0, pose1;

    [Range(0.0f, 1.0f)]
    public float param0;

    [Range(0.0f, 1.0f)]
    public float param1;

    // Update is called once per frame
    void Update()
    {
        /* Do scale w/ 0 and 1
         * Scale is identity w/ 0 and 1
         * Then concatenate the 2 together after
         */
        //BlendScale(pose0, param0);
        //BlendScale(pose1, param1);

        BlendAllChildren(pose0, pose1);
    }

    public override BlendTransform Blend(Transform p1, Transform p2)
    {
        BlendTransform t0, t1;
        t0 = BlendScale(p1, param0);
        t1 = BlendScale(p2, param1);
        return ConcatTransforms(t0, t1);
    }

    public BlendTransform Blend(BlendTransform d1, BlendTransform d2, float param1, float param2)
    {
        BlendTransform t0, t1;
        t0 = BlendScale(d1, param1);
        t1 = BlendScale(d2, param2);
        return ConcatTransforms(t0, t1);
    }

    private BlendTransform BlendScale(Transform concatPose, float param)
    {
        // translation: just a regular old LERP
        BlendTransform newDon = new BlendTransform();
        newDon.translation = Vector3.Lerp(pose_identity.localPosition, concatPose.localPosition, param);
        newDon.scale = Vector3.Lerp(pose_identity.localScale, concatPose.localScale, param);

        // rotation: quat SLREP (or, NLERP // Optimization) or LERP
        if (usingQuaternionRotation)
            newDon.rotation = Quaternion.Slerp(pose_identity.localRotation, concatPose.localRotation, param);
        else
            newDon.rotationEuler = Vector3.Lerp(pose_identity.localEulerAngles, concatPose.localEulerAngles, param) ;

        return newDon;
    }

    private BlendTransform BlendScale(BlendTransform concatLaackman, float param)
    {
        // translation: just a regular old LERP
        BlendTransform newDon = new BlendTransform();
        newDon.translation = Vector3.Lerp(pose_identity.localPosition, concatLaackman.translation, param);
        newDon.scale = Vector3.Lerp(pose_identity.localScale, concatLaackman.scale, param);

        // rotation: quat SLREP (or, NLERP // Optimization) or LERP
        if (usingQuaternionRotation)
            newDon.rotation = Quaternion.Slerp(pose_identity.localRotation, concatLaackman.rotation, param);
        else
            newDon.rotationEuler = Vector3.Lerp(pose_identity.localEulerAngles, concatLaackman.rotationEuler, param);

        return newDon;
    }

    //private Vector3 BlendTransform(Transform concatPose, float param)
    //{
    //}

    //private Vector3 BlendScaleButNotReally(Transform concatPose, float param)
    //{
    //    // scale: regular old LERP
    //}

    //private (Vector3, Quaternion) BlendRot(Transform concatPose, float param)
    //{

    //}

    private BlendTransform ConcatTransforms(BlendTransform t0, BlendTransform t1)
    {
        BlendTransform newPose = new BlendTransform();
        newPose.translation = t0.translation + t1.translation;
        newPose.translation = newPose.translation.normalized * boneLength;

        newPose.scale = multVectors(t0.scale, t1.scale);
        if (usingQuaternionRotation)
            newPose.rotation = t0.rotation * t1.rotation;
        else
            newPose.rotationEuler = t0.rotationEuler + t1.rotationEuler;

        return newPose;
    }
}

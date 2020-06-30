using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTransform
{
    public Vector4 position, scale;
    public Quaternion rotation;
    public Matrix4x4 localToWorldTransform;
    public Matrix4x4 worldToLocalTransform;

    private static readonly Vector4 vIdentity = Vector4.zero;
    private static readonly Quaternion qIdentity = Quaternion.identity;
    private static readonly Matrix4x4 mIdentity = Matrix4x4.identity;

    public int boneIndex;

    public Vector4 deltaPThisFrame;
    public Quaternion deltaRotThisFrame;
    public Vector4 deltaScaleThisFrame;

    public BlendTransform()
    {
        SetIdentity();
    }

    public BlendTransform(Vector4 pos, Vector4 scal, Quaternion newQ)
    {
        position = pos;
        scale = scal;
        rotation = newQ;
        localToWorldTransform = mIdentity;
        worldToLocalTransform = mIdentity;
    }

    private void SetIdentity()
    {
        position = vIdentity;
        scale = vIdentity;
        rotation = qIdentity;
        localToWorldTransform = mIdentity;
        worldToLocalTransform = mIdentity;
    }
}

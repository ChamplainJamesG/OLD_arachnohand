using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBlendControlOp : BlendControlOp
{
    public float t;

    public override BlendTransform Blend(BlendTransform t1, BlendTransform t2)
    {
        BlendTransform newT = new BlendTransform();

        newT.position = Vector3.Lerp(t1.position, t2.position, t);
        newT.scale = Vector3.Lerp(t1.scale, t2.scale, t);
        newT.rotation = Quaternion.Slerp(t1.rotation, t2.rotation, t);
        newT.deltaPThisFrame = Vector4.Lerp(t1.deltaPThisFrame, t2.deltaPThisFrame, t);
        newT.deltaRotThisFrame = Quaternion.Slerp(t1.deltaRotThisFrame, t2.deltaRotThisFrame, t);
        newT.deltaScaleThisFrame = Vector4.Lerp(t1.deltaScaleThisFrame, t2.deltaScaleThisFrame, t);

        return newT;
    }
}

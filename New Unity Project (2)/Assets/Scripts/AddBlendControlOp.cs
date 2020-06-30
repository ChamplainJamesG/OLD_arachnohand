using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddBlendControlOp : BlendControlOp
{

    public override BlendTransform Blend(BlendTransform t1, BlendTransform t2)
    {
        BlendTransform newT = new BlendTransform();
        newT.position = t1.position + t2.position;
        // Normalize that shit here?!
        newT.scale = multVectors(t1.scale, t2.scale);
        newT.rotation = t1.rotation * t2.rotation;
        return newT;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendControlOp 
{
    public Vector3 multVectors(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
    }

    public Vector3 divVectors(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
    }

    public List<BlendTransform> BlendAllChildren(List<BlendTransform> origTs, List<BlendTransform> otherTs)
    {
        List<BlendTransform> ts = new List<BlendTransform>();
        int szCount = Mathf.Min(origTs.Count, otherTs.Count);

        List<BlendTransform> smallerList = new List<BlendTransform>();
        List<BlendTransform> largerList = new List<BlendTransform>();

        if(szCount == origTs.Count)
        {
            smallerList = origTs;
            largerList = otherTs;
        }
        else
        {
            smallerList = otherTs;
            largerList = origTs;
        }

        for (int i = 0; i < smallerList.Count; ++i)
        {
            // TODO: FIX THIS:
            BlendTransform t1 = smallerList[i];
            BlendTransform t2 = largerList[i];

            BlendTransform finalBlend = Blend(t1, t2);
            ts.Add(finalBlend);
        }

        return ts;
    }

    public virtual BlendTransform Blend(BlendTransform t1, BlendTransform t2)
    {
        return new BlendTransform();
    }
}

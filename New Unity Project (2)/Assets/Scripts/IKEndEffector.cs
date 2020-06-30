using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKPackage
{
    public float[] boneLength;
    public float totalLength;
    public BlendTransform elbowConstraint;
    public BlendTransform[] blendTransforms;
    public Vector4 target;
}

public class IKEndEffector : MonoBehaviour
{
    public int boneAmount;
    public Transform elbowConstraint;
    public bool doIK;

    public Transform target;
    public Transform wrist;

    IKPackage package;
    Transform[] joints;

    IKSolver ik;

    void Start()
    {
        ik = FindObjectOfType<IKSolver>();
    }

    public void Init()
    {
        package = new IKPackage();

        package.elbowConstraint = new BlendTransform();
        package.elbowConstraint.position = elbowConstraint.position;
        package.blendTransforms = new BlendTransform[boneAmount + 1];
        package.boneLength = new float[boneAmount];
        package.totalLength = 0;

        joints = new Transform[boneAmount + 1];

        Transform current = transform;

        for (int i = boneAmount; i >= 0; i--)
        {
            joints[i] = current;
            package.blendTransforms[i] = new BlendTransform
            {
                position = current.position,
                rotation = current.rotation,
                scale = current.localScale
            };

            if (i != boneAmount)
            {
                package.boneLength[i] = (current.GetChild(0).position - current.position).magnitude;
                package.totalLength += package.boneLength[i];
            }

            current = current.parent;
        }
    }

    void FixedUpdate()
    {
        if (doIK)
        {
            package.blendTransforms = ik.ResolveIK(package);

            for (int i = 0; i < joints.Length; i++)
            {
                //List<Transform> parents = new List<Transform>();

                //Transform current = joints[i].parent;

                //while (current.parent != null)
                //{
                //    parents.Add(current);
                //    current = current.parent;
                //}

                //for (int j = parents.Count - 2; j >= 0; j--)
                //{
                //    package.blendTransforms[i].position = parents[j].InverseTransformVector(package.blendTransforms[i].position);
                //}

                //joints[i].localPosition = joints[i].InverseTransformVector(package.blendTransforms[i].position);
                //joints[i].position = new Vector3(joints[i].position.x + package.blendTransforms[i].position.x, joints[i].position.y + package.blendTransforms[i].position.y, joints[i].position.z + package.blendTransforms[i].position.z);
                joints[i].position = package.blendTransforms[i].position;
                //joints[i].transform.localPosition = new Vector3(0, joints[i].transform.localPosition.y, joints[i].transform.localPosition.z);

                //if (maybe)
                //{
                //    if (joints[i].transform.childCount > 0)
                //        joints[i].transform.LookAt(joints[i].transform.GetChild(0));
                //    else
                //        joints[i].transform.LookAt(target);
                //}
            }
        }
    }

    public void SetTarget(Vector3 newTarget)
    {
        package.target = newTarget;
    }
}

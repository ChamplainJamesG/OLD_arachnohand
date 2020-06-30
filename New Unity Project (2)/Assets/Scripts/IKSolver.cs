using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    public int iterations;

    BlendTransform[] jointPositions;

    public BlendTransform[] ResolveIK(IKPackage package)
    {
        Vector4 originalRootPos = package.blendTransforms[0].position;
        jointPositions = new BlendTransform[package.blendTransforms.Length];

        int i;
        for (i = 0; i < jointPositions.Length; i++)
        {
            jointPositions[i] = new BlendTransform();
            jointPositions[i].position = package.blendTransforms[i].position;
        }

        if ((package.target - jointPositions[0].position).sqrMagnitude >= package.totalLength * package.totalLength)
        {
            Vector4 dir = (package.target - jointPositions[0].position).normalized;

            for (i = 1; i < jointPositions.Length; i++)
                jointPositions[i].position = jointPositions[i - 1].position + dir * package.boneLength[i - 1];
        }
        else
        {
            int j;

            for (i = 0; i < iterations; i++)
            {
                jointPositions[jointPositions.Length - 1].position = package.target;
                for (j = jointPositions.Length - 2; j > 0; j--)
                    jointPositions[j].position = jointPositions[j + 1].position + (jointPositions[j].position - jointPositions[j + 1].position).normalized * package.boneLength[j];

                jointPositions[0].position = originalRootPos;
                for (j = 1; j < jointPositions.Length; j++)
                    jointPositions[j].position = jointPositions[j - 1].position + (jointPositions[j].position - jointPositions[j - 1].position).normalized * package.boneLength[j - 1];

                if ((package.target - jointPositions[jointPositions.Length - 1].position).sqrMagnitude < 0.01f * 0.01f)
                    break;
            }

            for (i = 1; i < jointPositions.Length - 1; i++)
            {
                Plane plane = new Plane(jointPositions[i + 1].position - jointPositions[i - 1].position, jointPositions[i - 1].position);
                Vector4 projectedPole = plane.ClosestPointOnPlane(package.elbowConstraint.position);
                Vector4 projectedBone = plane.ClosestPointOnPlane(jointPositions[i].position);
                float angle = Vector3.SignedAngle(projectedBone - jointPositions[i - 1].position, projectedPole - jointPositions[i - 1].position, plane.normal);
                jointPositions[i].position = (Vector4)(Quaternion.AngleAxis(angle, plane.normal) * (jointPositions[i].position - jointPositions[i - 1].position)) + jointPositions[i - 1].position;
            }
        }

        for(i = 0; i < jointPositions.Length; i++)
        {
           // jointPositions[i].position = jointPositions[i].position - package.blendTransforms[i].position;
        }
        return jointPositions;
    }
}

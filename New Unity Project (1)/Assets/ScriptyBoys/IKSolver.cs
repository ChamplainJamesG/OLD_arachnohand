using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{
    public Transform[] joints;
    public Transform target;
    public Transform elbowThing;
    public int iterations;
    Vector3[] positions;
    Vector3[] startingPositions;
    float[] boneLength;
    float totalBoneLength;

    bool initted;

    private void Start()
    {
       // Init();
    }

    public void Init()
    {
        totalBoneLength = 0;
        boneLength = new float[joints.Length - 1];
        positions = new Vector3[joints.Length];
        startingPositions = new Vector3[joints.Length];
        for(int i = 0; i < joints.Length; i++)
        {
            positions[i] = joints[i].position;
            startingPositions[i] = positions[i];
            if (i < boneLength.Length)
            {
                boneLength[i] = (joints[i + 1].position - joints[i].position).magnitude;
                totalBoneLength += boneLength[i];
            }
        }

        initted = true;
    }

    void LateUpdate()
    {
        if(initted)
        {
            Solve();
        }
    }

    void Solve()
    {
        //If the target is farther than the length of each bone, then just project each joint onto the vector to the target
        if (Vector3.Distance(joints[0].position, target.position) >= totalBoneLength)
        {
            //Get the vector from the root joint to the target
            Vector3 direction = (target.position - joints[0].position).normalized;

            //project each joint onto this vector and make that their new position
            for (int i = 1; i < joints.Length; i++)
            {
                joints[i].position = joints[i - 1].position + direction * boneLength[i - 1];
            }
        }
        else
        {

            for (int k = 0; k < iterations; k++)
            {
                //Set position array equal to the current positions to fuck around with
                for (int i = 0; i < joints.Length; i++)
                {
                    positions[i] = joints[i].position;
                }

                //Set end effector to the target position
                positions[positions.Length - 1] = target.position;

                //Go through the other joints (other than the root) and move it to the new spot based on the length of the bone between the joints
                for (int i = joints.Length - 2; i > 0; i--)
                {
                    positions[i] = joints[i + 1].position + (positions[i] - positions[i + 1]).normalized * boneLength[i];
                }

                //Set root joint back to starting position
                positions[0] = startingPositions[0];

                //Do the thing that we did above, but backwards (or should I say FORWARDS [cause we did it backwards before and now we are doing it forwards {hahaha}])
                for (int i = 1; i < joints.Length; i++)
                {
                    positions[i] = joints[i - 1].position + (positions[i] - positions[i - 1]).normalized * boneLength[i - 1];
                }

                //If ya close enough, stop bb
                if (Vector3.Distance(positions[positions.Length - 1], target.position) < 0.001f)
                    return;
            }


            //Oh boy Mr. Krabs
            //Here we go sonny
            //We have to bend the joints towards our elbow thing
            //SO we have to create a plane for each joint (other than the root) and find the closest point in a circle on that plane to the elbow thing
            for (int i = 1; i < joints.Length - 1; i++)
            {
                Plane plane = new Plane(joints[i + 1].position - joints[i - 1].position, joints[i - 1].position);
                Vector3 projectedPole = plane.ClosestPointOnPlane(elbowThing.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(joints[i].position);
                float angle = Vector3.SignedAngle(projectedBone - joints[i - 1].position, projectedPole - joints[i - 1].position, plane.normal);
                joints[i].position = Quaternion.AngleAxis(angle, plane.normal) * (joints[i].position - joints[i - 1].position) + joints[i - 1].position;
            }

            //Set each joint to the position we found
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].position = positions[i];
            }

        }
    }
}

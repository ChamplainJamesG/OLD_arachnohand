using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKSolver : MonoBehaviour
{

    public struct IKJoint
    {
        public Transform transform;
        public Vector3 savedPosition;
        public AnimationController.ObjAndTransform objAndT;
        public Vector3 localPosition;
        public Vector3 parentPosition;
    }

    public int boneAmount;

    public Transform elbowConstraint;

    public int iterations;

    public Transform shoulder;
    public Transform startingPos; 

    Vector3 target;
    Vector3 newTarget;

    float[] boneLength;
    float totalLength;
    IKJoint[] joints;

    bool grabbing;

    float distToClosestBitch;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!grabbing)
        {
            newTarget = startingPos.position;
            CheckForTargets();
        }

        target = Vector3.MoveTowards(target, newTarget, Time.deltaTime * 10);

        ResolveIK();
    }

    void Init()
    {
        grabbing = false;

        joints = new IKJoint[boneAmount + 1];
        boneLength = new float[boneAmount];
        totalLength = 0;

        Transform current = transform;

        for (int i = joints.Length - 1; i >= 0; i--)
        {
            joints[i].transform = current;

            if (i != joints.Length - 1)
            {
                boneLength[i] = (joints[i + 1].transform.position - current.position).magnitude;
                totalLength += boneLength[i];
            }
            joints[i].objAndT = new AnimationController.ObjAndTransform();
            joints[i].objAndT.tForm.translation = joints[i].transform.position;
            joints[i].objAndT.objRef = joints[i].transform.gameObject;
            joints[i].objAndT.tForm.rotation = Quaternion.identity;
            joints[i].objAndT.tForm.scale = Vector3.one;
            joints[i].localPosition = joints[i].transform.localPosition;
            joints[i].parentPosition = joints[i].transform.parent.position;
            current = current.parent;
        }

        target = newTarget =  startingPos.position;
    }

    void CheckForTargets()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 5.0f, (1 << 8));

        if (cols.Length != 0)
        {
            Vector3 closest = cols[0].ClosestPointOnBounds(shoulder.position);

            foreach (Collider c in cols)
            {
                Vector3 vec = c.ClosestPoint(shoulder.position);
                if (Vector3.Distance(shoulder.position, vec) < Vector3.Distance(shoulder.position, closest))
                {
                    closest = vec;
                }
            }
            RaycastHit hit;
            Physics.Raycast(shoulder.position, closest - shoulder.position, out hit, 10, (1 << 8));
            distToClosestBitch = Vector3.Distance(transform.position, closest) / 5.0f;
            newTarget = hit.point;

            grabbing = true;
        }
    }

    public void ResolveIK()
    {
        if (target == null)
            return;

        int i;
        Vector3 shoulderStart = joints[0].objAndT.tForm.translation;

        for (i = 0; i < joints.Length; i++)
            joints[i].savedPosition = joints[i].objAndT.tForm.translation;

        if ((target - joints[0].transform.position).sqrMagnitude >= totalLength * totalLength - 0.5f)
        {
            Vector3 dir = (target - joints[0].savedPosition).normalized;

            for (i = 1; i < joints.Length; i++)
                joints[i].savedPosition = joints[i - 1].savedPosition + dir * boneLength[i - 1];

            grabbing = false;
        }
        else if ((target - joints[0].transform.position).sqrMagnitude >= (totalLength - 0.5f) * (totalLength - 0.5f))
            CheckForTargets();

        int j;

        for (i = 0; i < iterations; i++)
        {
            joints[joints.Length - 1].savedPosition = target;
            for (j = joints.Length - 2; j > 0; j--)
                joints[j].savedPosition = joints[j + 1].savedPosition + (joints[j].savedPosition - joints[j + 1].savedPosition).normalized * boneLength[j];

            joints[0].objAndT.tForm.translation = shoulderStart;
            for (j = 1; j < joints.Length; j++)
                joints[j].savedPosition = joints[j - 1].savedPosition + (joints[j].savedPosition - joints[j - 1].savedPosition).normalized * boneLength[j - 1];

            if ((target - joints[joints.Length - 1].savedPosition).sqrMagnitude < 0.01f * 0.01f)
                break;
        }

        for(i = 1; i < joints.Length - 1; i++)
        {
            Plane plane = new Plane(joints[i + 1].savedPosition - joints[i - 1].savedPosition, joints[i - 1].savedPosition);
            Vector3 projectedPole = plane.ClosestPointOnPlane(elbowConstraint.position);
            Vector3 projectedBone = plane.ClosestPointOnPlane(joints[i].savedPosition);
            float angle = Vector3.SignedAngle(projectedBone - joints[i - 1].savedPosition, projectedPole - joints[i - 1].savedPosition, plane.normal);
            joints[i].savedPosition = Quaternion.AngleAxis(angle, plane.normal) * (joints[i].savedPosition - joints[i - 1].savedPosition) + joints[i - 1].savedPosition;
        }

        for (i = 0; i < joints.Length; i++)
        {
            //joints[i].localPosition = joints[i].transform.parent.position + joints[i].savedPosition;
            joints[i].objAndT.tForm.translation = joints[i].savedPosition;
            //var v = joints[i].transform.parent.InverseTransformPoint(joints[i].savedPosition);
            //var v = joints[i].savedPosition - joints[i].transform.parent.position;
            Vector3 v;
            if (i == 0)
            {
                v = joints[i].savedPosition - joints[i].transform.parent.position;
            }
            else
            {
                joints[i].parentPosition = joints[i - 1].savedPosition;
                v = joints[i].savedPosition - joints[i - 1].savedPosition;

            }
            joints[i].localPosition = v;
            //joints[i].objAndT.tForm.translation -= joints[i].transform.parent.position;
        }

    }


    public IKJoint[] GetArr()
    {
        return joints;
    }

    public List<AnimationController.ObjAndTransform> GetArrBetter()
    {
        List<AnimationController.ObjAndTransform> blehs = new List<AnimationController.ObjAndTransform>();
        for(int i = 0; i < joints.Length; ++i)
        {
            AnimationController.ObjAndTransform newBoy = new AnimationController.ObjAndTransform(joints[i].objAndT);

            newBoy.tForm.translation = joints[i].localPosition;
            blehs.Add(newBoy);
        }

        return blehs;
    }

    public float GetDist()
    {
        return distToClosestBitch;
    }

    public bool IsGrabbing()
    {
        return grabbing;
    }
}

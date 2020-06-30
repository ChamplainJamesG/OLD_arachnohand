using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class IKSolver2 : MonoBehaviour
{
    public int boneAmount;
    public Transform[] elbowConstraint;
    public int iterations;
    public float timeOffset;
    public CharacterController.Finger fingerType;
    public Transform grabTarget;
    float[] boneLength;
    float totalLength;
    Vector4 target;
    Vector4 newTarget;
    Transform[] joints;
    bool doIK;
    bool grabbing;

    BlendTransform[] IKData;
    CharacterController cc;


    void Start()
    {
        cc = FindObjectOfType<CharacterController>();
        boneLength = new float[boneAmount];
        totalLength = 0;
        doIK = true;
        grabbing = false;

        joints = new Transform[boneAmount + 1];
        IKData = new BlendTransform[boneAmount + 1];

        Transform current = transform;

        for (int i = boneAmount; i >= 0; i--)
        {
            joints[i] = current;
            IKData[i] = new BlendTransform();

            if (i != boneAmount)
            {
                boneLength[i] = (current.GetChild(0).position - current.position).magnitude;
                totalLength += boneLength[i];
            }

            current = current.parent;
        }

        cc.RaycastFinger((int)fingerType);
    }

    void FixedUpdate()
    {
        if (doIK)
        {
            ResolveIK();

            target = Vector4.MoveTowards(target, newTarget, Time.deltaTime * 30);

            if (grabbing)
                newTarget = grabTarget.position;
        }
    }

    public IEnumerator MoveFinger()
    {
        if (fingerType != CharacterController.Finger.thumb)
        {
            cc.RaycastFinger((int)fingerType);
            if (InputManager.ActiveDevice.LeftStick)
                yield return new WaitForSeconds(0.15f / InputManager.ActiveDevice.LeftStick.Vector.magnitude);
        }
        cc.RaycastFingerUp((int)fingerType);
        if (InputManager.ActiveDevice.LeftStick)
            yield return new WaitForSeconds(0.3f / InputManager.ActiveDevice.LeftStick.Vector.magnitude);
        cc.RaycastFinger((int)fingerType);
    }

    public void ResolveIK()
    {
        int i;

        for (i = 0; i < joints.Length; i++)
            IKData[i].position = joints[i].position;

        if ((target - IKData[0].position).sqrMagnitude >= totalLength * totalLength)
        {
            Vector4 dir = (target - IKData[0].position).normalized;

            for (i = 1; i < IKData.Length; i++)
                IKData[i].position = IKData[i - 1].position + dir * boneLength[i - 1];
        }
        else
        {
            int j;

            for (i = 0; i < iterations; i++)
            {
                IKData[IKData.Length - 1].position = target;
                for (j = IKData.Length - 2; j > 0; j--)
                    IKData[j].position = IKData[j + 1].position + (IKData[j].position - IKData[j + 1].position).normalized * boneLength[j];

                for (j = 1; j < IKData.Length; j++)
                    IKData[j].position = IKData[j - 1].position + (IKData[j].position - IKData[j - 1].position).normalized * boneLength[j - 1];

                if ((target - IKData[IKData.Length - 1].position).sqrMagnitude < 0.01f * 0.01f)
                    break;
            }

            for (i = 1; i < IKData.Length - 1; i++)
            {
                Vector3 elbowConstraintPos;

                if (i == IKData.Length - 1)
                    elbowConstraintPos = elbowConstraint[0].position;
                else
                    elbowConstraintPos = elbowConstraint[1].position;

                Plane plane = new Plane(IKData[i + 1].position - IKData[i - 1].position, IKData[i - 1].position);
                Vector4 projectedPole = plane.ClosestPointOnPlane(elbowConstraintPos);
                Vector4 projectedBone = plane.ClosestPointOnPlane(IKData[i].position);
                float angle = Vector3.SignedAngle(projectedBone - IKData[i - 1].position, projectedPole - IKData[i - 1].position, plane.normal);
                IKData[i].position = (Vector4)(Quaternion.AngleAxis(angle, plane.normal) * (IKData[i].position - IKData[i - 1].position)) + IKData[i - 1].position;
            }
        }

        for (i = 0; i < IKData.Length; i++)
            joints[i].position = IKData[i].position;
    }

    public void SetTarget(Vector3 t)
    {
        newTarget = t;
    }

    public void BeginGrabbing()
    {
        grabbing = true;
    }

    public void EndGrabbing()
    {
        grabbing = false;
    }

    public void StopIK()
    {
        doIK = false;
    }

    public void DoIK()
    {
        doIK = true;
        target = transform.position;
    }
}

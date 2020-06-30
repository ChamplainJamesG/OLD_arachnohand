using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendSub : TestBlendBase
{
    public Transform pose0, pose1;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        pose_result = gameObject.transform;
        pose_result.localPosition = pose0.localPosition - pose1.localPosition;
        pose_result.localScale = multVectors(pose0.localScale, pose1.localScale);

        if (usingQuaternionRotation)
            pose_result.localRotation = Quaternion.Inverse(pose0.localRotation) * pose1.localRotation;
        else
            pose_result.localEulerAngles = pose0.localEulerAngles - pose1.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

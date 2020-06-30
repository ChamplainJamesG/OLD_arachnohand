using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileLoader : MonoBehaviour
{
    public enum HTRState
    {
        start,
        header,
        hierarchy,
        basePosition,
        keyframes
    }

    AnimationController animationController;
    HTRState state;

    int numSegments;
    int numFrames;

    Joint jointBeingLoaded;

    void Start()
    {
        animationController = GetComponent<AnimationController>();
        animationController.Init();
        LoadFile("Assets/monster_anim.htr");
    }

    void Update()
    {
        
    }

    public void LoadFile(string fileName)
    {
        StreamReader reader = new StreamReader(fileName);
        state = HTRState.start;

        while(!reader.EndOfStream)
        {
            string line = reader.ReadLine();

            AnalyzeLine(line);
        }

        reader.Close();

        animationController.CreateJointVisuals();
    }

    void AnalyzeLine(string line)
    {
        if(line.StartsWith("#"))
        {
            return;
        }

        if(line.StartsWith("["))
        {
            ChangeState();
            if (state != HTRState.keyframes)
                return;
        }

        switch(state)
        {
            case HTRState.header:
                HeaderAnalyzer(line);
                break;
            case HTRState.hierarchy:
                HierarchyAnalyzer(line);
                break;
            case HTRState.basePosition:
                BasePositionAnalyzer(line);
                break;
            case HTRState.keyframes:
                KeyframeAnalyzer(line);
                break;
        }
    }

    void HeaderAnalyzer(string line)
    {
        string[] split = line.Split(' ');
            
        switch(split[0])
        {
            case "NumSegments":
                numSegments = int.Parse(split[1]);
                break;
            case "NumFrames":
                numFrames = int.Parse(split[1]);
                break;
        }
    }

    void HierarchyAnalyzer(string line)
    {
        string[] split = line.Split('\t');

        Joint joint = new Joint();
        joint.Init(numFrames, split[0]);

        if(split[1] != "GLOBAL")
        {
            joint.parentJoint = animationController.joints[split[1]];
        }

        animationController.joints.Add(joint.jointName, joint);
        animationController.jointPos.Add(joint);
    }

    void BasePositionAnalyzer(string line)
    {
        string[] split = line.Split('\t');

        AnimationController.Keyframe basePos = animationController.joints[split[0]].basePosition;

        basePos.translationX = float.Parse(split[1]);
        basePos.translationY = float.Parse(split[2]);
        basePos.translationZ = float.Parse(split[3]);
        basePos.rotationX = float.Parse(split[4]);
        basePos.rotationY = float.Parse(split[5]);
        basePos.rotationZ = float.Parse(split[6]);
        basePos.boneLength = float.Parse(split[7]);
        Matrix4x4 trs;
        Vector3 t = new Vector3(basePos.translationX, basePos.translationY, basePos.translationZ);
        Quaternion r = Quaternion.identity;
        r.eulerAngles = new Vector3(basePos.rotationX, basePos.rotationY, basePos.rotationZ);
        trs = Matrix4x4.TRS(t, r, Vector3.one);
        animationController.joints[split[0]].SetTRS(trs);
        //animationController.joints[split[0]].SetTRS(trs);
    }

    void KeyframeAnalyzer(string line)
    {
        if(line.StartsWith("["))
        {
            line = line.Replace("[", string.Empty);
            line = line.Replace("]", string.Empty);
            if (line == "EndOfFile")
                return;
            jointBeingLoaded = animationController.joints[line];
            return;
        }

        string[] split = line.Split('\t');
        jointBeingLoaded.keyframes[int.Parse(split[0])] = new AnimationController.Keyframe();
        jointBeingLoaded.keyframes[int.Parse(split[0])].translationX = float.Parse(split[1]) + jointBeingLoaded.basePosition.translationX;
        jointBeingLoaded.keyframes[int.Parse(split[0])].translationY = float.Parse(split[2]) + jointBeingLoaded.basePosition.translationY;
        jointBeingLoaded.keyframes[int.Parse(split[0])].translationZ = float.Parse(split[3]) + jointBeingLoaded.basePosition.translationZ;
        jointBeingLoaded.keyframes[int.Parse(split[0])].rotationX = float.Parse(split[4]) + jointBeingLoaded.basePosition.rotationX;
        jointBeingLoaded.keyframes[int.Parse(split[0])].rotationY = float.Parse(split[5]) + jointBeingLoaded.basePosition.rotationY;
        jointBeingLoaded.keyframes[int.Parse(split[0])].rotationZ = float.Parse(split[6]) + jointBeingLoaded.basePosition.rotationZ;
        jointBeingLoaded.keyframes[int.Parse(split[0])].boneLength = float.Parse(split[7]);
    }

    void ChangeState()
    {
        if(state != HTRState.keyframes)
            state = state + 1;
    }
}

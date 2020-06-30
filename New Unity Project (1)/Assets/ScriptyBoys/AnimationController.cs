using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public class Keyframe
    {
        public float translationX;
        public float translationY;
        public float translationZ;
        public float rotationX;
        public float rotationY;
        public float rotationZ;
        public float boneLength;
    }

    public Dictionary<string, Joint> joints;
    public Dictionary<string, GameObject> objects;
    public List<Joint> jointPos = new List<Joint>();
    public GameObject jointPrefab;
    public Material lineMat;

    FileLoader fl;
    bool draw;

    private int frameIndex = 0;

    void Start()
    {


        //fl.LoadFile("Assets/monster_anim.htr");

        //CreateJointVisuals();
    }

    void Update()
    {
        if (draw)
        {
            foreach (KeyValuePair<string, Joint> j in joints)
            {
                DoForwardKinematicsPartial(0, jointPos.Count);
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                ++frameIndex;
                if (frameIndex > joints["main"].keyframes.Length)
                    frameIndex = 0;

                UpdateJointVisuals();
            }

            foreach (KeyValuePair<string, GameObject> j in objects)
            {
                if (j.Value.transform.parent != null)
                {
                    Debug.DrawLine(j.Value.transform.position, j.Value.transform.parent.transform.position);
                    var jo = joints[j.Key];
                    // post kinematics set world position
                    Vector3 position = new Vector3(jo.getWTRS().m03, jo.getWTRS().m13, jo.getWTRS().m23);
                    j.Value.transform.localPosition = position;
                    Debug.Log("<color=blue>"+ position + "</color>");
                    Debug.Log("<color=red>" + j.Value.transform.position + "</color>");
                }
            }

        }
    }

    public void Init()
    {
        joints = new Dictionary<string, Joint>();
        objects = new Dictionary<string, GameObject>();
        fl = GetComponent<FileLoader>();
        draw = false;
    }

    public void CreateJointVisuals()
    {
        foreach (KeyValuePair<string, Joint> j in joints)
        {
            GameObject obj = Instantiate(jointPrefab);
            if (j.Value.parentJoint != null)
            {
                obj.transform.parent = objects[j.Value.parentJoint.jointName].transform;
                obj.transform.localEulerAngles = new Vector3(j.Value.basePosition.rotationZ, j.Value.basePosition.rotationY, j.Value.basePosition.rotationX);
                obj.transform.localPosition = new Vector3(j.Value.basePosition.translationZ, j.Value.basePosition.translationY, j.Value.basePosition.translationX);
                
            }
            else
            {
                obj.transform.eulerAngles = new Vector3(j.Value.basePosition.rotationZ, j.Value.basePosition.rotationY, j.Value.basePosition.rotationX);
                obj.transform.position = new Vector3(j.Value.basePosition.translationZ, j.Value.basePosition.translationY, j.Value.basePosition.translationX);
            }

            obj.name = j.Value.jointName;
            objects.Add(obj.name, obj);
        }
        draw = true;
    }

    public void UpdateJointVisuals()
    {
        foreach(KeyValuePair<string, GameObject> kvp in objects)
        {
            Joint j = joints[kvp.Key];
            //kvp.Value.transform.position += j.keyframes[frameIndex].translationX;
            Vector3 v = kvp.Value.transform.position;
            //v.x += j.keyframes[frameIndex].translationX;
            //v.y += j.keyframes[frameIndex].translationY;
            //v.z += j.keyframes[frameIndex].translationZ;

            var v2 = new Vector3(j.keyframes[frameIndex].translationX, j.keyframes[frameIndex].translationY, j.keyframes[frameIndex].translationZ);

            var newPos = Vector3.Lerp(v, v2, 1.0f);

            kvp.Value.transform.localPosition = newPos;

            Vector3 r1 = kvp.Value.transform.rotation.eulerAngles;
            var r2 = new Vector3(j.keyframes[frameIndex].rotationX, j.keyframes[frameIndex].rotationY, j.keyframes[frameIndex].rotationZ);

            var newRot = Vector3.Lerp(r1, r2, 1.0f);
            kvp.Value.transform.eulerAngles = newRot;
        }
    }

    public void DoForwardKinematicsPartial(int firstIndex, int count)
    {
        int i = 0;
        int end = firstIndex + count;
        for(i = firstIndex; i < end; ++i)
        {
            int parentIndex = jointPos.FindIndex(x => x.Equals(jointPos[i].parentJoint));
            if(jointPos[i].parentJoint != null)
            {
                var newTrs = jointPos[i].getTRS() * jointPos[parentIndex].getTRS();
                jointPos[i].setWTRS(newTrs);
            }
        }
    }

}

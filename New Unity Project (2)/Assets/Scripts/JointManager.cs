using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JointManager : MonoBehaviour
{
    // Keep references to joints, because I'm concerned.
    private static int jointCount = 0;

    public class Joint
    {
        public Joint(Transform newT)
        {
            tRef = newT;
            count = jointCount++;
        }

        public Transform tRef;
        public int count;

        public void SetJoint(BlendTransform t)
        {
            if(t.deltaPThisFrame != Vector4.zero)
                tRef.localPosition = t.position;
            if(t.deltaRotThisFrame != Quaternion.identity)
                tRef.localRotation = t.rotation;
            if(t.deltaScaleThisFrame != Vector4.zero)
                tRef.localScale = t.scale;
        }
    }

    public static List<Joint> joints = new List<Joint>();

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        joints.Add(new Joint(transform));
        foreach(Transform c in transform)
        {
            if (c.tag != "Joint")
                continue;
            joints.Add(new Joint(c));
            CheckJoints(c);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode m)
    {
        joints.Clear();
    }

    private void CheckJoints(Transform parent)
    {
        foreach(Transform c in parent)
        {
            if (c.tag != "Joint")
                continue;
            joints.Add(new Joint(c));
            CheckJoints(c);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class KeyframeLoader : MonoBehaviour
{
    public readonly string UPPER_DELIMITER = "Spine1";
    public readonly string LOWER_DELIMITER = "Pelvis";
    public readonly string UPPER_FILE_DELIMITER = "upper";
    public readonly string LOWER_FILE_DELIMITER = "lower";

    public enum KEY_MASK
    {
        UPPER = 0,
        LOWER = 1,
        FULL = 2
    }

    // Start is called before the first frame update
    void Start()
    {
        //DEBUG:
        //LoadKeyframe("clip1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public static List<AnimationController.KeyFrame> LoadKeyframe(string name)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(Application.streamingAssetsPath +"/" + name + ".xml");

        var allFrames = doc.DocumentElement.SelectNodes("/root/keyframe");
        List<AnimationController.KeyFrame> frames = new List<AnimationController.KeyFrame>();
        foreach(XmlNode kFrame in allFrames)
        {
            AnimationController.KeyFrame frame = new AnimationController.KeyFrame();

            AddJoints(kFrame, ref frame);
            frame.keyTime = 0.3f;
            frames.Add(frame);
        }

        return frames;
    }

    private static void AddJoints(XmlNode keyNode, ref AnimationController.KeyFrame frame)
    {
        var jointList = keyNode.SelectNodes("joint");

        foreach(XmlNode j in jointList)
        {
            var b = new AnimationController.KeyBone();
            b.boneIndex = int.Parse(j.Attributes[0].Value);
            b.translation = LoadVectorFromNode(j.SelectSingleNode("transform"));
            b.rotation = LoadQuatFromNode(j.SelectSingleNode("rotation"));
            b.scale = LoadVectorFromNode(j.SelectSingleNode("scale"));

            frame.bones.Add(b);
        }
    }

    //public static Transform FindDeepChild(string name, Transform thisT)
    //{
    //    for (int i = 0; i < thisT.childCount; ++i)
    //    {
    //        if (thisT.GetChild(i).name == name)
    //            return thisT.GetChild(i);
    //        else
    //            FindDeepChild(name, thisT.GetChild(i));
    //    }

    //    return null;
    //}

    public static Transform FindDeepChild(string name, Transform parent)
    {
        foreach(Transform c in parent)
        {
            if (c.name == name)
                return c;
            var result = FindDeepChild(name, c);
            if (result != null)
                return result;
        }

        return null;
    }

    private static Vector3 LoadVectorFromNode(XmlNode n)
    {
        Vector3 v = Vector3.zero;
        v.x = float.Parse( n.Attributes[0].Value);
        v.y = float.Parse( n.Attributes[1].Value);
        v.z = float.Parse( n.Attributes[2].Value);

        return v;
    }

    private static Quaternion LoadQuatFromNode(XmlNode n)
    {
        Quaternion q = Quaternion.identity;
        q.x = float.Parse(n.Attributes[0].Value);
        q.y = float.Parse(n.Attributes[1].Value);
        q.z = float.Parse(n.Attributes[2].Value);

        return q;
    }
}


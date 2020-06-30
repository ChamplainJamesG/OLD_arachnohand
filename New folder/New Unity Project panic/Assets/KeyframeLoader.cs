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
        // DEBUG:
        LoadKeyframe(KEY_MASK.UPPER, "walking_lower");
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public List<AnimationController.KeyFrame> LoadKeyframe(KEY_MASK mask, string name)
    {
        string delToUse = "";
        string baseToUse = "";
        switch(mask)
        {
            case KEY_MASK.UPPER:
                delToUse = UPPER_FILE_DELIMITER;
                baseToUse = UPPER_DELIMITER;
                break;
            case KEY_MASK.LOWER:
                delToUse = LOWER_FILE_DELIMITER;
                baseToUse = LOWER_DELIMITER;
                break;
            default:
                break;
        }
        XmlDocument doc = new XmlDocument();
        doc.Load(Application.streamingAssetsPath +"/" + name + ".xml");

        var allFrames = doc.DocumentElement.SelectNodes("/root/keyframe");
        List<AnimationController.KeyFrame> frames = new List<AnimationController.KeyFrame>();
        foreach(XmlNode kFrame in allFrames)
        {
            AnimationController.KeyFrame frame = new AnimationController.KeyFrame();
            var baseD = kFrame.SelectSingleNode("Root/" + baseToUse);
            AnimationController.ObjAndTransform oat = new AnimationController.ObjAndTransform();
            oat.objRef = FindDeepChild(baseD.Name, transform).gameObject;
            oat.tForm.translation = LoadVectorFromNode(baseD.SelectSingleNode("transform"));
            oat.tForm.scale = LoadVectorFromNode(baseD.SelectSingleNode("scale"));
            oat.tForm.rotation = LoadQuatFromNode(baseD.SelectSingleNode("rotation"));
            frame.objTs.Add(oat);
            var children = baseD.ChildNodes;
            for(int i = 0; i < children.Count; ++i)
            {
                if (children.Item(i).Attributes.Count == 0)
                    LoadItALot(ref frame, children.Item(i));
            }

            frame.keyTime = 0.3f;
            frames.Add(frame);
        }

        return frames;
    }

    private void LoadItALot(ref AnimationController.KeyFrame frame, XmlNode me)
    {
        var children = me.ChildNodes;
        AnimationController.ObjAndTransform oat = new AnimationController.ObjAndTransform();
        oat.objRef = FindDeepChild(me.Name, transform).gameObject;
        oat.tForm.translation = LoadVectorFromNode(me.SelectSingleNode("transform"));
        oat.tForm.rotation = LoadQuatFromNode(me.SelectSingleNode("rotation"));
        oat.tForm.scale = LoadVectorFromNode(me.SelectSingleNode("scale"));
        frame.objTs.Add(oat);
        for(int i = 0; i < children.Count; ++i)
        {
            if (children.Item(i).Attributes.Count == 0)
                LoadItALot(ref frame, children.Item(i));
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

    private Vector3 LoadVectorFromNode(XmlNode n)
    {
        Vector3 v = Vector3.zero;
        v.x = float.Parse( n.Attributes[0].Value);
        v.y = float.Parse( n.Attributes[1].Value);
        v.z = float.Parse( n.Attributes[2].Value);

        return v;
    }

    private Quaternion LoadQuatFromNode(XmlNode n)
    {
        Quaternion q = Quaternion.identity;
        q.x = float.Parse(n.Attributes[0].Value);
        q.y = float.Parse(n.Attributes[1].Value);
        q.z = float.Parse(n.Attributes[2].Value);

        return q;
    }
}


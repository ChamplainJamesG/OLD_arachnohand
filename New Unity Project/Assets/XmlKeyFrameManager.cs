using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;


public class XmlKeyFrameManager : MonoBehaviour
{
    private Dictionary<int, List<KeyframeRecorder.Recording>> recs = new Dictionary<int, List<KeyframeRecorder.Recording>>();
    // Dictionary to store all keyframes.

    // Start is called before the first frame update
    void Start()
    {
        //SaveClip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        //SaveClip();
    }

    public void SaveOneClip(int index, KeyframeRecorder.Recording r)
    {
        if (recs.ContainsKey(index))
            recs[index].Add(r);
        else
        {
            recs.Add(index, new List<KeyframeRecorder.Recording>());
            recs[index].Add(r);
        }
    }

    /// <summary>
    /// Save all the keyframes that were generated for each object, into an XML.
    /// </summary>
    public void SaveClip()
    {
        // Initial XML construction
        XmlDocument doc = new XmlDocument();
        var root = doc.CreateElement("root");
        doc.AppendChild(root);

        // Go through each pair within the recording structure.
        foreach(KeyValuePair<int, List<KeyframeRecorder.Recording>> recordings in recs)
        {
            // Create parent node for an "object". An animatable object.
            XmlElement objId = doc.CreateElement("objectid");
            XmlAttribute objIdAttr = doc.CreateAttribute("id");
            objIdAttr.Value = recordings.Key.ToString();
            objId.Attributes.Append(objIdAttr);
            root.AppendChild(objId);
            // Construct keyframe nodes within the parent.
            for(int i = 0; i < recordings.Value.Count; ++i)
            {
                // Keyframe parent node.
                XmlElement keyNode = doc.CreateElement("keyframe");
                XmlAttribute keyAttr = doc.CreateAttribute("index");
                keyAttr.Value = i.ToString();
                keyNode.Attributes.Append(keyAttr);
                objId.AppendChild(keyNode);

                // Now we need the duration of the keyframe, or how long from time = 0 it is.
                XmlElement keyDur = doc.CreateElement("duration");
                XmlAttribute durAttr = doc.CreateAttribute("time");
                durAttr.Value = recordings.Value[i].time.ToString();
                keyDur.Attributes.Append(durAttr);
                keyNode.AppendChild(keyDur);

                // Construct the transform at that time. This is the postion value.
                XmlElement transNode = doc.CreateElement("transform");
                XmlAttribute transX = doc.CreateAttribute("x");
                transX.Value = recordings.Value[i].position.x.ToString();
                XmlAttribute transY = doc.CreateAttribute("y");
                transY.Value = recordings.Value[i].position.y.ToString();
                XmlAttribute transZ = doc.CreateAttribute("z");
                transZ.Value = recordings.Value[i].position.z.ToString();
                transNode.Attributes.Append(transX);
                transNode.Attributes.Append(transY);
                transNode.Attributes.Append(transZ);
                keyNode.AppendChild(transNode);

                // This is the rotation value.
                XmlElement rotNode = doc.CreateElement("rotation");
                XmlAttribute rotX = doc.CreateAttribute("x");
                rotX.Value = recordings.Value[i].rotation.x.ToString();
                XmlAttribute rotY = doc.CreateAttribute("y");
                rotY.Value = recordings.Value[i].rotation.y.ToString();
                XmlAttribute rotZ = doc.CreateAttribute("z");
                rotZ.Value = recordings.Value[i].rotation.z.ToString();
                rotNode.Attributes.Append(rotX);
                rotNode.Attributes.Append(rotY);
                rotNode.Attributes.Append(rotZ);
                keyNode.AppendChild(rotNode);
            }
        }

        // Save
        doc.Save(Application.streamingAssetsPath + "/clip" + 1 + ".xml");
    }

    /// <summary>
    /// Loads the keyframes for a specific object, based on an id.
    /// </summary>
    /// <param name="clipIndex">The object index within the clip.</param>
    /// <returns>A list of keyframes.</returns>
    public List<KeyFrameController.KeyFrame> LoadClip(int clipIndex)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(Application.streamingAssetsPath + "/clip" + 1 + ".xml");

        // Go through and grab the stuff, put it in the new struct.
        List< KeyFrameController.KeyFrame> frames = new List<KeyFrameController.KeyFrame>();

        // Read and give the recordings back to whoever needs them.
        XmlNode node = doc.DocumentElement.SelectSingleNode("/root/objectid[@id='" + clipIndex.ToString() +"']");
        foreach(XmlNode keyframe in node.ChildNodes)
        {
            KeyFrameController.KeyFrame frame = new KeyFrameController.KeyFrame();
            float duration = float.Parse(keyframe.SelectSingleNode("duration").Attributes["time"].Value);
            Vector3 position = ReadVec3(keyframe.SelectSingleNode("transform"));
            Vector3 rotation = ReadVec3(keyframe.SelectSingleNode("rotation"));

            frame.duration = 0; // Will be figured out by the keyframes.
            frame.position = position;
            frame.rotation = rotation;
            frame.time = duration;

            frames.Add(frame);
        }

        return frames;
    }

    /// <summary>
    /// Helper function to read vector 3's from the XML.
    /// </summary>
    /// <param name="n">The node whose attributes we need to read.</param>
    /// <returns>A vector 3 from that node.</returns>
    private Vector3 ReadVec3(XmlNode n)
    {
        Vector3 v = new Vector3();
        v.x = float.Parse(n.Attributes["x"].Value);
        v.y = float.Parse(n.Attributes["y"].Value);
        v.z = float.Parse(n.Attributes["z"].Value);

        return v;
    }
}

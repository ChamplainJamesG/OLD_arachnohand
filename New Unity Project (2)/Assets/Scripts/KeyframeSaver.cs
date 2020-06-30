using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
public class KeyframeSaver : MonoBehaviour
{
    public int curKey;
    [ContextMenu("Save")]
    public void Save()
    {
        XmlDocument doc = new XmlDocument();
        if (!File.Exists(Application.streamingAssetsPath + "/clip1.xml"))
        {
            var f = File.Create(Application.streamingAssetsPath + "/clip1.xml");
            f.Close();
            doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        }
        else
            doc.Load(Application.streamingAssetsPath + "/clip1.xml");
        if(doc.SelectSingleNode("root") == null)
        {
            var root1 = doc.CreateElement("root");
            doc.AppendChild(root1);
        }

        XmlNode root = doc.SelectSingleNode("root");

        SaveKeyFrame(ref doc, root);

        doc.Save(Application.streamingAssetsPath + "/clip1.xml");
    }

    private void SaveKeyFrame(ref XmlDocument doc, XmlNode root)
    {
        var keyFrame = doc.CreateElement("keyframe");
        var attr = doc.CreateAttribute("frameVal");
        attr.Value = curKey.ToString();
        keyFrame.Attributes.Append(attr);

        root.AppendChild(keyFrame);
        int jointNum = 0;
        SaveThisJoint(transform, ref doc, keyFrame ,ref jointNum);
        foreach(Transform t in transform)
        {
            if (t.gameObject.tag != "Joint")
                continue;
            SaveThisJoint(t, ref doc, keyFrame, ref jointNum);
            SaveItAlot(t, ref doc, keyFrame, ref jointNum);
        }
    }

    private void SaveItAlot(Transform t, ref XmlDocument doc, XmlNode parent, ref int jointNum)
    {
        //SaveThisJoint(t, ref doc, parent, ref jointNum);
        foreach (Transform tr in t)
        {
            if (tr.gameObject.tag != "Joint")
                continue;
            SaveThisJoint(tr, ref doc, parent, ref jointNum);
            SaveItAlot(tr, ref doc, parent, ref jointNum);
        }
    }

    private void SaveThisJoint(Transform t, ref XmlDocument doc, XmlNode parent, ref int jointNum)
    {
        var joint = doc.CreateElement("joint");
        var idAttr = doc.CreateAttribute("id");
        idAttr.Value = jointNum.ToString();
        ++jointNum;

        joint.Attributes.Append(idAttr);

        XmlElement transNode = doc.CreateElement("transform");
        XmlAttribute transX = doc.CreateAttribute("x");
        transX.Value = t.localPosition.x.ToString();
        XmlAttribute transY = doc.CreateAttribute("y");
        transY.Value = t.localPosition.y.ToString();
        XmlAttribute transZ = doc.CreateAttribute("z");
        transZ.Value = t.localPosition.z.ToString();
        transNode.Attributes.Append(transX);
        transNode.Attributes.Append(transY);
        transNode.Attributes.Append(transZ);
        joint.AppendChild(transNode);

        // This is the rotation value.
        XmlElement rotNode = doc.CreateElement("rotation");
        XmlAttribute rotX = doc.CreateAttribute("x");
        rotX.Value = t.localRotation.x.ToString();
        XmlAttribute rotY = doc.CreateAttribute("y");
        rotY.Value = t.localRotation.y.ToString();
        XmlAttribute rotZ = doc.CreateAttribute("z");
        rotZ.Value = t.localRotation.z.ToString();
        rotNode.Attributes.Append(rotX);
        rotNode.Attributes.Append(rotY);
        rotNode.Attributes.Append(rotZ);
        joint.AppendChild(rotNode);

        XmlElement scaleNode = doc.CreateElement("scale");
        XmlAttribute scaleX = doc.CreateAttribute("x");
        scaleX.Value = t.localScale.x.ToString();
        XmlAttribute scaleY = doc.CreateAttribute("y");
        scaleY.Value = t.localScale.y.ToString();
        XmlAttribute scaleZ = doc.CreateAttribute("z");
        scaleZ.Value = t.localScale.z.ToString();
        scaleNode.Attributes.Append(scaleX);
        scaleNode.Attributes.Append(scaleY);
        scaleNode.Attributes.Append(scaleZ);
        joint.AppendChild(scaleNode);

        parent.AppendChild(joint);
    }
}

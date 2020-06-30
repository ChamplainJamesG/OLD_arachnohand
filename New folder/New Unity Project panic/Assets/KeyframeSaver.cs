using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;

public class KeyframeSaver : EditorWindow
{
    string fileName = "file";
    string keyframeNum = "0";
    string niceMessage = "We love you, Ball Boy";

    XmlDocument doc;
    XmlElement keyframe;

    [MenuItem("Window/Keyframe Saver")]
    static void OpenWindow()
    {
        KeyframeSaver window = (KeyframeSaver)GetWindow(typeof(KeyframeSaver));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("\n\nPlease Select The Root Of The Ball Boy You Wish To Save");
        fileName = EditorGUILayout.TextField("File Name:", fileName);
        keyframeNum = EditorGUILayout.TextField("Keyframe Number:", keyframeNum);
        niceMessage = EditorGUILayout.TextField("Nice Message for Ball Boy:", niceMessage);

        if (GUILayout.Button("Save Pose As Keyframe") && Selection.activeGameObject)
        {
            doc = new XmlDocument();
            bool flag = false;

            if (!File.Exists(Application.streamingAssetsPath + "/" + fileName + ".xml"))
            {
                flag = true;
                var file = File.Create(Application.streamingAssetsPath + "/" + fileName + ".xml");
                file.Close();
            }

            XmlNode root;

            if (!flag)
            {
                doc.Load(Application.streamingAssetsPath + "/" + fileName + ".xml");
                root = doc.SelectSingleNode("root");
            }
            else
            { 
                root = doc.CreateElement("root");
                doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            }

            doc.AppendChild(root);

            keyframe = doc.CreateElement("keyframe");
            XmlAttribute num = doc.CreateAttribute("num");
            num.Value = keyframeNum;
            keyframe.Attributes.Append(num);

            root.AppendChild(keyframe);

            SaveKeyFrame(Selection.activeGameObject.transform, keyframe);

            doc.Save(Application.streamingAssetsPath + "/" + fileName + ".xml");

            Debug.Log("<color=blue>Saved pose...</color>");
            Debug.Log("<color=green>" + niceMessage + "</color>");
        }
    }

    void SaveKeyFrame(Transform obj, XmlNode parent)
    {
        var newParent = XMLSave(obj, parent);

        for(int i = 0; i < obj.childCount; i++)
        {
            SaveKeyFrame(obj.GetChild(i), newParent);
        }
    }

    XmlNode XMLSave(Transform obj, XmlNode parent)
    {
        XmlElement joint = doc.CreateElement(obj.gameObject.name);
        parent.AppendChild(joint);

        XmlElement transNode = doc.CreateElement("transform");
        XmlAttribute transX = doc.CreateAttribute("x");
        transX.Value = obj.localPosition.x.ToString();
        XmlAttribute transY = doc.CreateAttribute("y");
        transY.Value = obj.localPosition.y.ToString();
        XmlAttribute transZ = doc.CreateAttribute("z");
        transZ.Value = obj.localPosition.z.ToString();
        transNode.Attributes.Append(transX);
        transNode.Attributes.Append(transY);
        transNode.Attributes.Append(transZ);
        joint.AppendChild(transNode);

        // This is the rotation value.
        XmlElement rotNode = doc.CreateElement("rotation");
        XmlAttribute rotX = doc.CreateAttribute("x");
        rotX.Value = obj.localRotation.x.ToString();
        XmlAttribute rotY = doc.CreateAttribute("y");
        rotY.Value = obj.localRotation.y.ToString();
        XmlAttribute rotZ = doc.CreateAttribute("z");
        rotZ.Value = obj.localRotation.z.ToString();
        rotNode.Attributes.Append(rotX);
        rotNode.Attributes.Append(rotY);
        rotNode.Attributes.Append(rotZ);
        joint.AppendChild(rotNode);

        XmlElement scaleNode = doc.CreateElement("scale");
        XmlAttribute scaleX = doc.CreateAttribute("x");
        scaleX.Value = obj.localScale.x.ToString();
        XmlAttribute scaleY = doc.CreateAttribute("y");
        scaleY.Value = obj.localScale.y.ToString();
        XmlAttribute scaleZ = doc.CreateAttribute("z");
        scaleZ.Value = obj.localScale.z.ToString();
        scaleNode.Attributes.Append(scaleX);
        scaleNode.Attributes.Append(scaleY);
        scaleNode.Attributes.Append(scaleZ);
        joint.AppendChild(scaleNode);

        return joint;
    }
}

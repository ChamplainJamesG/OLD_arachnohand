using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyFramePlayBack : MonoBehaviour
{
    public XmlKeyFrameManager manager;

    private List<KeyFrameController.KeyFrame> frames = new List<KeyFrameController.KeyFrame>();

    // Start is called before the first frame update
    void Start()
    {
        frames = manager.LoadClip(GetComponent<AnimeTable>().ID);
        ConstructDuration();
        DebugFrame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ConstructDuration()
    {
        for(int i = 0; i < frames.Count - 1; ++i)
        {
            frames[i].duration = frames[i + 1].time - frames[i].time;
        }

    }

    private void DebugFrame()
    {
        int c = 0;
        foreach(var f in frames)
        {
            Debug.Log("<color=yellow>For frame: " + c + "</color>");
            Debug.Log("<color=red>Position is: " + f.position + "</color>");
            Debug.Log("<color=blue>Rotation is: " + f.rotation + "</color>");
            Debug.Log("<color=black>Time is: " + f.time + "</color>");
            Debug.Log("<color=green>Duration is: " + f.duration + "</color>");

            ++c;
        }
    }
}

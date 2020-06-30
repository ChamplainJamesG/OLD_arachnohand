using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyFrameController : MonoBehaviour
{
    public enum PlayDirection
    {
        REVERSE = -1,
        PAUSED = 0,
        FORWARD = 1
    }
    /// <summary>
    /// Slightly different keyframe structure that stores a different thing. 
    /// </summary>
    public class KeyFrame
    {
        public Vector3 position;
        public Vector3 rotation;
        public float time;
        public float duration;
    }
    //public Dictionary<int, List<KeyFrame>> allKeyFrames = new Dictionary<int, List<KeyFrame>>();
    public float speed;
    /// <summary>
    /// Construct that controls playing the animation.
    /// </summary>
    public class KeyFrameControl
    {
        public int index;
        public GameObject animObj;
        public List<KeyFrame> frames = new List<KeyFrame>();
        public float timeInFrame = 0.0f;
        public bool play;

        /// <summary>
        /// Does the thing we did in the lab.
        /// </summary>
        /// <param name="dt">change in time.</param>
        /// <param name="d">current direction we're playing in.</param>
        public void UpdateControl(float dt, PlayDirection d, float speed)
        {
            timeInFrame += dt * (int)d * speed;

            bool solving = true;
            while(solving)
            {
                if (frames[index].duration == 0.0f)
                    break;

                int nextIndex;
                switch(d)
                {
                    case PlayDirection.FORWARD:
                        if (timeInFrame >= frames[index].duration)
                        {
                            timeInFrame -= frames[index].duration;

                            nextIndex = index + 1;
                            if (nextIndex >= frames.Count)
                            {
                                --nextIndex;
                            }

                            index = nextIndex;
                        }
                        else
                            solving = false;
                        break;
                    case PlayDirection.PAUSED:
                        solving = false;
                        break;
                    case PlayDirection.REVERSE:
                        solving = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Lerps the things so that it looks like they animate.
        /// </summary>
        /// <param name="speed">How fast they lerp.</param>
        public void AnimateController(float speed)
        {
            // Stop playing back if we're at the end.
            if (index == frames.Count - 1)
            {
                animObj.transform.position = frames[index].position;
                animObj.transform.eulerAngles = frames[index].rotation;
                return;
            }

            // Otherwise, lerp the position and rotation of the object.
            //timeInFrame += Time.deltaTime * speed;
            float normalTime = timeInFrame / frames[index].duration; // NORMALIZE YOUR SHIT JIMMY
            animObj.transform.position = Vector3.Lerp(frames[index].position, frames[index + 1].position, normalTime);                
            animObj.transform.eulerAngles = new Vector3(Mathf.LerpAngle(frames[index].rotation.x, frames[index + 1].rotation.x, normalTime),
                Mathf.LerpAngle(frames[index].rotation.y, frames[index + 1].rotation.y, normalTime), 
                Mathf.LerpAngle(frames[index].rotation.z, frames[index + 1].rotation.z, normalTime));
            
            /*
            if (animObj.transform.position == frames[index + 1].position)
            {
                ++index;
                timeInFrame = 0;
            }
            */
        }
    }
    public List<KeyFrameControl> kc = new List<KeyFrameControl>();

    public XmlKeyFrameManager manager;

    public PlayDirection playDir;

    // Start is called before the first frame update
    void Start()
    {
        //GetAllAnimatableObjects();
    }

    // Update is called once per frame
    // Checking whether we are outside of the current keyframe.
    /*
        ---------------->
        -------->
         __		__		__
         f1     f2		f3
    */

    /*
        <-----------------
                 <--------
        __		__		__
        f1		f2		f3
    */
    void Update()
    {
        Debug.Log(Time.deltaTime);
        for(int i = 0; i < kc.Count; ++i)
        {
            kc[i].UpdateControl(Time.deltaTime, playDir, speed);
            kc[i].AnimateController(speed);
        }
    }

    /// <summary>
    /// Each controller controls it's own object. It's like we have a parent controller (this), that contains each object's own controller.
    /// </summary>
    public void GetAllAnimatableObjects()
    {
        var allAnimatables = FindObjectsOfType<AnimeTable>();
        
        foreach(AnimeTable a in allAnimatables)
        {
            KeyFrameControl c = new KeyFrameControl();
            var keyList = manager.LoadClip(a.ID);

            ConstructDuration(keyList);

            c.index = 0;
            c.animObj = a.gameObject;
            c.frames = keyList;
            kc.Add(c);
        }
    }

    /// <summary>
    /// Since we don't save the duration of each frame, we have to do it ourselves on start.
    /// </summary>
    /// <param name="frames"></param>
    private void ConstructDuration(List<KeyFrame> frames)
    {
        for (int i = 0; i < frames.Count - 1; ++i)
        {
            frames[i].duration = frames[i + 1].time - frames[i].time;
        }

    }
}

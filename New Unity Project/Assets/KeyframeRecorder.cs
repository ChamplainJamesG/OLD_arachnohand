using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyframeRecorder : MonoBehaviour
{
    [System.Serializable]
    public enum RecordingType
    {
        positionBased,
        timeBased
    }

    public RecordingType recordingType;

    public bool representKeyframesAsGameobjects;

    public GameObject keyframeObject;

    public bool record;

    [Header("Position Based Variables")]
    public float positionDifferenceThreshold;
    float positionDifference;

    public float rotationDifferenceThreshold;
    float rotationDifference;

    [Header("Time Based Variables")]
    public float keyFrameTimeStep;

    int keyframeCount;
    GameObject keyframeHolder;

    float keyframeTime;

    public XmlKeyFrameManager manager;
    //These are only set when a keyframe is created
    Vector3 lastPos;
    Vector3 lastRot;

    public struct Recording
    {
        public Vector3 position;
        public Vector3 rotation;
        public float time;
    }

    void Start()
    {
        keyframeCount = 0;
        keyframeTime = 0;

        if (representKeyframesAsGameobjects)
        {
            keyframeHolder = new GameObject();
            keyframeHolder.name = gameObject.name + "_KeyframeHolder";
        }

        //Create and save keyframe for the initial position
        SaveKeyframe(CreateKeyframe());
    }

    void FixedUpdate()
    {
        if (record)
        {
            if (recordingType == RecordingType.positionBased)
            {
                PositionChecker();
            }
            else
            {
                TimeChecker();
            }
        }
    }

    void PositionChecker()
    {
        //Add together the differences from the last keyframes position and the current one
        positionDifference += Mathf.Abs(transform.position.x - lastPos.x);
        positionDifference += Mathf.Abs(transform.position.y - lastPos.y);
        positionDifference += Mathf.Abs(transform.position.z - lastPos.z);

        //Check to see if the position has changed significantly, if so then record a keyframe
        if (positionDifference > positionDifferenceThreshold)
        {
            //create and save recording
            SaveKeyframe(CreateKeyframe());
        }
        else
        {
            float x, y, z;
            float x2, y2, z2;
            x = transform.eulerAngles.x;
            y = transform.eulerAngles.y;
            z = transform.eulerAngles.z;
            x2 = lastRot.x;
            y2 = lastRot.y;
            z2 = lastRot.z;

            //Yeah... It's big brain time.
            YeahItsBigBrainTime(ref x);
            YeahItsBigBrainTime(ref y);
            YeahItsBigBrainTime(ref z);
            YeahItsBigBrainTime(ref x2);
            YeahItsBigBrainTime(ref y2);
            YeahItsBigBrainTime(ref z2);

            //Add together the differences from the last keyframes rotation and the current one
            rotationDifference += Mathf.Abs(x - x2);
            rotationDifference += Mathf.Abs(y - y2);
            rotationDifference += Mathf.Abs(z - z2);

            //Check to see if the rotation has changed significantly, if so then record a keyframe
            if (rotationDifference > rotationDifferenceThreshold)
            {
                //create and save recording
                SaveKeyframe(CreateKeyframe());
            }
        }

        //Reset
        positionDifference = 0;
        rotationDifference = 0;
    }

    void TimeChecker()
    {
        keyframeTime += Time.deltaTime;

        if(keyframeTime >= keyFrameTimeStep)
        {
            SaveKeyframe(CreateKeyframe());
            keyframeTime = 0;
        }
    }

    Recording CreateKeyframe()
    {
        Recording recording = new Recording
        {
            position = transform.position,
            rotation = transform.eulerAngles,
            time = Time.time
        };

        if (representKeyframesAsGameobjects)
        {
            //Create an object to represent the keyframes
            GameObject keyframe = Instantiate(keyframeObject, keyframeHolder.transform);
            keyframe.name = gameObject.name + "_Keyframe_" + keyframeCount.ToString();
            keyframe.transform.position = transform.position;
            keyframe.transform.rotation = transform.rotation;
        }

        //Increase keyframe count (only used for displaying the keyframes in the scene)
        keyframeCount++;

        //Set the new last position and rotation to the current one
        lastPos = transform.position;
        lastRot = transform.eulerAngles;

        return recording;
    }

    void YeahItsBigBrainTime(ref float num)
    {
        /*
         * Okay so if we kept the rotation as 0-360, then when the object goes from 0 to 360 it would be
         * counted as a huge difference instead of a small one as it should be. So to account for this,
         * we reformat rotation to just be from 0-180-0
         */
        if (num > 180)
        {
            num = 360 - num;
        }
    }

    public void SaveKeyframe(Recording recording)
    {
        manager.SaveOneClip(GetComponent<AnimeTable>().ID, recording);
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Hack to make sure our keyframes are gucci.
        if (collision.gameObject.tag == "Particle")
            CreateKeyframe();
    }
}

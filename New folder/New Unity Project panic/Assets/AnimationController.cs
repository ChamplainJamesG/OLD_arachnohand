using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public class KeyFrame
    {
        public Vector3 translation;
        public Vector3 rotation;
        public Vector3 scale;

        public List<ObjAndTransform> objTs = new List<ObjAndTransform>();

        public float keyTime;
    }

    public class ObjAndTransform
    {
        public GameObject objRef;
        public TestBlendBase.BlendTransform tForm;

        public ObjAndTransform()
        {
            objRef = null;
            tForm = new TestBlendBase.BlendTransform();
        }

        public ObjAndTransform(ObjAndTransform prev)
        {
            objRef = prev.objRef;
            tForm = prev.tForm;
        }

        public void SetPositionToTForm()
        {
            objRef.transform.localRotation = tForm.rotation;
            objRef.transform.localPosition = tForm.translation;
            objRef.transform.localScale = tForm.scale;
        }
    }

    public enum PLAY_CLIP
    {
        REVERSE = -1,
        STOP,
        FORWARD
    }

    /// <summary>
    /// Doesn't actually do anything yet.
    /// I'd like it to do something
    /// But it doesn't
    /// We could change that
    /// But why do something that takes time?
    /// I just want to work on Capstone
    /// ...
    /// Capstone? More like Napstone
    /// Naptsone? More like Hearthstone?
    /// Hearthstone? More like Blizzard is a 
    /// corportate entity that has no respect
    /// its fan base, and allies itself with dictators
    /// who commit genocide against groups of people.
    /// </summary>
    public class ClipController
    {
        public List<KeyFrame> keyFrames = new List<KeyFrame>();

        public PLAY_CLIP playDir;

        private float timeInFrame = 0.0f;
        private float normalizedTime = 0.0f;
        private int curKeyFrameIndex = 0;
        private int nextKeyFrameIndex = 1;

        public void UpdateClip(float dT)
        {
            if (keyFrames.Count <= 0)
                return;
            switch (playDir)
            {
                case PLAY_CLIP.FORWARD:
                    timeInFrame += dT;
                    while (timeInFrame > keyFrames[curKeyFrameIndex].keyTime)
                    {
                        timeInFrame -= keyFrames[curKeyFrameIndex].keyTime;
                        ++curKeyFrameIndex;
                        ++nextKeyFrameIndex;
                        if (nextKeyFrameIndex >= keyFrames.Count)
                            nextKeyFrameIndex = 0;
                        if (curKeyFrameIndex >= keyFrames.Count)
                            curKeyFrameIndex = 0;
                    }
                    normalizedTime = timeInFrame / keyFrames[curKeyFrameIndex].keyTime;
                    break;
            }
        }

        public List<ObjAndTransform> DoClip()
        {
            //Vector3 t = Vector3.Lerp(keyFrames[curKeyFrameIndex].translation, keyFrames[curKeyFrameIndex + 1].translation, normalizedTime);
            //Vector3 r = Vector3.Lerp(keyFrames[curKeyFrameIndex].rotation, keyFrames[curKeyFrameIndex + 1].rotation, normalizedTime);
            //Vector3 s = Vector3.Lerp(keyFrames[curKeyFrameIndex].scale, keyFrames[curKeyFrameIndex + 1].scale, normalizedTime);

            foreach(var bone in keyFrames[curKeyFrameIndex].objTs)
            {
                int curObjIndex = keyFrames[nextKeyFrameIndex].objTs.FindIndex(x => x.objRef == bone.objRef);
                bone.tForm.translation = Vector3.Lerp(bone.tForm.translation, keyFrames[nextKeyFrameIndex].objTs[curObjIndex].tForm.translation, normalizedTime);
                bone.tForm.rotation = Quaternion.Slerp(bone.tForm.rotation, keyFrames[nextKeyFrameIndex].objTs[curObjIndex].tForm.rotation, normalizedTime);
                bone.tForm.rotationEuler = Vector3.Lerp(bone.tForm.rotationEuler, keyFrames[nextKeyFrameIndex].objTs[curObjIndex].tForm.rotationEuler, normalizedTime);
                bone.tForm.scale = Vector3.Lerp(bone.tForm.scale, keyFrames[nextKeyFrameIndex].objTs[curObjIndex].tForm.scale, normalizedTime);

                bone.SetPositionToTForm();
            }
            // ^^ lerp every bone based on keyframe.
            // return the t r s of THIS BONE. Then we do the thing.

            return keyFrames[curKeyFrameIndex].objTs;
        }
    }
    ClipController cntrl = new ClipController();

    // Start is called before the first frame update
    void Start()
    {
        cntrl.playDir = PLAY_CLIP.FORWARD;
    }
    // Update is called once per frame
    void Update()
    {
    }

    public List<ObjAndTransform> DoClip(float dt)
    {
        cntrl.UpdateClip(dt);
        return cntrl.DoClip();
    }

    public void LoadKeyFrames(List<KeyFrame> frames)
    {
        cntrl.keyFrames = frames;
    }
}

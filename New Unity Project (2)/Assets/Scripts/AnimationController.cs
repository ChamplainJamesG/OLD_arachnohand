using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public class KeyFrame
    {
        //public List<int> boneIndeces;
        public List<KeyBone> bones = new List<KeyBone>();
        public float keyTime;
    }

    public class KeyBone
    {
        public Vector3 translation;
        public Quaternion rotation;
        public Vector3 scale;
        public int boneIndex;
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

        public List<BlendTransform> DoClip()
        {

            // TODO: THIS
            List<BlendTransform> bts = new List<BlendTransform>();
            for(int i = 0; i < keyFrames[curKeyFrameIndex].bones.Count; ++i)
            {
                var curBoneT = keyFrames[curKeyFrameIndex].bones[i];
                var nextBoneT = keyFrames[nextKeyFrameIndex].bones[i];

                var translation = Vector3.Lerp(curBoneT.translation, nextBoneT.translation, normalizedTime);
                var rot = Quaternion.Slerp(curBoneT.rotation, nextBoneT.rotation, normalizedTime);
                //Quaternion q = Quaternion.FromToRotation(Vector3.zero, rot);
                var scale = Vector3.Lerp(curBoneT.scale, nextBoneT.scale, normalizedTime);
                BlendTransform newT = new BlendTransform(translation, scale, rot);
                newT.boneIndex = keyFrames[curKeyFrameIndex].bones[i].boneIndex;
                newT.deltaPThisFrame = curBoneT.translation - nextBoneT.translation;
                newT.deltaRotThisFrame = nextBoneT.rotation * Quaternion.Inverse(curBoneT.rotation);
                newT.deltaScaleThisFrame = curBoneT.scale - nextBoneT.scale;
                bts.Add(newT);
            }
            return bts;
        }
    }
    public ClipController cntrl = new ClipController();

    // Start is called before the first frame update
    void Start()
    {
        cntrl.playDir = PLAY_CLIP.FORWARD;
    }
    // Update is called once per frame
    void Update()
    {
    }

    public List<BlendTransform> ControlUpdate(float dt)
    {
        cntrl.UpdateClip(dt);
        return cntrl.DoClip();
    }

    public void LoadKeyFrames(List<KeyFrame> frames)
    {
        cntrl.keyFrames = frames;
    }

    public void LoadKeyFrames(string fileName)
    {
        cntrl.keyFrames = KeyframeLoader.LoadKeyframe(fileName);
    }
}

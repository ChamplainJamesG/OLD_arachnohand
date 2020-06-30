using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject recordingUI;
    public GameObject recordingButton;
    public GameObject loadButton;
    public float borderAnimationSpeed;
    private Image[] images;
    private bool back;

    private void Start()
    {
        images = new Image[recordingUI.transform.childCount - 2];
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = recordingUI.transform.GetChild(i).GetComponent<Image>();
        }

        back = true;
    }

    private void Update()
    {
        if (recordingUI.activeSelf)
        {
            for (int i = 0; i < images.Length; i++)
            {
                if (back)
                {
                    images[i].color = Color.Lerp(images[i].color, new Color(1, 0, 0, 0), Time.deltaTime * borderAnimationSpeed);

                    if (images[i].color.a < 0.1f)
                    {
                        back = false;
                    }
                }
                else
                {
                    images[i].color = Color.Lerp(images[i].color, new Color(1, 0, 0, 1), Time.deltaTime * borderAnimationSpeed);

                    if (images[i].color.a > 0.9f)
                    {
                        back = true;
                    }
                }
            }
        }
    }

    public void ShowRecordingUI()
    {
        recordingUI.SetActive(true);
        recordingButton.SetActive(false);
        loadButton.SetActive(false);

        ToggleRecording(true);
    }

    public void StopRecording()
    {
        ToggleRecording(false);

        recordingUI.SetActive(false);
        loadButton.SetActive(true);

        GameObject.FindObjectOfType<XmlKeyFrameManager>().SaveClip();
    }

    void ToggleRecording(bool b)
    {
        KeyframeRecorder[] kr = GameObject.FindObjectsOfType<KeyframeRecorder>();
        for (int i = 0; i < kr.Length; i++)
        {
            kr[i].record = b;
        }
    }

    public void LoadAnimation()
    {
        GameObject.FindObjectOfType<KeyFrameController>().GetAllAnimatableObjects();
    }
}

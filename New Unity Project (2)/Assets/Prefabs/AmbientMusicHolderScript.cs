using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmbientMusicHolderScript : MonoBehaviour
{
    private static AmbientMusicHolderScript mInstance;
    public List<AudioClip> musicOptions;
    private AudioSource mSource;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (mInstance == null)
            mInstance = this;

        else
            DestroyImmediate(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        mSource = GetComponent<AudioSource>();
        PlaySong();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if(!mSource.isPlaying)
            PlaySong();
    }

    void PlaySong()
    {
        mSource.volume = 0.44f;
        mSource.clip = musicOptions[Random.Range(0, musicOptions.Count)];
        mSource.Play();
    }
}

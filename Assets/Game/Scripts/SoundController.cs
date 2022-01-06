using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    [Header("Audio Clip")]
    public AudioClip BGM;
    public AudioClip ButtonClip;
    public AudioClip CatchClip;
    public AudioClip DestroyClip;
    public AudioClip MatchEndClip;

    AudioSource audioSource;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        PlayBGM();

    }

    public void PlayBGM() {
        audioSource.clip = BGM;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopBGM() {
        audioSource.clip = BGM;
        audioSource.Stop();
    }

    public void SelectSFX(string _clipName) {
        switch (_clipName)
        {
            case "button" :
                PlaySFX(ButtonClip);
            break;
            case "catch" :
                PlaySFX(CatchClip);
            break;
            case "destroy" :
                PlaySFX(DestroyClip);
            break;
            case "matchend" :
                PlaySFX(MatchEndClip);
            break; 
        }
    }

    private void PlaySFX(AudioClip _clip) {
        audioSource.PlayOneShot(_clip);
    }
}

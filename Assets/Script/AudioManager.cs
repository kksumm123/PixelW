using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    void Awake()
    {
        instance = this;
    }

    GameObject audioSourceItem;
    AudioSource bgmAudioSource;
    void Start()
    {
        audioSourceItem = (GameObject)Resources.Load("AudioSourceItem");
        bgmAudioSource = GameObject.Find("SFXs/BGMPlayer").GetComponent<AudioSource>();
    }

    GameObject soundGo;
    AudioSource audioSource;
    public void GenerateAudioClip(AudioClip clip, float volume)
    {
        soundGo = ObjectPool.instance.SoundOP(audioSourceItem);
        audioSource = soundGo.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume * VolumeManager.instance.gSFXVolume;
        audioSource.Play();
    }
}

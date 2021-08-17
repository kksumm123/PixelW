using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager instance;
    void Awake()
    {
        instance = this;
    }

    AudioSource bgmAudioSource;
    float gBGMVolume = 1;
    float gSFXVolume = 1;
    float originBGMVolume;

    public float GBGMVolume
    {
        get => gBGMVolume;
        set
        {
            gBGMVolume = value;
            bgmAudioSource.volume = originBGMVolume * gBGMVolume;
        }
    }

    public float GSFXVolume { get => gSFXVolume; set => gSFXVolume = value; }

    void Start()
    {
        bgmAudioSource = GameObject.Find("BGMPlayer").GetComponent<AudioSource>();
        originBGMVolume = bgmAudioSource.volume;
    }
}

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
    AudioSource envSFXAudioSource;
    float gBGMVolume = 1;
    float gSFXVolume = 1;
    float originBGMVolume;
    float originEnvSFXVolume;

    public float GBGMVolume
    {
        get => gBGMVolume;
        set
        {
            gBGMVolume = value;
            bgmAudioSource.volume = originBGMVolume * gBGMVolume;
            envSFXAudioSource.volume = originEnvSFXVolume * gBGMVolume;
        }
    }

    public float GSFXVolume { get => gSFXVolume; set => gSFXVolume = value; }

    void Start()
    {
        bgmAudioSource = GameObject.Find("BGMPlayer").GetComponent<AudioSource>();
        envSFXAudioSource = GameObject.Find("EnvironmentSFX").GetComponent<AudioSource>();
        originBGMVolume = bgmAudioSource.volume;
        originEnvSFXVolume = envSFXAudioSource.volume;
    }
}

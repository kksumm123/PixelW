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
    AudioSource playerFootStepSource;
    AudioSource bgmAudioSource;
    AudioSource envSFXAudioSource;
    float gBGMVolume = 1;
    float gSFXVolume = 1;
    float originPlayerFootStepVolume;
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

    public float GSFXVolume 
    { 
        get => gSFXVolume;
        set
        {
            gSFXVolume = value;
            playerFootStepSource.volume = originPlayerFootStepVolume * gSFXVolume;
        }
    }

    void Start()
    {
        playerFootStepSource = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<AudioSource>();
        bgmAudioSource = GameObject.Find("BGMPlayer").GetComponent<AudioSource>();
        envSFXAudioSource = GameObject.Find("EnvironmentSFX").GetComponent<AudioSource>();
        originPlayerFootStepVolume = playerFootStepSource.volume;
        originBGMVolume = bgmAudioSource.volume;
        originEnvSFXVolume = envSFXAudioSource.volume;
    }
}

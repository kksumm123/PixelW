using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager instance;
    void Awake()
    {
        instance = this;
        StageManager.instance.DontDestroy(gameObject);
    }

    AudioSource bgmAudioSource;
    [Range(0, 1)]
    public float gSFXVolume = 1;
    float gBGMVolume = 1;
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

    void Start()
    {
        bgmAudioSource = GameObject.Find("BGMPlayer").GetComponent<AudioSource>();
        originBGMVolume = bgmAudioSource.volume;
    }
}

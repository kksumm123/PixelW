using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager instance;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(transform);
    }

    public float gSFXVolume = 1;
    public float gBGMVolume = 1;
}

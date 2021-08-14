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
    void Start()
    {
        audioSourceItem = (GameObject)Resources.Load("AudioSourceItem");
    }

    public void GenerateAudioClip(AudioClip clip, Transform parent)
    {
        ObjectPool.instance.InstantiateOP(audioSourceItem, parent);
    }
}

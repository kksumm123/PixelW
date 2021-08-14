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

    GameObject soundGo;
    AudioSource audioSource;
    public void GenerateAudioClip(AudioClip clip, Transform parent)
    {
        soundGo = ObjectPool.instance.SoundOP(audioSourceItem, parent);
        audioSource = soundGo.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }
}

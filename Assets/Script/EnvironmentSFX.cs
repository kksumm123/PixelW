using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSFX : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] bool isPlay = true;
    [SerializeField] float playMinDelay = 5;
    [SerializeField] float playMaxDelay = 15;
    [SerializeField] List<AudioClip> envSFXList;
    IEnumerator Start()
    {
        Debug.Assert(envSFXList.Count != 0, "환경음 리스트 설정해줘야함");
        StageManager.instance.DontDestroy(transform.root.gameObject);
        audioSource = GetComponent<AudioSource>();
        while (isPlay == true)
        {
            audioSource.clip = envSFXList[Random.Range(0, envSFXList.Count)];
            audioSource.Play();
            var playDelay = Random.Range(playMinDelay, playMaxDelay);
            yield return new WaitForSeconds(playDelay);
        }
    }
}
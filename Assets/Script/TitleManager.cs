using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    AudioSource audioSource;
    CanvasGroup canvasGroup;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvasGroup = GameObject.Find("Canvas").transform.Find("Title").GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            audioSource.Play();
            StartCoroutine(LoadingCo());
        }
    }

    IEnumerator LoadingCo()
    {
        // 비동기 로드
        var progress = SceneManager.LoadSceneAsync("Main");
        while (progress.isDone == false)
        {
            canvasGroup.alpha = 1 - progress.progress;
            yield return null;
        }
    }
}

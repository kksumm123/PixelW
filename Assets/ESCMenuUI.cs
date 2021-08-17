using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCMenuUI : MonoBehaviour
{
    GameObject child;
    Button reStartButton;
    Slider sliderBGM;
    Slider sliderSFXs;

    void Start()
    {
        child = transform.Find("child").gameObject;
        reStartButton = transform.Find("child/ReStartButton").GetComponent<Button>();
        reStartButton.onClick.AddListener(() =>
                {
                    Time.timeScale = 1;
                    SceneManager.LoadSceneAsync("Title");
                    StageManager.instance.ClearDontDestroy();
                });
        sliderBGM = transform.Find("child/BGM/Slider").GetComponent<Slider>();
        sliderSFXs = transform.Find("child/SFXs/Slider").GetComponent<Slider>();
        child.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            child.SetActive(!child.activeSelf);
            Time.timeScale = child.activeSelf == true ? 0 : 1;
        }

        VolumeBGM();
        VolumeSFXs();
    }

    void VolumeBGM()
    {
        VolumeManager.instance.GBGMVolume = sliderBGM.value;
    }
    void VolumeSFXs()
    {
        VolumeManager.instance.GSFXVolume = sliderSFXs.value;
    }
}
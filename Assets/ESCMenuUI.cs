using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCMenuUI : MonoBehaviour
{
    Button reStartButton;
    void Start()
    {
        reStartButton = transform.Find("ReStartButton").GetComponent<Button>();
        reStartButton.onClick.AddListener(() =>
                {
                    SceneManager.LoadSceneAsync("Title");
                    StageManager.instance.ClearDontDestroy();
                });
    }
}
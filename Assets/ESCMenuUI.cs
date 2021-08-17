using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ESCMenuUI : MonoBehaviour
{
    Button reStartButton;
    GameObject child;
    void Start()
    {
        reStartButton = transform.Find("child/ReStartButton").GetComponent<Button>();
        reStartButton.onClick.AddListener(() =>
                {
                    SceneManager.LoadSceneAsync("Title");
                    StageManager.instance.ClearDontDestroy();
                });
        child = transform.Find("child").gameObject;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            child.SetActive(!child.activeSelf);
        }
    }
}
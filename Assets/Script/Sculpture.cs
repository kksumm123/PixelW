using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sculpture : MonoBehaviour
{
    public static Sculpture instance;
    GameObject particleSystemGo;
    void Awake()
    {
        instance = this;
        this.enabled = false;
        particleSystemGo = transform.Find("ParticleSystem").gameObject;
    }

    float enableDistance = 1f;
    void Update()
    {
        if (Player.Instance)
        {
            if (Vector3.Distance(Player.Instance.transform.position, transform.position) < enableDistance)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    CenterNotifyUI.instance.ShowNotice("여신상에서 S 눌러따 ! 다음맵 넘어갈꺼다 !", 3);
                    this.enabled = false;
                    StartCoroutine(LoadNextSceneCo());
                }
            }
        }
    }

    IEnumerator LoadNextSceneCo()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadSceneAsync("NextScene");
        Player.Instance.transform.position = Vector3.zero;
    }

    public void EnableSculpture()
    {
        particleSystemGo.SetActive(true);
        this.enabled = true;
    }
}

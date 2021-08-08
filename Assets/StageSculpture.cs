using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSculpture : MonoBehaviour
{
    public static StageSculpture instance;
    GameObject particleSystemGo;
    void Awake()
    {
        isClear = false;
        instance = this;
        this.enabled = false;
        particleSystemGo = transform.Find("ParticleSystem").gameObject;
    }

    bool isClear = false;
    float enableDistance = 1f;
    void Update()
    {
        if (isClear == false)
        {
            if (Player.Instance)
            {
                if (Vector3.Distance(Player.Instance.transform.position, transform.position) < enableDistance)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        CenterNotifyUI.instance.ShowNotice("���Ż󿡼� S ������ ! ������ �Ѿ���� !", 3);
                        isClear = true;
                    }
                }
            }
        }
    }

    public void EnableSculpture()
    {
        particleSystemGo.SetActive(true);
        this.enabled = true;
    }
}

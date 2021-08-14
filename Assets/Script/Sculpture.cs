using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    CenterNotifyUI.instance.ShowNotice("���Ż󿡼� S ������ ! ������ �Ѿ���� !", 3);
                    this.enabled = false;
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

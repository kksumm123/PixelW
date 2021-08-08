using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSculpture : MonoBehaviour
{
    public static StageSculpture instance;
    void Awake()
    {
        instance = this;
        this.enabled = false;
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

                }
            }
        }
    }

    void EnableSculpture()
    {
        this.enabled = true;
    }
}

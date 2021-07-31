using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    public static GoldUI instance;
    void Awake()
    {
        instance = this;
    }

    Text goldValueText;
    Text goldAddValueText;
    void Start()
    {
        goldValueText = transform.Find("ValueText").GetComponent<Text>();
        goldAddValueText = transform.Find("AddValueText").GetComponent<Text>();
        goldAddValueText.text = "";
    }

    void Update()
    {
        if (Player.Instance)
        {
            goldValueText.text = Player.Instance.PlayersGold();
        }
    }
    Coroutine TextColorFadeCoHandle;
    public void AddValueText(int addValue)
    {
        goldAddValueText.DOKill();
        goldAddValueText.text = $" + {addValue} G";

        TextColorFadeCoHandle = StopAndStartCoroutine(TextColorFadeCoHandle, TextColorFadeCo());
    }

    float addValueShowTime = 1f;
    float addValueFadeTime = 1f;
    IEnumerator TextColorFadeCo()
    {
        var textColor = goldAddValueText.color;
        float endtime = Time.time + addValueShowTime;
        var fadeTime = 1 / addValueFadeTime;

        textColor.a = 1;
        goldAddValueText.color = textColor;
        while (Time.time < endtime)
        {
            textColor.a -= fadeTime * Time.deltaTime;
            goldAddValueText.color = textColor;
            yield return null;
        }
        textColor.a = 0;
        goldAddValueText.color = textColor;
    }

    Coroutine StopAndStartCoroutine(Coroutine handle, IEnumerator function)
    {
        if (handle != null)
            StopCoroutine(handle);
        return StartCoroutine(function);
    }
}

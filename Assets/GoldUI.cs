using DG.Tweening;
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

    float addValueShowTime = 1f;
    float addValueShowFadeTime = 1f;
    public void AddValueText(int addValue)
    {
        goldAddValueText.DOKill();
        goldAddValueText.DOFade(1, 0.001f);
        goldAddValueText.text = $" + {addValue} G";
        goldAddValueText.DOFade(0, addValueShowFadeTime).SetDelay(addValueShowTime);
    }
}

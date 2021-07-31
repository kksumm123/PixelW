using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    Image hpGauge;
    Text hpValueText;

    void Start()
    {
        hpGauge = transform.Find("HPBar/Gauge").GetComponent<Image>();
        hpValueText = transform.Find("HPBar/ValueText").GetComponent<Text>();
    }
    void Update()
    {
        if (Player.Instance)
        {
            hpGauge.fillAmount = Player.Instance.PlayersHPRate();
            hpValueText.text = Player.Instance.PlayersHpText();
        }
    }
}
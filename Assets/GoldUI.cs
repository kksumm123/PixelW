using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    Text goldValueText;
    Text goldAddValueText;
    void Start()
    {
        goldValueText = transform.Find("ValueText").GetComponent<Text>();
        goldAddValueText = transform.Find("AddValueText").GetComponent<Text>();
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
    void AddValueText(int addValue)
    {
        //goldAddValueText.color;
        goldAddValueText.text = addValue.ToString();
    }
}

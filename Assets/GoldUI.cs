using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    Text goldValueText;
    void Start()
    {
        goldValueText = transform.Find("ValueText").GetComponent<Text>();
    }

    void Update()
    {
        if (Player.Instance)
        {
            goldValueText.text = Player.Instance.PlayersGold();
        }
    }
}

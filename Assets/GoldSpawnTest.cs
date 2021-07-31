using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldSpawnTest : MonoBehaviour
{
    readonly string goldCoinString = "GoldCoin";
    GameObject coinGo;
    void Start()
    {
        coinGo = (GameObject)Resources.Load(goldCoinString);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Instantiate(coinGo, transform.position, transform.rotation);
    }
}

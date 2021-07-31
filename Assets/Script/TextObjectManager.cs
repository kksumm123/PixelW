using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextObjectManager : MonoBehaviour
{
    public static TextObjectManager instance;
    readonly string textObjectString = "TextObject";
    GameObject textObjectOnMemory;
    void Awake()
    {
        instance = this;
        textObjectOnMemory = (GameObject)Resources.Load(textObjectString);
    }

    float randPosValueX = 0.5f;
    float randPosValueY = 0.2f;
    public void NewTextObject(Transform tr, string text, Color color)
    {
        var randomPosValue = new Vector3(Random.Range(-randPosValueX, randPosValueX), Random.Range(-randPosValueY, randPosValueY), 0);
        var newGo = Instantiate(textObjectOnMemory, tr.position + randomPosValue, Quaternion.identity, tr);
        var newGoCS = newGo.GetComponent<TextObject>();
        StartCoroutine(newGoCS.SetText(text));
        StartCoroutine(newGoCS.SetColor(color));
    }
}

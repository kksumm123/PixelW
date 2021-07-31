using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void NewTextObject(Transform tr, string text, Color color)
    {
        var newGo = Instantiate(textObjectOnMemory, tr.position, Quaternion.identity, tr.parent);
        var newGoCS = newGo.GetComponent<TextObject>();
        StartCoroutine(newGoCS.SetText(text));
        StartCoroutine(newGoCS.SetColor(color));
    }
}

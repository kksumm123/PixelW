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

    void NewTextObject(Vector3 position, string text, Color color)
    {
        var newGo = Instantiate(textObjectOnMemory, position, Quaternion.identity);
        var newGoCS = newGo.GetComponent<TextObject>();
        newGoCS.SetText(text);
        newGoCS.SetColor(color);
    }
}

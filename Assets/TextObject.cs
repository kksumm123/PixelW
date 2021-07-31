using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextObject : MonoBehaviour
{
    new Renderer renderer;
    TextMeshPro textMeshPro;

    public int SortingOrder = 0;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (transform.parent)
        {
            var parentRenderer = transform.parent.GetComponent<Renderer>();
            if (parentRenderer)
            {
                renderer.sortingLayerID = parentRenderer.sortingLayerID;
                renderer.sortingOrder = parentRenderer.sortingOrder;
            }
        }
        else
            renderer.sortingOrder = SortingOrder;
    }

    public void SetText(string value)
    {
        textMeshPro.text = value;
    }
    public void SetColor(Color color)
    {
        textMeshPro.color = color;
    }
}
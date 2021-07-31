using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextObject : MonoBehaviour
{
    new Renderer renderer;
    TextMeshPro textMeshPro;

    public int SortingOrder = 0;
    IEnumerator Start()
    {
        renderer = GetComponent<Renderer>();
        textMeshPro = GetComponent<TextMeshPro>();
        Debug.Log("여기 만든다~");
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

        transform.DOLocalMoveY(1, 2);
        var textColor = textMeshPro.color;
        while (textMeshPro.color.a > 0.01f)
        {
            textColor = textMeshPro.color;
            textColor.a -= Time.deltaTime;
            textMeshPro.color = textColor;
            yield return null;
        }
        Destroy(gameObject, 2);
    }

    public IEnumerator SetText(string value)
    {
        while (textMeshPro == null)
            yield return null;
        textMeshPro.text = value;
    }
    public IEnumerator SetColor(Color color)
    {
        while (textMeshPro == null)
            yield return null;
        textMeshPro.color = color;
    }
}
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextObject : MonoBehaviour
{
    new Renderer renderer;
    TextMeshProUGUI textMeshPro;

    public int SortingOrder = 0;
    void Start()
    {
        renderer = GetComponent<Renderer>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
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
        textMeshPro.DOFade(0,1).SetDelay(1).OnComplete(() => Destroy(gameObject));
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
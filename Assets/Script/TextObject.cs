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
        if (transform.parent)
        {
            var parentRenderer = transform.parent.GetComponentInChildren<Renderer>();
            if (parentRenderer)
            {
                renderer.sortingLayerID = parentRenderer.sortingLayerID;
                renderer.sortingOrder = parentRenderer.sortingOrder;
            }
        }
        else
            renderer.sortingOrder = SortingOrder;

        transform.DOLocalMoveY(1, 2).SetLink(gameObject);
        var textColor = textMeshPro.color;

        yield return new WaitForSeconds(0.2f);
        while (textMeshPro.color.a > 0.01f)
        {
            textColor = textMeshPro.color;
            textColor.a -= Time.deltaTime;
            textMeshPro.color = textColor;
            yield return null;
        }
        Destroy(gameObject, 2);
    }
    void Update()
    {
        transform.rotation = Quaternion.identity;
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
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterNotifyUI : MonoBehaviour
{
    public static CenterNotifyUI instance;
    void Awake()
    {
        instance = this;
    }
    RectTransform rectTransform;
    Vector2 rectSizeDelta;
    CanvasGroup canvasGroup;
    Text centerNotifyText;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectSizeDelta = rectTransform.sizeDelta;
        canvasGroup = GetComponent<CanvasGroup>();
        centerNotifyText = transform.Find("CenterNotifyUI/Text").GetComponent<Text>();
    }

    void ShowNotice(string content, float visibleTime)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        rectSizeDelta.x = 0;
        rectTransform.DOScaleX(1, 1).SetEase(Ease.OutBounce);
        canvasGroup.DOFade(0, 1);

        centerNotifyText.text = content;
        canvasGroup.DOFade(0, 1)
                    .SetDelay(visibleTime)
                    .OnComplete(() => gameObject.SetActive(false));
    }
}

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
    CanvasGroup canvasGroup;
    Text centerNotifyText;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        centerNotifyText = transform.Find("Text").GetComponent<Text>();
        gameObject.SetActive(false);
    }

    public void ShowNotice(string content, float visibleTime)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        rectTransform.DOScaleX(0, 0);
        rectTransform.DOScaleX(1, 1).SetEase(Ease.OutBounce);
        canvasGroup.DOFade(1, 0.5f);

        centerNotifyText.text = content;
        canvasGroup.DOFade(0, 1)
                    .SetDelay(visibleTime)
                    .OnComplete(() => gameObject.SetActive(false));
    }
}

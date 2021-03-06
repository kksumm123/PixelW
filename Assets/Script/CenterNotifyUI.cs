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
        StageManager.instance.DontDestroy(transform.root.gameObject);
    }
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Text centerNotifyText;
    AudioSource audioSource;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        centerNotifyText = transform.Find("Text").GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        gameObject.SetActive(false);
    }

    public void ShowNotice(string content, float visibleTime = 3)
    {
        rectTransform.DOKill();
        canvasGroup.DOKill();
         
        gameObject.SetActive(true);
        audioSource.Play();

        canvasGroup.alpha = 0;
        rectTransform.DOScaleX(0, 0);
        rectTransform.DOScaleX(1, 1f)
                     .SetEase(Ease.OutBounce)
                     .SetLink(gameObject);
        canvasGroup.DOFade(1, 0.5f);

        centerNotifyText.text = content;
        canvasGroup.DOFade(0, 1)
                    .SetDelay(visibleTime)
                    .OnComplete(() => gameObject.SetActive(false))
                    .SetLink(gameObject);
    }
}

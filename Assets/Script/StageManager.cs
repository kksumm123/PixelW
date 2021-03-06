using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static List<GameObject> dontdestroys = new List<GameObject>();
    // 몬스터 마릿수 체크
    // 다 잡으면 안내 UI 띄워줌
    //   ㄴ CenterNotifyUI 만들어야함
    // 석상앞에서 S키로 다음 스테이지 이동
    public static StageManager instance;
    void Awake()
    {
        instance = this;
        DontDestroy(gameObject);
    }

    public void OnStageClear()
    {
        // CenterNotifyUI 호출 ("스테이지 클리어 !", 3초)
        CenterNotifyUI.instance.ShowNotice("몬스터 다 자바따 !\n여신상 앞에서 S키 !", 3);

        // 여신석상 활성화 (S누르면 맵 넘어갈 수 있도록 해야함)
        Sculpture.instance.EnableSculpture();
    }

    public void DontDestroy(GameObject _GameObject)
    {
        dontdestroys.Add(_GameObject);
        DontDestroyOnLoad(_GameObject);
    }

    public void ClearDontDestroy()
    {
        foreach (var item in dontdestroys)
        {
            if (item == gameObject)
                continue;
            Destroy(item);
        }
        Destroy(gameObject);
    }
}

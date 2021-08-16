using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // ���� ������ üũ
    // �� ������ �ȳ� UI �����
    //   �� CenterNotifyUI ��������
    // ����տ��� SŰ�� ���� �������� �̵�
    public static StageManager instance;
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(transform);
    }

    public void OnStageClear()
    {
        // CenterNotifyUI ȣ�� ("�������� Ŭ���� !", 3��)
        CenterNotifyUI.instance.ShowNotice("���� �� �ڹٵ� !\n�������� ������ !", 3);

        // ���ż��� Ȱ��ȭ (S������ �� �Ѿ �� �ֵ��� �ؾ���)
        Sculpture.instance.EnableSculpture();
    }

    public void TossShowNotice(string content, float visibleTime = 3)
    {
        CenterNotifyUI.instance.ShowNotice(content, visibleTime);
    }
}

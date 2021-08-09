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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CenterNotifyUI.instance.ShowNotice("�ѹ� ������ ���������� ���� ��", 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CenterNotifyUI.instance.ShowNotice("���� UI�� ������ ���� �ٽ� ȣ���� ��", 3);
        }


    }

    public void OnStageClear()
    {
        // CenterNotifyUI ȣ�� ("�������� Ŭ���� !", 3��)
        CenterNotifyUI.instance.ShowNotice("���� �� �ڹٵ� !\n�������� ������ !", 3);

        // ���ż��� Ȱ��ȭ (S������ �� �Ѿ �� �ֵ��� �ؾ���)
        Sculpture.instance.EnableSculpture();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // ������Ʈ Ǯ
    // ���� �뷮 8, �뷮�� ������ addedvalue��ŭ ����
    // ������ ������Ʈ�� ��ȿ����, ��ȿ�ð���ŭ Ȱ��ȭ, ���� ��Ȱ��ȭ
    //   ��> ��� �Ǵ��ұ�? ���ο��� üũ�غ���
    // ������Ʈ�� ������ �ڷᱸ���� �ϳ� �ʿ�

    public static ObjectPool instance;

    [SerializeField] int capacity = 8;
    [SerializeField] int addedCapaValue = 4;
    [SerializeField] int curGoCount;
    [SerializeField] int totalGoCount;
    [SerializeField] int validGoCount = 10;
    [SerializeField] float validGoTime = 5;
    [SerializeField] static List<GameObject> opGoList = new List<GameObject>();

    Coroutine validChkCoHandle;
    void Awake()
    {
        instance = this;
    }
    public GameObject SoundOP(GameObject original)
    {
        // ��Ȱ��ȭ�� ������Ʈ�� ������ �װ� ��������
        GameObject resultGo = null;
        bool isPopping = false;
        foreach (var item in opGoList)
        {
            if (item.activeSelf == false)
            {
                item.SetActive(true);
                item.transform.parent = null;
                isPopping = true;
                resultGo = item;
                break;
            }
        }
        if (isPopping == false)
        {
            resultGo = Instantiate(original);
            opGoList.Add(resultGo);
            totalGoCount = opGoList.Count;
            if (totalGoCount >= capacity)
                capacity += addedCapaValue;
        }

        curGoCount++;

        if (totalGoCount > validGoCount)
        {
            StopCo(validChkCoHandle);
            validChkCoHandle = StartCoroutine(validChkCo(totalGoCount));
        }
        return resultGo;
    }
    public void InstantiateOP(GameObject original, Vector3 position
                        , Quaternion rotation, Transform parent = null)
    {
        // ��Ȱ��ȭ�� ������Ʈ�� ������ �װ� ��������
        bool isPopping = false;
        foreach (var item in opGoList)
        {
            if (item.activeSelf == false)
            {
                item.SetActive(true);
                item.transform.parent = null;
                item.transform.position = position;
                item.transform.rotation = rotation;
                isPopping = true;
                break;
            }
        }
        if (isPopping == false)
        {
            GameObject newGo;
            if (parent)
                newGo = Instantiate(original, position, rotation, parent);
            else
                newGo = Instantiate(original, position, rotation);

            opGoList.Add(newGo);
            totalGoCount = opGoList.Count;
            if (totalGoCount >= capacity)
                capacity += addedCapaValue;
        }

        curGoCount++;

        if (totalGoCount > validGoCount)
        {
            StopCo(validChkCoHandle);
            validChkCoHandle = StartCoroutine(validChkCo(totalGoCount));
        }
    }

    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    private IEnumerator validChkCo(int objCount)
    {
        yield return new WaitForSeconds(validGoTime);
        for (int i = 0; i < objCount - validGoCount; i++)
        {
            opGoList[i].transform.parent = transform;
            opGoList[i].SetActive(false);
        }
        curGoCount = validGoCount;
        totalGoCount = objCount;
    }
}
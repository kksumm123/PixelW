using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // 오브젝트 풀
    // 최초 용량 8, 용량을 넘으면 addedvalue만큼 더함
    // 생성된 오브젝트는 유효개수, 유효시간만큼 활성화, 이후 비활성화
    //   ㄴ> 어디서 판단할까? 메인에서 체크해보자
    // 오브젝트를 관리할 자료구조가 하나 필요

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
        opGoList.Clear();
        StageManager.instance.DontDestroy(gameObject);
    }
    public GameObject SoundOP(GameObject original)
    {
        // 비활성화된 오브젝트가 있으면 그걸 꺼내오자
        GameObject resultGo = null;
        bool isPopping = false;
        foreach (var item in opGoList)
        {
            if (item.activeSelf == false)
            {
                item.SetActive(true);
                isPopping = true;
                resultGo = item;
                break;
            }
        }
        if (isPopping == false)
        {
            resultGo = Instantiate(original, transform);
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
        // 비활성화된 오브젝트가 있으면 그걸 꺼내오자
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

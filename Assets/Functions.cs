using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Functions : MonoBehaviour
{
    protected void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    /// <summary>
    /// 화면 흔들림
    /// </summary>
    /// <param name="time">미 입력시 기본값 0.1f</param>
    protected void WiggleScreen(float time = 0.1f)
    {
        MoveCam.Instance.WiggleScreen(time);
    }
}
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
}
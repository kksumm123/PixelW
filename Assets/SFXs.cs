using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXs : MonoBehaviour
{
    void Start()
    {
        StageManager.instance.DontDestroy(gameObject);
    }
}

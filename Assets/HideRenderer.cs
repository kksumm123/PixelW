using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRenderer : MonoBehaviour
{
    void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }
}

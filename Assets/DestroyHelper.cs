using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHelper : MonoBehaviour
{

    [SerializeField] float delay;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

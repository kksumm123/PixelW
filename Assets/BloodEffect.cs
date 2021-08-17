using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    [SerializeField] float randomRotationY = 45;
    [SerializeField] float destroyDelay = 0.6f;
    void Start()
    {
        Destroy(gameObject, destroyDelay);
        var eulerRotation = transform.rotation.eulerAngles;
        eulerRotation += new Vector3(0, 0, Random.Range(-randomRotationY, randomRotationY));
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}

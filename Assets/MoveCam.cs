using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    Transform playerTr;
    Transform tr;

    [SerializeField] float speed = 2f;
    void Start()
    {
        playerTr = GameObject.Find("Player").GetComponent<Transform>();
        tr = transform;
    }

    void Update()
    {
        var distance = new Vector3(playerTr.position.x - tr.position.x, 0, 0);
        transform.Translate(distance * speed * Time.deltaTime, Space.World);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField] float speed = 5f;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float moveX = 0;
        if (Input.GetKey(KeyCode.A))
            moveX = -1;
        if (Input.GetKey(KeyCode.D))
            moveX = 1;

        if (moveX != 0)
        {
            var pos = transform.position;
            pos.x += moveX * speed * Time.deltaTime;
            transform.position = pos;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 900f;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }


    private void Move()
    {
        float moveX = 0;
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1;
            transform.rotation = new Quaternion(0, 180, 0, 0);

        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1;
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        if (moveX != 0)
        {
            var pos = transform.position;
            pos.x += moveX * speed * Time.deltaTime;
            transform.position = pos;
        }
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rigid.AddForce(new Vector2(0, jumpForce));
        }
    }
}

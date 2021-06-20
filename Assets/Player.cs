using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    BoxCollider2D boxCol2D;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 900f;

    StateType state;
    [SerializeField]
    StateType State
    {
        get { return state; }
        set { state = value; }
    }
    enum StateType
    {
        Ground,
        Jump,
        Fall
    }
    void StateUpdate()
    {
        if (IsGound())
            State = StateType.Ground;
        if (rigid.velocity.y < 0)
            State = StateType.Fall;
    }

    [SerializeField] float groundRayOffset = 0;
    [SerializeField] float groundRayLength = 0.9f;
    [SerializeField] LayerMask groundLayer;
    private bool IsGound()
    {
        var pos = transform.position;
        if (ChkRay(pos, Vector2.down, groundRayLength, groundLayer))
            return true;
        if (ChkRay(pos - new Vector3(groundRayOffset, 0, 0)
            , Vector2.down, groundRayLength, groundLayer))
            return true;
        if (ChkRay(pos + new Vector3(groundRayOffset, 0, 0)
            , Vector2.down, groundRayLength, groundLayer))
            return true;
        return false;
    }

    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        if (hit.transform != null)
            return true;
        return false;
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        groundRayOffset = boxCol2D.size.x / 2;
    }

    void Update()
    {
        StateUpdate();
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
        if (State == StateType.Ground)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                rigid.AddForce(new Vector2(0, jumpForce));
                State = StateType.Jump;
            }
        }
    }
}

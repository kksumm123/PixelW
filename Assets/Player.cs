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
    [SerializeField] StateType state;

    #region StateUpdate
    bool isUpdatePhysics = false;
    private void SetGroundRaySetting()
    {
        groundRayOffsetX = boxCol2D.size.x / 2;
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
    }
    void StateUpdate()
    {
        if (isUpdatePhysics == false)
            return;
        var velo = rigid.velocity;

        if (velo.y == 0 && IsGound())
            State = StateType.Ground;
        if (velo.y < 0)
            State = StateType.Fall;
    }

    [SerializeField] float groundRayOffsetX = 0;
    [SerializeField] float groundRayOffsetY = 0.2f;
    [SerializeField] float groundRayLength = 0.2f;
    [SerializeField] LayerMask groundLayer;
    private bool IsGound()
    {
        var pos = transform.position;
        if (ChkRay(pos + new Vector3(0, groundRayOffsetY, 0)
            , Vector2.down, groundRayLength, groundLayer))
            return true;
        if (ChkRay(pos + new Vector3(-groundRayOffsetX, groundRayOffsetY, 0)
            , Vector2.down, groundRayLength, groundLayer))
            return true;
        if (ChkRay(pos + new Vector3(groundRayOffsetX, groundRayOffsetY, 0)
            , Vector2.down, groundRayLength, groundLayer))
            return true;
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position
            + new Vector3(0, groundRayOffsetY, 0)
            , Vector2.down * groundRayLength);
        Gizmos.DrawRay(transform.position
            + new Vector3(-groundRayOffsetX, groundRayOffsetY, 0)
            , Vector2.down * groundRayLength);
        Gizmos.DrawRay(transform.position
            + new Vector3(groundRayOffsetX, groundRayOffsetY, 0)
            , Vector2.down * groundRayLength);
    }

    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }
    #endregion

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        boxCol2D = GetComponent<BoxCollider2D>();
        SetGroundRaySetting();
    }

    void FixedUpdate()
    {
        isUpdatePhysics = true;
    }
    void Update()
    {
        StateUpdate();
        Move();
        Jump();
    }
    #region Move
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
    #endregion

    #region Jump
    private void Jump()
    {
        if (State == StateType.Ground)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                isUpdatePhysics = false;
                State = StateType.Jump;
                rigid.AddForce(new Vector2(0, jumpForce));
            }
        }
    }
    #endregion

    #region StateType
    StateType State
    {
        get { return state; }
        set
        {
            if (state != StateType.Ground && value != StateType.Ground)
                Debug.Log($"{state} -> {value}");
            state = value;
        }
    }
    enum StateType
    {
        Ground,
        Jump,
        Fall
    }
    #endregion
}

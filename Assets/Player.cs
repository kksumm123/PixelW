using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Transform tr;
    Rigidbody2D rigid;
    BoxCollider2D boxCol2D;
    Animator animator;

    [SerializeField] float speed = 5f;
    [SerializeField] float jumpForce = 900f;
    [SerializeField] StateType state;
    [SerializeField] AnimType anim = AnimType.Idle;


    #region About Ray
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
        tr = transform;
        rigid = GetComponent<Rigidbody2D>();
        boxCol2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        SetGroundRaySetting();

        void SetGroundRaySetting()
        {
            slideRayOffsetX = boxCol2D.size.x / 2;
            slideRayOffsetY = boxCol2D.size.y / 2;
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
        }
    }

    void FixedUpdate()
    {
        isUpdatePhysics = true;
    }

    bool isUpdatePhysics = false;

    void Update()
    {
        StateUpdate();
        AnimForState();
        AnimationPlay();
        Move();
        Jump();
    }

    #region StateUpdate
    void StateUpdate()
    {
        if (isUpdatePhysics == false)
            return;

        var velo = rigid.velocity;

        if (velo.y == 0 && ChkGound())
            State = StateType.Ground;
        if (State != StateType.Jump && State != StateType.Fall
            && State != StateType.WallSlide && moveX != 0)
            State = StateType.Run;
        if (velo.y < 0)
            State = StateType.Fall;
        if (ChkWall())
            State = StateType.WallSlide;
    }
    #region Ground
    [SerializeField] float groundRayOffsetX = 0.2f;
    [SerializeField] float groundRayOffsetY = 0.2f;
    [SerializeField] float groundRayLength = 0.2f;
    [SerializeField] LayerMask groundLayer;
    private bool ChkGound()
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
    #endregion Ground
    #region WallSlide
    [SerializeField] float slideRayOffsetX = 0;
    [SerializeField] float slideRayOffsetY = 0;
    [SerializeField] float slideRayLength = 0.01f;
    private bool ChkWall()
    {
        if (State == StateType.Jump || State == StateType.Fall)
        {
            if (ChkRay(tr.position + new Vector3(-slideRayOffsetX, slideRayOffsetY, 0)
            , Vector2.left, slideRayLength, groundLayer))
                return true;
            else if (ChkRay(tr.position + new Vector3(slideRayOffsetX, slideRayOffsetY, 0)
                , Vector2.right, slideRayLength, groundLayer))
                return true;
        }
        return false;
    }
    #endregion WallSlide

    #endregion

    #region AnimForState
    private void AnimForState()
    {
        switch (State)
        {
            case StateType.Ground:
                Anim = AnimType.Idle;
                break;
            case StateType.Jump:
                Anim = AnimType.Jump;
                break;
            case StateType.Fall:
                Anim = AnimType.Fall;
                break;
            case StateType.Run:
                Anim = AnimType.Run;
                break;
            case StateType.WallSlide:
                Anim = AnimType.WallSlide;
                break;
        }
    }
    #endregion

    #region AnimationPlay
    void AnimationPlay()
    {
        switch (Anim)
        {
            case AnimType.Idle:
                animator.Play("Idle");
                break;
            case AnimType.Run:
                animator.Play("Run");
                break;
            case AnimType.Jump:
                animator.Play("Jump");
                break;
            case AnimType.Fall:
                animator.Play("Fall");
                break;
            case AnimType.WallSlide:
                animator.Play("WallSlide");
                break;
        }
    }
    #endregion

    #region Move
    float moveX = 0;
    private void Move()
    {
        if (State == StateType.WallSlide)
            return;

        moveX = 0;
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
        if (State == StateType.Ground || State == StateType.Run)
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
            if (state == value)
                return;
            Debug.Log($"{state} -> {value}");
            state = value;
        }
    }
    enum StateType
    {
        Ground,
        Jump,
        Fall,
        Run,
        WallSlide,
    }
    #endregion
    #region AnimationType
    AnimType Anim
    {
        get { return anim; }
        set
        {
            if (anim == value)
                return;
            Debug.Log($"{anim} -> {value}");
            anim = value;
        }
    }
    enum AnimType
    {
        Idle,
        Run,
        Fall,
        Jump,
        WallSlide
    }
    #endregion
}

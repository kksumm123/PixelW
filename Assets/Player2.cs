using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    Transform tr;
    Rigidbody2D rigid;
    BoxCollider2D boxCol2D;
    Animator animator;

    [SerializeField] Vector3 fowward;
    [SerializeField] Vector3 velocity;
    [SerializeField] float speed = 5f;
    [SerializeField] float slideJumpForceX = 250f;
    [SerializeField] float slideJumpForceY = 900f;
    [SerializeField] float jumpForce = 900f;
    [SerializeField] StateType state;
    [SerializeField] AnimType anim = AnimType.Idle;

    #region AboutRay
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

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position
            + new Vector3(-slideRayOffsetX, slideRayOffsetY, 0)
            , Vector2.left * slideRayLength);
        Gizmos.DrawRay(transform.position
            + new Vector3(slideRayOffsetX, slideRayOffsetY, 0)
                 , Vector2.right * slideRayLength);
    }

    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }
    #endregion AboutRay
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

    [SerializeField] bool isUpdatePhysics = false;

    void Update()
    {
        fowward = transform.forward;
        velocity = rigid.velocity;
        StateUpdate();
        AnimForState();
        AnimationPlay();

        Move();
        Rolling();
        Jump();
        Attack();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ChkGound())
            {
                Anim = AnimType.Attack1;
            }
        }
    }


    #region StateUpdate
    void StateUpdate()
    {
        if (isUpdatePhysics == false)
            return;
        else if (isRolling == true)
            return;

        var velo = rigid.velocity;

        if (ChkGound())
        {
            State = StateType.Ground;
            rigid.velocity = Vector2.zero;
        }

        if (State != StateType.Jump && State != StateType.Fall
            && State != StateType.WallSlide && moveX != 0)
            State = StateType.Run;

        if (velo.y < 0)
            State = StateType.Fall;

        if (velo.y > 0)
            State = StateType.Jump;

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
        var pos = tr.position;
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
            if (transform.forward.z == 1)
            {
                if (ChkRay(tr.position + new Vector3(slideRayOffsetX, slideRayOffsetY, 0)
                    , Vector2.right, slideRayLength, groundLayer))
                    return true;
            }
            else
            {
                if (ChkRay(tr.position + new Vector3(-slideRayOffsetX, slideRayOffsetY, 0)
                , Vector2.left, slideRayLength, groundLayer))
                    return true;
            }
        }
        return false;
    }
    #endregion WallSlide

    #endregion StateUpdate

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
            case StateType.Roll:
                Anim = AnimType.Roll;
                break;
        }
    }
    #endregion AnimForState

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
            case AnimType.Roll:
                animator.Play("Roll");
                break;
            case AnimType.Attack1:
                animator.Play("Attack1");
                break;
            case AnimType.Attack2:
                animator.Play("Attack2");
                break;
            case AnimType.Attack3:
                animator.Play("Attack3");
                break;
        }
    }
    #endregion AnimationPlay

    #region Move
    float moveX = 0;
    private void Move()
    {
        if (State == StateType.WallSlide)
            return;
        else if (Mathf.Abs(velocity.x) > 4.9)
            return;
        else if (isRolling == true)
            return;

        moveX = 0;
        if (Input.GetKey(KeyCode.A))
            moveX = -1;
        if (Input.GetKey(KeyCode.D))
            moveX = 1;

        if (moveX != 0)
        {
            transform.rotation = new Quaternion(0, moveX == -1 ? 180 : 0, 0, 0);

            var pos = tr.position;
            pos.x += moveX * speed * Time.deltaTime;
            tr.position = pos;
        }
    }
    #endregion Move

    #region Rolling
    [SerializeField] bool isRolling = false;
    [SerializeField] float rollTime = 0.7f;
    private void Rolling()
    {
        if (isRolling)
        {
            var pos = tr.position;
            pos.x += transform.forward.z * speed * Time.deltaTime;
            tr.position = pos;
        }
        else if (ChkGound() && Input.GetKey(KeyCode.LeftShift))
        {
            State = StateType.Roll;
            StartCoroutine(IsRollingCo());
        }
    }

    private IEnumerator IsRollingCo()
    {
        isRolling = true;
        yield return new WaitForSeconds(rollTime);
        isRolling = false;
    }
    #endregion Rolling

    #region Jump
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Normal Jump
            if (ChkGound())
            {
                isUpdatePhysics = false;
                State = StateType.Jump;
                rigid.AddForce(new Vector2(0, jumpForce));
            }
            // Slide Jump
            else if (State == StateType.WallSlide)
            {
                isUpdatePhysics = false;
                State = StateType.Jump;

                rigid.velocity = Vector2.zero;

                var forZ = transform.forward.z;
                rigid.AddForce(
                    new Vector2(slideJumpForceX * forZ * -1, slideJumpForceY));
                transform.rotation =
                    new Quaternion(0, transform.rotation.y == 0 ? 180 : 0, 0, 0);
            }
        }
    }
    #endregion Jump


    #region StateType
    StateType State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;
            Debug.Log($"StateType : {state} -> {value}");
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
        Roll,
    }
    #endregion StateType
    #region AnimationType
    AnimType Anim
    {
        get { return anim; }
        set
        {
            if (anim == value)
                return;
            Debug.Log($"AnimType : {anim} -> {value}");
            anim = value;
        }
    }
    enum AnimType
    {
        Idle,
        Run,
        Fall,
        Jump,
        WallSlide,
        Roll,
        Attack1,
        Attack2,
        Attack3,
    }
    #endregion AnimationType
}

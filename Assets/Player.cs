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

    [SerializeField] Vector3 trForward;
    [SerializeField] Vector3 velocity;
    [SerializeField] StateType state;
    [SerializeField] AnimType anim = AnimType.Idle;
    [SerializeField] float speed = 5f;


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
    float originSpeed;
    void Start()
    {
        tr = transform;
        rigid = GetComponent<Rigidbody2D>();
        boxCol2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        SetGroundRaySetting();
        originSpeed = speed;

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
        trForward = transform.forward;
        velocity = rigid.velocity;
        StateUpdate();
        AnimForState();
        //AnimationPlay();

        Move();
        Rolling();
        Jump();
        Attack();
        Block();
    }

    void Block()
    {
        if (ChkIdle() || ChkBlocking())
        {
            if (Input.GetMouseButton(1))
            {
                State = StateType.IdleBlock;
            }
            else
                State = StateType.Ground;
        }
    }

    #region StateUpdate
    void StateUpdate()
    {
        if (isUpdatePhysics == false)
            return;
        else if (isRolling == true)
            return;

        if (ChkAttackAndBlock() == false)
        {
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
    }
    #region ChkAttackAndBlock
    private bool ChkAttackAndBlock()
    {
        switch (State)
        {
            case StateType.Attack1:
            case StateType.Attack2:
            case StateType.Attack3:
            case StateType.IdleBlock:
            case StateType.Block:
                return true;
        }
        return false;
    }
    #endregion ChkAttackAndBlock
    #region ChkIdle
    private bool ChkIdle()
    {
        switch (State)
        {
            case StateType.Ground:
            case StateType.Run:
                return true;
        }
        return false;
    }
    #endregion ChkIdle
    #region ChkBlocking
    private bool ChkBlocking()
    {
        switch (State)
        {
            case StateType.IdleBlock:
            case StateType.Block:
                return true;
        }
        return false;
    }
    #endregion ChkBlocking
    #region ChkGound
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
    #endregion ChkGound
    #region ChkWall
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
    #endregion ChkWall

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
            case StateType.Attack1:
                Anim = AnimType.Attack1;
                break;
            case StateType.Attack2:
                Anim = AnimType.Attack2;
                break;
            case StateType.Attack3:
                Anim = AnimType.Attack3;
                break;
            case StateType.IdleBlock:
                Anim = AnimType.IdleBlock;
                break;
            case StateType.Block:
                Anim = AnimType.Block;
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
            case AnimType.IdleBlock:
                animator.Play("IdleBlock");
                break;
            case AnimType.Block:
                animator.Play("Block");
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
    [SerializeField] float rollTime = 0.6f;
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
    [SerializeField] float jumpForce = 900f;
    [SerializeField] float slideJumpForceX = 250f;
    [SerializeField] float slideJumpForceY = 900f;
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

    #region Attack
    [SerializeField]
    List<float> attackDelay =
        new List<float>() { 0.43f, 0.43f, 0.57f };
    [SerializeField] float attackCurDelay = 0;
    [SerializeField] int attackIdx = 0;
    [SerializeField] int attackMaxIdx = 2; // = 3 (0, 1, 2)
    [SerializeField] float attackIdxResetTime = 1.2f;
    [SerializeField] float attackIdxResetCurTime = 0f;
    Coroutine attackCoHandle;
    Coroutine attackDelayCoHandle;
    Coroutine attackIndxResetCoHandle;

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ChkGound() && attackCurDelay <= 0)
            {
                if (attackIdx <= attackMaxIdx)
                {
                    attackCurDelay = attackDelay[attackIdx];
                    attackIdxResetCurTime = attackIdxResetTime;
                    switch (attackIdx)
                    {
                        case 0:
                            State = StateType.Attack1;
                            break;
                        case 1:
                            State = StateType.Attack2;
                            break;
                        case 2:
                            State = StateType.Attack3;
                            break;

                    }
                    StopCo(attackCoHandle);
                    StopCo(attackDelayCoHandle);
                    StopCo(attackIndxResetCoHandle);
                    attackCoHandle = StartCoroutine(AttackCo(attackCurDelay));
                    attackDelayCoHandle = StartCoroutine(AttackDelayCo());
                    attackIndxResetCoHandle = StartCoroutine(AttackIndxResetCo());
                    attackIdx++;
                }
                else
                    attackIdx = 0;
            }
        }
    }

    IEnumerator AttackCo(float attackCurDelay)
    {
        speed = 1;
        yield return new WaitForSeconds(attackCurDelay);
        State = StateType.AttackExit;
        speed = originSpeed;
    }
    IEnumerator AttackDelayCo()
    {
        while (attackCurDelay > 0)
        {
            attackCurDelay -= Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AttackIndxResetCo()
    {
        while (attackIdxResetCurTime > 0)
        {
            attackIdxResetCurTime -= Time.deltaTime;
            yield return null;
        }
        attackIdx = 0;
    }
    #endregion Attack

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
        Attack1,
        Attack2,
        Attack3,
        AttackExit,
        IdleBlock,
        Block,

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
            AnimationPlay();
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
        IdleBlock,
        Block,

    }
    #endregion AnimationType
    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
}

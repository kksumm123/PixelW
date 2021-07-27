using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Declare
    static Player m_instance = null;
    public static Player Instance { get => m_instance; }
    private void Awake()
    {
        m_instance = this;
    }

    Transform tr;
    Rigidbody2D rigid;
    BoxCollider2D boxCol2D;
    Animator animator;
    Transform blockFlashTr;
    GameObject blockFlashEffectGo;
    string blockFlashEffectString = "BlockFlashEffect";

    [SerializeField] Vector3 trForward;
    [SerializeField] Vector3 velocity;
    [SerializeField] StateType state;
    [SerializeField] AnimType anim = AnimType.Idle;
    [SerializeField] int hp = 50;
    [SerializeField] int damage = 5;
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float battleSpeed = 0.5f;
    #endregion Declare

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

    #region Start()
    float originSpeed;
    void Start()
    {
        tr = transform;
        rigid = GetComponentInChildren<Rigidbody2D>();
        boxCol2D = GetComponentInChildren<BoxCollider2D>();
        animator = GetComponentInChildren<Animator>();
        blockFlashTr = transform.Find("BlockFlashPosition");
        blockFlashEffectGo = (GameObject)Resources.Load(blockFlashEffectString);
        attackBoxTr = transform.Find("AttackBox");
        enemyLayer = 1 << LayerMask.NameToLayer("Monster");
        SetGroundRaySetting();
        originSpeed = normalSpeed;

        void SetGroundRaySetting()
        {
            slideRayOffsetX = boxCol2D.size.x / 2;
            slideRayOffsetY = boxCol2D.size.y / 2;
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
        }
    }
    #endregion Start()

    #region Update()
    [SerializeField] bool isUpdatePhysics = false;
    void FixedUpdate()
    {
        isUpdatePhysics = true;
    }

    void Update()
    {
        trForward = transform.forward;
        velocity = rigid.velocity;
        StateUpdate();
        AnimForState();

        if (State != StateType.Hit && State != StateType.Death)
        {
            Move();
            Rolling();
            Jump();
            Attack();
            Block();
        }
    }
    #endregion Update()

    #region StateUpdate
    void StateUpdate()
    {
        if (isUpdatePhysics == false)
            return;
        else if (isRolling == true)
            return;

        if (State != StateType.Hit && ChkBattle() == false)
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
    private bool ChkBattle()
    {
        switch (State)
        {
            case StateType.Attack1:
            case StateType.Attack2:
            case StateType.Attack3:
            case StateType.IdleBlock:
            case StateType.Block:
            case StateType.Hit:
            case StateType.Death:
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
            if (ChkBlocking() == false)
                transform.rotation = new Quaternion(0, moveX == -1 ? 180 : 0, 0, 0);

            var pos = tr.position;
            pos.x += moveX * normalSpeed * Time.deltaTime;
            tr.position = pos;
        }
    }
    #endregion Move

    #region Rolling
    [SerializeField] bool isRolling = false;
    [SerializeField] float rollTime = 0.6f;
    bool isInvincibility = false;
    private void Rolling()
    {
        if (isRolling)
        {
            var pos = tr.position;
            pos.x += transform.forward.z * normalSpeed * Time.deltaTime;
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
        isInvincibility = true;
        yield return new WaitForSeconds(rollTime);
        isRolling = false;
        isInvincibility = false;
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
    [SerializeField] float attackApplyDelay = 0.15f;
    [SerializeField]
    List<float> attackDelay =
        new List<float>() { 0.43f, 0.43f, 0.57f };
    [SerializeField] float attackCurDelay = 0;
    [SerializeField] int attackIdx = 0;
    [SerializeField] int attackMaxIdx = 3;
    [SerializeField] float attackIdxResetTime = 1.2f;
    [SerializeField] float attackIdxResetCurTime = 0f;
    [SerializeField] GameObject attackBoxObj;
    Coroutine attackCoHandle;
    Coroutine attackDelayCoHandle;
    Coroutine attackIndxResetCoHandle;
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ChkGound() && attackCurDelay <= 0)
            {
                if (attackIdx < attackMaxIdx)
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
    Transform attackBoxTr;
    [SerializeField] Vector2 attackBoxSize = new Vector2(1.9f, 1.5f);
    LayerMask enemyLayer;
    Collider2D[] attackedEnemies;
    IEnumerator AttackCo(float delay)
    {
        normalSpeed = battleSpeed;
        yield return new WaitForSeconds(attackApplyDelay);
        attackedEnemies = null;
        // 실제 공격 적용
        var point = new Vector2(
            attackBoxTr.transform.position.x
            , attackBoxTr.transform.position.y);
        attackedEnemies =
            Physics2D.OverlapBoxAll(point, attackBoxSize, 90, enemyLayer);
        foreach (var item in attackedEnemies)
            item.GetComponent<NewMonster>().TakeHit(damage);

        yield return new WaitForSeconds(delay);
        State = StateType.AttackAndHitExit;
        normalSpeed = originSpeed;
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

    #region Block
    float parryingTime = 0.3f;
    bool isParrying = false;
    Coroutine parrayingCoHendle;
    void Block()
    {
        if (ChkIdle() || ChkBlocking())
        {
            if (Input.GetMouseButtonDown(1))
            {
                State = StateType.IdleBlock;
                normalSpeed = battleSpeed;
                StopCo(parrayingCoHendle);
                parrayingCoHendle = StartCoroutine(ParryingCo());
            }
            else if (Input.GetMouseButton(1) == false)
            {
                StopCo(parrayingCoHendle);
                isParrying = false;
                State = StateType.Ground;
                normalSpeed = originSpeed;
            }
        }
    }

    IEnumerator ParryingCo()
    {
        isParrying = true;
        yield return new WaitForSeconds(parryingTime);
        isParrying = false;
    }
    #endregion Block


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
            case StateType.Hit:
                Anim = AnimType.Hit;
                break;
            case StateType.Death:
                Anim = AnimType.Death;
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
            case AnimType.Hit:
                animator.Play("Hit");
                break;
            case AnimType.Death:
                animator.Play("Death");
                break;
        }
    }
    #endregion AnimationPlay
    #region StateType
    StateType State
    {
        get => state;
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
        AttackAndHitExit,
        IdleBlock,
        Block,
        Hit,
        Death,
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
        Hit,
        Death,
    }
    #endregion AnimationType
    #region TakeHit
    public void TakeHit(int damage, Transform monsterTr)
    {
        if (hp > 0)
        {
            if (FrontBlock(monsterTr) == true && isParrying == true)
                Instantiate(blockFlashEffectGo, blockFlashTr.position, transform.rotation);
            else if (isInvincibility == true)
                return;
            else
            {
                hp -= damage;
                StartCoroutine(HitCo());
            }
        }
    }


    private bool FrontBlock(Transform monsterTr)
    {// true = Parrying, false = Fail Parrinying
        var distanceX = monsterTr.position.x - transform.position.x;

        return IsZero(transform.rotation.eulerAngles.y)
            ? IsPositive(distanceX) : IsNegative(distanceX);

        bool IsZero(float value)
        {
            return value == 0;
        }
        bool IsPositive(float value)
        {
            return value > 0;
        }
        bool IsNegative(float value)
        {
            return value < 0;
        }
    }

    [SerializeField] float hitAnimationLenth = 0.273f;
    IEnumerator HitCo()
    {
        State = StateType.Hit;
        yield return new WaitForSeconds(hitAnimationLenth);
        if (hp > 0)
            State = StateType.AttackAndHitExit;
        else
        {
            State = StateType.Death;
            StartCoroutine(DeathCo());
        }
    }

    [SerializeField] float deathTime = 1;
    IEnumerator DeathCo()
    {
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
    #endregion TakeHit

    #region Methods
    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    void OnDestroy()
    {
        m_instance = null;
    }
    #endregion Methods
}

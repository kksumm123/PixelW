using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator animator;
    Transform playerTr;
    Transform tr;
    [SerializeField] StateType state;
    [SerializeField] AnimType anim = AnimType.Idle;

    [SerializeField] float hitAnimationLenth = 0.4f;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        tr = transform;
        attackBoxObj = transform.Find("AttackBoxObj").gameObject;
    }
    void Update()
    {
        AnimForState();
        Walk();
        Attack();
    }

    #region Attack
    [SerializeField]
    List<float> attackDelay =
        new List<float>() { 0.667f, 0.667f };
    [SerializeField] float attackReadyMotionDelay = 0.5f;
    [SerializeField] float attackCurDelay = 0;
    [SerializeField] int attackIdx = 0;
    [SerializeField] int attackMaxIdx = 2;
    [SerializeField] float attackIdxResetTime = 1.2f;
    [SerializeField] float attackIdxResetCurTime = 0f;
    [SerializeField] GameObject attackBoxObj;
    Coroutine attackCoHandle;
    Coroutine attackDelayCoHandle;
    Coroutine attackIndxResetCoHandle;
    private void Attack()
    {
        if (State != StateType.Hit && ChkAttack())
        {
            if (attackCurDelay <= 0)
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
                    }
                    StopCo(attackCoHandle);
                    StopCo(attackDelayCoHandle);
                    StopCo(attackIndxResetCoHandle);
                    attackCoHandle = 
                        StartCoroutine(
                            AttackCo(attackCurDelay - attackReadyMotionDelay));
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
        yield return new WaitForSeconds(attackReadyMotionDelay);
        attackBoxObj.SetActive(true);
        yield return new WaitForSeconds(attackCurDelay);
        State = StateType.AttackExit;
        attackBoxObj.SetActive(false);
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
    #region Walk
    void Walk()
    {
        if (State != StateType.Hit && ChkAttack() == false && attackCurDelay <= 0)
        {
            var distance = playerTr.position - transform.position;
            distance.Normalize();

            transform.Translate(
                new Vector3(distance.x * Time.deltaTime, 0, 0), Space.World);
            transform.rotation = new Quaternion(0, distance.x >= 0 ? 0 : 180, 0, 0);

            State = StateType.Walk;
        }
    }
    #endregion Walk
    #region AboutRay
    [SerializeField] float chkAttackRangeDistance = 2f;
    [SerializeField] LayerMask playerLayer;
    bool ChkAttack()
    {
        if (ChkRay(tr.position
            , tr.forward.z == 1 ? Vector2.right : Vector2.left
            , chkAttackRangeDistance, playerLayer))
            return true;

        return false;
    }
    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }
    #endregion AboutRay
    #region OnTrigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackBoxObj"))
        {
            StartCoroutine(HitCo());
        }
    }

    IEnumerator HitCo()
    {
        State = StateType.Hit;
        yield return new WaitForSeconds(hitAnimationLenth);
        State = StateType.Idle;
    }
    #endregion OnTrigger


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
        Idle,
        Walk,
        Attack1,
        Attack2,
        AttackExit,
        Hit,
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
        Walk,
        Attack1,
        Attack2,
        Hit,
    }
    #endregion AnimationType
    #region AnimForState
    private void AnimForState()
    {
        switch (State)
        {
            case StateType.Idle:
                Anim = AnimType.Idle;
                break;
            case StateType.Walk:
                Anim = AnimType.Walk;
                break;
            case StateType.Attack1:
                Anim = AnimType.Attack1;
                break;
            case StateType.Attack2:
                Anim = AnimType.Attack2;
                break;
            case StateType.Hit:
                Anim = AnimType.Hit;
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
            case AnimType.Walk:
                animator.Play("Walk");
                break;
            case AnimType.Attack1:
                animator.Play("Attack1");
                break;
            case AnimType.Attack2:
                animator.Play("Attack2");
                break;
            case AnimType.Hit:
                animator.Play("Hit");
                break;
        }
    }
    #endregion AnimationPlay
    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
}

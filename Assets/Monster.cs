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
    [SerializeField] bool ishit = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        tr = transform;
    }
    void Update()
    {
        AnimForState();
        Walk();
        Attack();
    }

    void Attack()
    {
        if (ChkAttack())
        {
            State = StateType.Attack;
        }
    }

    void Walk()
    {
        if (ishit == false && ChkAttack() == false)
        {
            var distance = playerTr.position - transform.position;
            distance.Normalize();

            transform.Translate(
                new Vector3(distance.x * Time.deltaTime, 0, 0), Space.World);
            transform.rotation = new Quaternion(0, distance.x >= 0 ? 0 : 180, 0, 0);

            State = StateType.Walk;
        }
    }
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
    #region OnTrigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackBoxObj"))
        {
            StartCoroutine(HitCo());
            animator.Play("Hit");
        }
    }

    IEnumerator HitCo()
    {
        ishit = true;
        yield return new WaitForSeconds(hitAnimationLenth);
        ishit = false;
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
        Attack,
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
        Attack,
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
            case StateType.Attack:
                Anim = AnimType.Attack;
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
            case AnimType.Attack:
                animator.Play("Attack1");
                break;
        }
    }
    #endregion AnimationPlay
}

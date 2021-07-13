using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonster : MonoBehaviour
{
    // 하기시렁
    Transform tr;
    Transform playerTr;
    Func<IEnumerator> currentCo;
    Coroutine currnetCoHandle;

    Animator animator;

    [SerializeField] bool isAlive = false;
    [SerializeField] float speed = 3;
    IEnumerator Start()
    {
        #region Init
        tr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();

        isAlive = true;
        currentCo = IdleCo;
        #endregion Init

        while (isAlive)
        {
            currnetCoHandle = StartCoroutine(currentCo());
            yield return null;
        }
    }

    IEnumerator IdleCo()
    {
        State = StateType.Idle;

        while (ChkDetectDistance() == false)
        {

            yield return null;
        }

        currentCo = ChaseCo;
    }

    IEnumerator ChaseCo()
    {
        State = StateType.Run;
        while (ChkAttackDistance() == false)
        {

            yield return null;
        }

        //currentCo = AttackCo;
    }
    #region StateType
    [SerializeField] StateType state;
    StateType State
    {
        get => state;
        set
        {
            if (state == value)
                return;
            Debug.Log($"MonsterState : {state} -> {value}");
            state = value;
            animator.Play(state.ToString());
        }
    }
    enum StateType
    {
        Idle,
        Run,
        Attack1,
        Attack2,
        Attack3,
        AttackExit,
        Hit,
    }
    #endregion StateType

    #region Methods
    float detectDistance;
    [SerializeField] float detectRange = 5;
    bool ChkDetectDistance()
    {
        // Idle while 탈출판단
        // 범위 내에 들어오면 true
        // 범위 밖에 있으면 false
        detectDistance = Vector3.Distance(tr.position, playerTr.position);
        return detectDistance < detectRange;
    }
    float attackDistance;
    [SerializeField] float attackRange = 5;
    bool ChkAttackDistance()
    {
        // 범위 내에 들어오면 true
        // 범위 밖에 있으면 false
        attackDistance = Vector3.Distance(tr.position, playerTr.position);
        return attackDistance < attackRange;
    }



    void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    #endregion Methods
}

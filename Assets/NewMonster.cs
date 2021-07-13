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

    Vector3 GapforPlayer;
    IEnumerator ChaseCo()
    {
        State = StateType.Run;
        while (ChkAttackDistance() == false)
        {
            GapforPlayer = playerTr.position - tr.position;

            tr.Translate(speed * Time.deltaTime * GapforPlayer, Space.World);
            tr.rotation = Quaternion.Euler(0, GapforPlayer.x > 0 ? 0 : 180, 0);

            yield return null;
        }

        currentCo = AttackCo;
    }
    [SerializeField] float attackAnimLenth = 0.667f;
    [SerializeField] float attackPreDelay = 0.2f;
    IEnumerator AttackCo()
    {
        State = StateType.Attack1;
        yield return new WaitForSeconds(attackPreDelay);
        // 어택 적용할 곳
        yield return new WaitForSeconds(attackAnimLenth - attackPreDelay);
        currentCo = IdleCo;
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
    [SerializeField] float attackRange = 1.8f;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tr.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(tr.position, attackRange);
    }
    #endregion Methods
}

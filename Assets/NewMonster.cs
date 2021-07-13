using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonster : MonoBehaviour
{
    // 하기시렁
    Transform tr;
    Transform playerTr;
    CircleCollider2D attackCol;
    Animator animator;
    Func<IEnumerator> currentCo;
    Func<IEnumerator> CurrentCo
    {
        get => currentCo;
        set
        {
            currentCo = value;
            currnetCoHandle = null;
        }
    }
    Coroutine currnetCoHandle;
    [SerializeField] LayerMask playerLayer;


    [SerializeField] bool isAlive = false;
    [SerializeField] int hp = 20;
    [SerializeField] int damage = 5;
    [SerializeField] float speed = 3;
    IEnumerator Start()
    {
        #region Init
        tr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").transform;
        attackCol = tr.Find("AttackCol").GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        playerLayer = 1 << LayerMask.NameToLayer("Player");

        isAlive = true;
        CurrentCo = IdleCo;
        #endregion Init

        while (isAlive)
        {
            currnetCoHandle = StartCoroutine(CurrentCo());
            while (currnetCoHandle != null)
                yield return null;
        }
    }
    #region IdleCo
    IEnumerator IdleCo()
    {
        State = StateType.Idle;

        while (ChkDetectDistance() == false)
        {

            yield return null;
        }
        CurrentCo = ChaseCo;
    }
    #endregion IdleCo

    #region ChaseCo
    Vector3 GapforPlayer;
    float rotationY;
    float preRotationY;
    [SerializeField] float rotateDelay = 0.5f;
    IEnumerator ChaseCo()
    {
        State = StateType.Walk;
        while (ChkAttackDistance() == false)
        {
            GapforPlayer = playerTr.position - tr.position;
            GapforPlayer.y = 0;
            GapforPlayer.z = 0;
            GapforPlayer.Normalize();

            tr.Translate(speed * Time.deltaTime * GapforPlayer, Space.World);
            rotationY = GapforPlayer.x > 0 ? 0 : 180;

            if (rotationY != preRotationY)
            {
                State = StateType.Idle;
                yield return new WaitForSeconds(rotateDelay);
                State = StateType.Walk;
            }

            tr.rotation = Quaternion.Euler(0, rotationY, 0);
            preRotationY = rotationY;
            yield return null;
        }
        CurrentCo = AttackCo;
    }
    #endregion ChaseCo

    #region AttackCo
    [SerializeField] float attackAnimLenth = 0.667f;
    [SerializeField] float attackPreDelay = 0.2f;
    Collider[] hitCols;
    IEnumerator AttackCo()
    {
        State = StateType.Attack1;
        yield return new WaitForSeconds(attackPreDelay);
        // 어택 적용할 곳
        hitCols = Physics.OverlapSphere(
            attackCol.transform.position, attackCol.radius, playerLayer);
        foreach (var item in hitCols)
        {
            item.GetComponent<Player>().TakeHit(damage);
        }
        yield return new WaitForSeconds(attackAnimLenth - attackPreDelay);
        CurrentCo = ChaseCo;
    }
    #endregion AttackCo

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
        Walk,
        Attack1,
        Attack2,
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackCol.transform.position, attackCol.radius);
    }
    #endregion Methods
}

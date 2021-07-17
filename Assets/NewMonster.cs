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
        PlayAnim(state.ToString());

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
        PlayAnim(state.ToString());

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
    [SerializeField] float attackTime = 1f;
    [SerializeField] float attackPreDelay = 0.4f;
    Collider2D[] hitCols;
    IEnumerator AttackCo()
    {
        State = StateType.Attack1;
        PlayAnim(state.ToString(), 0, 0);

        yield return new WaitForSeconds(attackPreDelay);
        // 어택 적용할 곳
        var point = new Vector2(attackCol.transform.position.x, attackCol.transform.position.y);
        hitCols = Physics2D.OverlapCircleAll(point, attackCol.radius, playerLayer);
        foreach (var item in hitCols)
        {
            item.GetComponent<Player>().TakeHit(damage);
        }
        yield return new WaitForSeconds(attackTime - attackPreDelay);
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
        }
    }

    private void PlayAnim(string stateName, int? layer = null, float? normalizedTime = null)
    {
        if (layer != null)
        {
            if (normalizedTime != null)
                animator.Play(stateName, (int)layer, (float)normalizedTime);
            else
                animator.Play(stateName, (int)layer);
        }
        else
            animator.Play(stateName);
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
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tr.position, detectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tr.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackCol.transform.position, attackCol.radius);
        }
    }
    #endregion Methods
}

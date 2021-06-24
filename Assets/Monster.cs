using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator animator;
    Transform playerTr;
    Transform tr;
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
        //Walk();
        Attack();
    }

    void Attack()
    {
        if (ChkAttack())
        {
            animator.Play("Attack1");
        }
        else
        {
            animator.Play("Idle");
        }
    }

    void Walk()
    {
        if (ishit == false)
        {
            var distance = playerTr.position - transform.position;
            distance.Normalize();

            transform.Translate(
                new Vector3(distance.x * Time.deltaTime, 0, 0), Space.World);
            transform.rotation = new Quaternion(0, distance.x >= 0 ? 0 : 180, 0, 0);

            animator.Play("Walk");
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
}

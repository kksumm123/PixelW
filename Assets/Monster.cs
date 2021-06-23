using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator animator;
    Transform playerTr;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerTr = GetComponent<Transform>();
    }
    void Update()
    {
        var distance = playerTr.position - transform.position;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("AttackObj"))
        {
            ishit = true;
            animator.Play("Hit");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackObj"))
        {
            ishit = false;
        }
    }
    bool ishit = false;
}

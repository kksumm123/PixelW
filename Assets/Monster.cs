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
        playerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    {
        if (ishit == false)
        {
            var distance = playerTr.position.x - transform.position.x;
            transform.Translate(new Vector3(distance, 0, 0), Space.World);
            animator.Play("Walk");
            Debug.Log($"distance {distance}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackObj"))
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

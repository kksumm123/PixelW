using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    Animator animator;

    int value;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        value = Random.Range(0, 50);
        animator.Play("Idle");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.GetGold(value);
            animator.Play("Disappear");
            Destroy(gameObject, 1);
        }
    }
}

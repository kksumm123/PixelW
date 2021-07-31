using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    Animator animator;

    int value;
    [SerializeField] float risingTime = 0.5f;
    [SerializeField] float risingSpeed = 3;
    IEnumerator Start()
    {
        animator = GetComponentInChildren<Animator>();
        value = Random.Range(0, 50);
        animator.Play("Idle");
        var endTime = Time.time + risingTime;
        while (Time.time < endTime)
        {
            transform.Translate(risingSpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }
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

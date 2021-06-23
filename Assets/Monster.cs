using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator animator;
    Transform playerTr;
    [SerializeField] float hitAnimationLenth;
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
            transform.Translate(new Vector3(distance * Time.deltaTime, 0, 0), Space.World);
            animator.Play("Walk");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackObj"))
        {
            StartCoroutine(HitCo());
            animator.Play("Hit");
        }
    }

    private IEnumerator HitCo()
    {
        ishit = true;
        yield return new WaitForSeconds(hitAnimationLenth);
        ishit = false;
    }
    bool ishit = false;
}

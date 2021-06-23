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
            var distance = playerTr.position - transform.position;
            distance.Normalize();
            transform.Translate(
                new Vector3(distance.x * Time.deltaTime, 0, 0), Space.World);
            transform.rotation = new Quaternion(0, distance.x >= 0 ? 0 : 180, 0, 0);
            animator.Play("Walk");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackBoxObj"))
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

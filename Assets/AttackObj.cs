using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObj : MonoBehaviour
{
    BoxCollider2D boxCol2D;
    Vector2 boxCol2DSize;
    [SerializeField] Vector2 attackForce = new Vector3(1000, 2000, 0);
    private void Start()
    {
        boxCol2D = GetComponent<BoxCollider2D>();
        boxCol2DSize = boxCol2D.size;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            collision.GetComponent<Rigidbody2D>().AddForce(attackForce);
            boxCol2D.size = Vector2.zero;
        }
    }
    private void OnDisable()
    {
        boxCol2D.size = boxCol2DSize;
    }
}

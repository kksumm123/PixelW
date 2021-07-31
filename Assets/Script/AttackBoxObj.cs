using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBoxObj : MonoBehaviour
{
    BoxCollider2D boxCol2D;
    Vector2 boxCol2DSize;
    Transform parentTr;
    [SerializeField] string tartgetTagName;
    [SerializeField] Vector2 attackForce = new Vector3(1000, 2000, 0);

    void Start()
    {
        boxCol2D = GetComponent<BoxCollider2D>();
        boxCol2DSize = boxCol2D.size;
        parentTr = transform.parent.GetComponent<Transform>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Assert(tartgetTagName != null, "Å¸°Ù ÁöÁ¤¾ÈµÊ");
        if (collision.CompareTag(tartgetTagName))
        {
            var attackDir = parentTr.forward.z;
            collision.GetComponent<Rigidbody2D>()
                .AddForce(attackForce * new Vector2(attackDir, 1));
            boxCol2D.size = Vector2.zero;
        }
    }
    void OnDisable()
    {
        boxCol2D.size = boxCol2DSize;
    }
}

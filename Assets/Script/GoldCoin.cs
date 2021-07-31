using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    CircleCollider2D circleCol2D;
    LayerMask wallLayer;
    Animator animator;

    float rayDistance = 0;
    int value;
    float gravityAcceleration = 9.81f;
    float gravityVelocity;
    float s;
    float risingTime = 0.2f;
    float risingSpeed = 10;
    float flySpeed = 5;
    float rotationZValue = 20;

    IEnumerator Start()
    {
        Init();

        circleCol2D = GetComponent<CircleCollider2D>();
        rayDistance = circleCol2D.radius + 0.01f;
        wallLayer = 1 << LayerMask.NameToLayer("Ground");
        Debug.Assert(wallLayer != 0, "·¹ÀÌ¾î ÁöÁ¤¾ÈµÊ");

        animator = GetComponentInChildren<Animator>();
        value = Random.Range(0, 50);
        transform.rotation = Quaternion.Euler(
                        transform.rotation.eulerAngles 
                        + new Vector3(0, 0, Random.Range(-rotationZValue, rotationZValue))
                        );
        // Rising
        var endTime = Time.time + risingTime;
        while (Time.time < endTime)
        {
            transform.Translate(risingSpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }
        // Fly as parabola(Æ÷¹°¼±)
        while (IsGround() == false)
        {
            transform.Translate(flySpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }
    }
    void Init()
    {
        gravityAcceleration = 9.81f;
        gravityVelocity = 0;
        s = 0;
    }

    RaycastHit2D ray;
    void Update()
    {
        if (IsGround() == false)
            gravityAccelerationMove(); // ¶¥¿¡ ¾È´ê¾ÒÀ¸¸é
        else
            Init(); //¶¥¿¡ ´ê¾ÒÀ¸¸é
    }

    bool IsGround()
    { // true = ¶¥¿¡ ´êÀ½, false = ¶¥¿¡ ¾È´êÀ½
        ray = Physics2D.Raycast(
                    transform.position, Vector2.down, rayDistance, wallLayer);
        return ray.transform;
    }

    float t;
    void gravityAccelerationMove()
    {
        t = Time.deltaTime;

        s = gravityVelocity + (0.5f * gravityAcceleration * Mathf.Pow(t, 2));

        transform.Translate(new Vector3(0, -s * t, 0), Space.World);

        gravityVelocity += gravityAcceleration * t;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, rayDistance * Vector3.down);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { 
            transform.rotation = Quaternion.identity;
            Player.Instance.GetGold(value);
            animator.Play("Disappear");
            Destroy(gameObject, 1);
        }
    }
}

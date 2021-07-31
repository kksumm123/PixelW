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
        GravityInit();

        circleCol2D = GetComponent<CircleCollider2D>();
        rayDistance = 0.01f;
        wallLayer = 1 << LayerMask.NameToLayer("Ground");
        Debug.Assert(wallLayer != 0, "���̾� �����ȵ�");
        animator = GetComponentInChildren<Animator>();

        value = Mathf.RoundToInt(Random.Range(0, 25));
        transform.position += new Vector3(0, 0.25f, 0);
        transform.rotation = Quaternion.Euler(
                        transform.rotation.eulerAngles
                        + new Vector3(0, 0, Random.Range(-rotationZValue, rotationZValue))
                        );
        // Rising
        var endTime = Time.time + risingTime;
        while (Time.time < endTime)
        {
            if (IsReachUpLeftDown() == true)
                yield break;

            transform.Translate(risingSpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }
        // Fly as parabola(������)
        while (IsGround() == false)
        {
            if (IsReachUpLeftDown() == true)
                yield break;

            transform.Translate(flySpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }
    }
    void GravityInit()
    {
        gravityAcceleration = 9.81f;
        gravityVelocity = 0;
        s = 0;
    }

    RaycastHit2D ray;
    void Update()
    {
        if (IsGround() == false)
            gravityAccelerationMove(); // ���� �ȴ������
        else
            GravityInit(); //���� �������
    }

    Vector2 rayStartPos;
    bool IsGround()
    { // true = ���� ����, false = ���� �ȴ���
        rayStartPos = transform.position - new Vector3(0, circleCol2D.radius, 0);
        ray = Physics2D.Raycast(rayStartPos, Vector2.down, rayDistance, wallLayer);
        return ray.transform;
    }
    bool IsReachUpLeftDown()
    { // true = ���� ����, false = ���� �ȴ���
        rayStartPos = transform.position - new Vector3(circleCol2D.radius, 0, 0);
        if (Physics2D.Raycast(rayStartPos, Vector2.left, rayDistance, wallLayer))
            return true;
        rayStartPos = transform.position + new Vector3(circleCol2D.radius, 0, 0);
        if (Physics2D.Raycast(rayStartPos, Vector2.right, rayDistance, wallLayer))
            return true;
        rayStartPos = transform.position + new Vector3(0, circleCol2D.radius, 0);
        if (Physics2D.Raycast(rayStartPos, Vector2.up, rayDistance, wallLayer))
            return true;

        return false;
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
        if (Application.isPlaying == true)
        {
            Gizmos.color = Color.red;
            rayStartPos = transform.position + new Vector3(0, circleCol2D.radius, 0);
            Gizmos.DrawRay(rayStartPos, rayDistance * Vector3.down);
            rayStartPos = transform.position - new Vector3(circleCol2D.radius, 0, 0);
            Gizmos.DrawRay(rayStartPos, rayDistance * Vector3.left);
            rayStartPos = transform.position + new Vector3(circleCol2D.radius, 0, 0);
            Gizmos.DrawRay(rayStartPos, rayDistance * Vector3.right);

        }
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

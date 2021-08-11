using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    CircleCollider2D circleCol2D;
    LayerMask wallLayer;
    Animator animator;

    float rayDistance = 0;
    int goldValue;
    int goldMinValue = 5;
    int goldMaxValue = 25;
    float gravityAcceleration = 9.81f;
    float gravityVelocity;
    float s;
    float risingMinTime = 0.05f;
    float risingMaxTime = 0.1f;
    float risingSpeed = 10;
    float flySpeed = 5;
    float rotationZValue = 30;
    IEnumerator Start()
    {
        GravityInit();

        circleCol2D = GetComponent<CircleCollider2D>();
        rayDistance = 0.01f;
        wallLayer = 1 << LayerMask.NameToLayer("Ground");
        Debug.Assert(wallLayer != 0, "레이어 지정안됨");
        animator = GetComponentInChildren<Animator>();

        // 동전에 골드값 랜덤하게 할당
        goldValue = Random.Range(goldMinValue, goldMaxValue + 1);

        // 동전 소환지점 조금 위로 올림
        transform.position += new Vector3(0, 0.25f, 0);
        // 동전 소환 각도 랜덤하게 지정(부채꼴 모양처럼 퍼짐)
        transform.rotation = Quaternion.Euler(
                        transform.rotation.eulerAngles
                        + new Vector3(0, 0, Random.Range(-rotationZValue, rotationZValue))
                        );
        
        // 이하 포물선 움직임 되는 동안 Update()가 실행되기때문에
        // 중력 물리코드의 영향을 받아 점점 아래로 처짐

        // Rising
        // 포물선 상승곡선을 그려줌
        // 일정시간동안 상승함
        var endTime = Time.time + Random.Range(risingMinTime, risingMaxTime);
        while (Time.time < endTime)
        {
            // 만약 상,하,좌,우 하나라도 벽이랑 부딪혓다면
            if (IsReachUpLeftDown() == true)
                yield break; // Rising 그만하고 나가도록

            // 직선으로 움직이도록
            // 랜덤하게 각도가 지정되었기때문에 부채꼴처럼 퍼짐
            // 강해지는 중력에 의해 점차 포물선 상승곡선처럼 진행됨
            transform.Translate(risingSpeed * Time.deltaTime * Vector2.up);
            yield return null;
        }

        // Fly as parabola(포물선)
        // 포물선 하강곡선을 그려줌
        // 땅에 닿을 때까지 곡선으로 떨어짐
        while (IsGround() == false)
        {
            if (IsReachUpLeftDown() == true)
                yield break;
            
            // Rising과 같은방향 직선으로 움직이도록
            // RisingSpeed와 다르게 FlySpeed 값도 낮고, 
            // 시간이 지나며 중력값 또한 강해져 점차 포물선 하강곡선처럼 진행됨
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
            GravityAccelerationMove(); // 땅에 안닿았으면
        else
        {
            GravityInit(); //땅에 닿았으면
            PlayIdleAnim();
            enabled = false;
        }
    }

    void PlayIdleAnim()
    {
        transform.rotation = Quaternion.identity;
        animator.Play("Idle");
    }

    Vector2 rayStartPos;
    bool IsGround()
    { // true = 땅에 닿음, false = 땅에 안닿음
        rayStartPos = transform.position - new Vector3(0, circleCol2D.radius, 0);
        ray = Physics2D.Raycast(rayStartPos, Vector2.down, rayDistance, wallLayer);
        return ray.transform;
    }
    bool IsReachUpLeftDown()
    { // true = 땅에 닿음, false = 땅에 안닿음
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
    void GravityAccelerationMove()
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
            enabled = false;
            this.circleCol2D.enabled = false;
            TextObjectManager.instance.NewTextObject(transform, goldValue.ToString(), Color.yellow);
            transform.rotation = Quaternion.identity;
            Player.Instance.GetGold(goldValue);
            animator.Play("Disappear");
            Destroy(gameObject, 3);
        }
    }
}

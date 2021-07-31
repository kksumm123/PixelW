using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAcceleration : MonoBehaviour
{
    // �ӵ��� �����ϰ�
    // ���ӵ���ŭ ��������
    // �Ÿ���ŭ �̵�
    // t = �ð�, g = �߷°��ӵ�
    // v = �ӵ�, s = �̵��Ÿ�,
    // ���� �ӵ� v1 = v0 + gt
    // ���� �ð� s = v0 + (0.5 * g * t^2)

    [SerializeField] float gravityAcceleration = 9.81f;
    [SerializeField] float gravityVelocity;
    [SerializeField] float s;

    CircleCollider2D circleCol2D;
    float rayDistance = 0;
    LayerMask wallLayer;
    void Start()
    {
        Init();

        circleCol2D = GetComponent<CircleCollider2D>();
        rayDistance = circleCol2D.radius + 0.01f;
        wallLayer = 1 << LayerMask.NameToLayer("Ground");
        Debug.Assert(wallLayer != 0, "���̾� �����ȵ�");
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
            gravityAccelerationMove(); // ���� �ȴ������
        else
            Init(); //���� �������
    }

    bool IsGround()
    { // true = ���� ����, false = ���� �ȴ���
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
}
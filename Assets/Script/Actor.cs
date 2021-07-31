using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Rigidbody2D rigid;
    protected void Awake()
    {
        rigid = GetComponentInChildren<Rigidbody2D>();
    }
    protected void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    /// <summary>
    /// 화면 흔들림
    /// </summary>
    /// <param name="time">미 입력시 기본값 0.1f</param>
    protected void WiggleScreen(float time = 0.1f)
    {
        MoveCam.Instance.WiggleScreen(time);
    }
    protected void TakeKnockBack(Vector3 enemyForward)
    {
        rigid.Sleep();
        rigid.AddForce(new Vector2(200 * enemyForward.z, 50));
    }
}
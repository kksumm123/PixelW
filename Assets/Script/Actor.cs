using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    protected Rigidbody2D rigid;
    [SerializeField] protected int hp;
    int m_maxHp;
    [SerializeField] int m_power = 5;
    protected int Power
    {
        get => CalcPower(m_power);
        set => m_power = value;
    }

    protected int MaxHp
    {
        get
        {
            Debug.Assert(m_maxHp != 0, "SetMaxHpAndHp()호출할 것, maxHp 할당 해줘야 함");
            return m_maxHp;
        }
        set => m_maxHp = value;
    }
    protected void Awake()
    {
        rigid = GetComponentInChildren<Rigidbody2D>();
    }

    protected void SetMaxHpAndHp(int _maxHpValue)
    {
        hp = MaxHp = _maxHpValue;
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

    protected int CalcPower(int power)
    {
        var calcValue = (power * 0.2f);
        var minPower = power - calcValue;
        var maxPower = power + calcValue;

        return Mathf.RoundToInt(Random.Range(minPower, maxPower));
    }
}
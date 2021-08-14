using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    #region About Audios
    protected enum AudioType
    {
        None,
        Attack,
        Block,
        Death,
        Hit,
        GetCoin,
        Parrying,
        Roll,
    }
    [System.Serializable]
    protected class AudioData
    {
        public AudioType audioType;
        public AudioClip clip;
    }
    [SerializeField] protected List<AudioData> audioList;
    protected Dictionary<AudioType, AudioClip> audioMap;
    void SetAudioMap()
    {
        audioMap = new Dictionary<AudioType, AudioClip>();
        foreach (var item in audioList)
        {
            if (item.audioType != AudioType.None && item.clip != null)
                audioMap[item.audioType] = item.clip;
        }
        if (audioMap.Count == 0)
            print($"오디오 맵 비었다 - {transform}, {transform.name}");
    }
    protected void PlaySound(AudioType toPlayAudioType, float volume = 0.5f)
    {
        if (audioMap.ContainsKey(toPlayAudioType))
            AudioManager.instance.GenerateAudioClip(audioMap[toPlayAudioType], volume);
    }
    #endregion About Audios

    protected Rigidbody2D rigid;
    protected Animator animator;
    [Header("체력")]
    [SerializeField] int m_Hp;
    protected int Hp
    {
        get => m_Hp;
        set
        {
            m_Hp = value;
            if (m_Hp < 0)
                m_Hp = 0;
        }
    }
    int m_maxHp;
    protected int MaxHp
    {
        get
        {
            Debug.Assert(m_maxHp != 0, "SetMaxHpAndHp()호출할 것, maxHp 할당 해줘야 함");
            return m_maxHp;
        }
    }
    [SerializeField] int m_power = 5;
    protected int Power
    {
        get => CalcPower(m_power);
        set => m_power = value;
    }

    protected void Start()
    {
        rigid = GetComponentInChildren<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        SetAudioMap();
    }

    protected void SetMaxHpAndHp(int _maxHpValue)
    {
        Hp = m_maxHp = _maxHpValue;
    }

    protected void StopCo(Coroutine handle)
    {
        if (handle != null)
            StopCoroutine(handle);
    }
    protected Coroutine StopAndStartCoroutine(Coroutine handle, IEnumerator function)
    {
        if (handle != null)
            StopCoroutine(handle);
        return StartCoroutine(function);
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
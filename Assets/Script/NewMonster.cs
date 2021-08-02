using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewMonster : Actor
{
    #region Init
    public static List<NewMonster> totalMonster = new List<NewMonster>();

    Transform tr;
    Transform playerTr;
    CircleCollider2D attackCol;
    BoxCollider2D boxCol2D;
    Animator animator;
    Func<IEnumerator> m_currentFSM;
    Func<IEnumerator> CurrentFSM
    {
        get => m_currentFSM;
        set
        {
            m_currentFSM = value;
            currnetCoHandle = null;
        }
    }
    Coroutine currnetCoHandle;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] bool isAlive = false;
    [SerializeField] int m_power = 5;
    int Power { get => CalcPower(m_power); }
    [SerializeField] float speed = 3;
    #endregion Init
    IEnumerator Start()
    {
        #region Init
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Monster"), LayerMask.NameToLayer("Monster"), true);
        totalMonster.Add(this);
        tr = GetComponent<Transform>();
        boxCol2D = GetComponent<BoxCollider2D>();
        attackCol = tr.Find("AttackCol").GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        SetMaxHpAndHp(20);
        yield return StartCoroutine(GetPlayerInstanceCo());
        isAlive = true;
        CurrentFSM = IdleCo;
        #endregion Init

        while (isAlive)
        {
            var preFSM = m_currentFSM;

            currnetCoHandle = StartCoroutine(CurrentFSM());

            //FSM�ȿ��� ���� �߻��� ���� ���� ���� �� ����
            if (currnetCoHandle == null && preFSM == m_currentFSM)
                yield return null;

            while (currnetCoHandle != null)
                yield return null;
        }
    }

    #region IdleCo
    IEnumerator IdleCo()
    {
        State = StateType.Idle;
        PlayAnim(State.ToString());
        while (ChkDetectDistance() == false)
            yield return new WaitForSeconds(RandomDelayTime(0.5f));

        CurrentFSM = ChaseCo;
    }
    #endregion IdleCo

    #region ChaseCo
    float rotationY;
    float preRotationY;
    [SerializeField] float rotateDelay = 0.5f;
    IEnumerator ChaseCo()
    {
        rotationY = dirforPlayer.x > 0 ? 0 : 180;
        tr.rotation = Quaternion.Euler(0, rotationY, 0);
        yield return new WaitForSeconds(RandomDelayTime(0.5f));

        State = StateType.Walk;
        PlayAnim(State.ToString());

        while (ChkAttackDistance() == false)
        {
            tr.Translate(speed * Time.deltaTime * DirForPlayer(), Space.World);
            rotationY = dirforPlayer.x > 0 ? 0 : 180;

            if (rotationY != preRotationY)
            {
                State = StateType.Idle;
                yield return new WaitForSeconds(rotateDelay + RandomDelayTime(0.2f));
                State = StateType.Walk;
            }

            tr.rotation = Quaternion.Euler(0, rotationY, 0);
            preRotationY = rotationY;
            yield return null;
        }
        CurrentFSM = AttackCo;
    }
    #endregion ChaseCo

    #region AttackCo
    [SerializeField] float attackTime = 1f;
    [SerializeField] float attackPreDelay = 0.5f;
    Collider2D[] hitCols;
    IEnumerator AttackCo()
    {
        yield return new WaitForSeconds(RandomDelayTime(0.1f));
        State = StateType.Attack1;
        PlayAnim(State.ToString(), 0, 0);
        transform.rotation = // ������ �÷��̾� �������� ȸ�� 
            Quaternion.Euler(0, DirForPlayer().x > 0 ? 0 : 180, 0);
        yield return new WaitForSeconds(attackPreDelay);
        // ���� ������ ��
        var point = new Vector2(attackCol.transform.position.x, attackCol.transform.position.y);
        hitCols = Physics2D.OverlapCircleAll(point, attackCol.radius, playerLayer);
        foreach (var item in hitCols)
        {
            item.GetComponent<Player>().TakeHit(Power, transform);
        }
        yield return new WaitForSeconds(attackTime - attackPreDelay);
        CurrentFSM = ChaseCo;
    }
    #endregion AttackCo

    #region TakeHit
    public void TakeHit(int damage, Vector3 playerForward)
    {
        if (hp > 0)
        {
            hp -= damage;
            TextObjectManager.instance.NewTextObject(transform, damage.ToString(), Color.red);
            // ���� ����Ǵ� �ڷ�ƾ ����
            StopCo(currnetCoHandle);
            TakeKnockBack(playerForward);
            if (hp > 0)
                CurrentFSM = TakeHitCo; // �ڷ�ƾ TakeHit
            else
                CurrentFSM = DeathCo;
        }
    }

    [SerializeField] float hitDelay = 0.5f;
    IEnumerator TakeHitCo()
    {
        // Death ����
        // Hit, Death �ִϸ��̼�

        State = StateType.Hit;
        PlayAnim(State.ToString(), 0, 0);
        yield return new WaitForSeconds(hitDelay);
        State = StateType.Idle;
        CurrentFSM = IdleCo;
    }
    #endregion TakeHit

    #region DeathCo
    [SerializeField] float deathDelay = 2f;
    IEnumerator DeathCo()
    {
        totalMonster.Remove(this);
        rigid.isKinematic = true;
        rigid.velocity = Vector2.zero;
        boxCol2D.enabled = false;
        State = StateType.Death;
        PlayAnim(State.ToString());
        SpawnCoins();
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
    #endregion DeathCo

    #region StateType
    [SerializeField] StateType m_state;
    StateType State
    {
        get => m_state;
        set
        {
            if (m_state == value)
                return;
            Debug.Log($"MonsterState : {m_state} -> {value}");
            m_state = value;
        }
    }

    enum StateType
    {
        Idle,
        Walk,
        Attack1,
        Attack2,
        Hit,
        Death,
    }
    #endregion StateType

    #region Methods
    float RandomDelayTime(float maxValue)
    {
        return Random.Range(0, maxValue);
    }
    IEnumerator GetPlayerInstanceCo()
    {
        while (Player.Instance == null)
            yield return null;
        playerTr = Player.Instance.transform;
    }

    float detectDistance;
    [SerializeField] float detectRange = 5;
    bool ChkDetectDistance()
    {
        // Idle while Ż���Ǵ�
        // ���� ���� ������ true
        // ���� �ۿ� ������ false
        detectDistance = Vector3.Distance(tr.position, playerTr.position);
        return detectDistance < detectRange;
    }


    float attackDistance;
    [SerializeField] float attackRange = 1.8f;

    bool ChkAttackDistance()
    {
        // ���� ���� ������ true
        // ���� �ۿ� ������ false
        attackDistance = Vector3.Distance(tr.position, playerTr.position);
        return attackDistance < attackRange;
    }
    Vector3 dirforPlayer;
    private Vector3 DirForPlayer()
    {
        dirforPlayer = playerTr.position - tr.position;
        dirforPlayer.y = 0;
        dirforPlayer.z = 0;
        dirforPlayer.Normalize();
        return dirforPlayer;
    }

    readonly string goldCoinString = "GoldCoin";
    int coinMaxCount = 5;
    void SpawnCoins()
    {
        var coinCount = Mathf.RoundToInt(Random.Range(0, coinMaxCount));
        var coinGo = (GameObject)Resources.Load(goldCoinString);
        for (int i = 0; i < coinCount; i++)
        {
            Instantiate(coinGo, transform.position, transform.rotation);
        }
    }

    void PlayAnim(string stateName, int? layer = null, float? normalizedTime = null)
    {
        if (layer != null)
        {
            if (normalizedTime != null)
                animator.Play(stateName, (int)layer, (float)normalizedTime);
            else
                animator.Play(stateName, (int)layer);
        }
        else
            animator.Play(stateName);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(tr.position, detectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(tr.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackCol.transform.position, attackCol.radius);
        }
    }
    #endregion Methods
}

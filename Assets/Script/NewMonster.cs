using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// Goblin
// �̼� ����, ���� ����, �ǰ� ȸ�� ����
// ���̷���
// �̼� ����, ���� ����, �ǰ� ȸ�� ����
// ����
// ������, �̼� ����, ���� ����, �ǰ� ȸ�� ����
public class NewMonster : Actor
{
    #region Init
    public static List<NewMonster> totalMonster = new List<NewMonster>();

    Transform playerTr;
    CircleCollider2D attackCol;
    BoxCollider2D boxCol2D;
    GameObject hpBarGo;
    Transform hpBarGauge;
    Coroutine hpBarCoHandle;
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
    [Header("�ִ�ü��")]
    [SerializeField] int initMaxHp = 20;
    [SerializeField] float speed = 3;

    LayerMask wallLayer;
    #endregion Init
    new IEnumerator Start()
    {
        #region Init
        base.Start();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Monster"), LayerMask.NameToLayer("Monster"), true);
        totalMonster.Add(this);
        boxCol2D = GetComponent<BoxCollider2D>();
        attackCol = transform.Find("AttackCol").GetComponent<CircleCollider2D>();
        attackRange = Mathf.Abs(attackCol.transform.localPosition.x) + Mathf.Abs(attackCol.radius);
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        SetMaxHpAndHp(initMaxHp);
        hpBarGo = transform.Find("HPBar").gameObject;
        hpBarGauge = transform.Find("HPBar/Gauge");
        wallLayer = 1 << LayerMask.NameToLayer("Ground");
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

        while (ChkDetectDistance() == false)
        {
            if (Random.Range(0, 2) == 0)
            {
                State = StateType.Idle;
                PlayAnim(State.ToString());
                yield return new WaitForSeconds(RandomDelayTime(0.5f));
            }
            else
            {
                int idleMoveDir = Random.Range(0, 2) == 0 ? 1 : -1;
                rotationY = idleMoveDir == 1 ? 0 : 180;
                transform.rotation = Quaternion.Euler(0, rotationY, 0);
                var endTime = Time.time + RandomDelayTime(0.5f);
                while (Time.time < endTime)
                {
                    State = StateType.Walk;
                    PlayAnim(State.ToString());
                    Vector3 idleMoveVector3 = new Vector3(speed * Time.deltaTime * 1, 0, 0);
                    trTranslate(idleMoveVector3);
                    yield return null;
                }
            }
        }

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
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
        yield return new WaitForSeconds(RandomDelayTime(0.5f));

        State = StateType.Walk;
        PlayAnim(State.ToString());

        while (ChkAttackDistance() == false)
        {
            trTranslate(speed * Time.deltaTime * DirForPlayer(), Space.World);
            rotationY = dirforPlayer.x > 0 ? 0 : 180;

            if (rotationY != preRotationY)
            {
                State = StateType.Idle;
                yield return new WaitForSeconds(rotateDelay + RandomDelayTime(0.2f));
                State = StateType.Walk;
            }

            transform.rotation = Quaternion.Euler(0, rotationY, 0);
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
        yield return new WaitForSeconds(RandomDelayTime(0.15f));
        State = Random.Range(0, 2) == 0 ? StateType.Attack1 : StateType.Attack2;
        PlayAnim(State.ToString(), 0, 0);
        transform.rotation = // ������ �÷��̾� �������� ȸ�� 
            Quaternion.Euler(0, DirForPlayer().x > 0 ? 0 : 180, 0);
        yield return new WaitForSeconds(attackPreDelay);
        // ���� ������ ��
        PlaySound(AudioType.Attack);
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
        if (Hp > 0)
        {
            Hp -= damage;
            TextObjectManager.instance.NewTextObject(transform, damage.ToString(), Color.red);
            // ���� ����Ǵ� �ڷ�ƾ ����
            StopCo(currnetCoHandle);
            UpdateHPBar();
            hpBarCoHandle = StopAndStartCoroutine(hpBarCoHandle, HPBarCo());
            CreateBloodEffect();
            TakeKnockBack(playerForward);
            if (Hp > 0)
                CurrentFSM = TakeHitCo; // �ڷ�ƾ TakeHit
            else
                CurrentFSM = DeathCo;
        }
    }

    [SerializeField] float hitDelay = 0.5f;
    IEnumerator TakeHitCo()
    {
        State = StateType.Hit;
        PlaySound(AudioType.Hit);
        PlayAnim(State.ToString(), 0, 0);

        yield return new WaitForSeconds(hitDelay);
        State = StateType.Idle;
        CurrentFSM = IdleCo;
    }

    Vector3 hpBarGaugeScale;
    void UpdateHPBar()
    {
        hpBarGaugeScale = hpBarGauge.localScale;
        hpBarGaugeScale.x = (float)Hp / MaxHp;
        hpBarGauge.localScale = hpBarGaugeScale;
    }

    float hpBarVisibleTime = 2f;
    IEnumerator HPBarCo()
    {
        hpBarGo.SetActive(true);
        yield return new WaitForSeconds(hpBarVisibleTime);
        hpBarGo.SetActive(false);
    }

    #endregion TakeHit

    #region DeathCo
    [SerializeField] float deathDelay = 2f;
    IEnumerator DeathCo()
    {
        PlaySound(AudioType.Death);
        totalMonster.Remove(this);
        if (totalMonster.Count == 0)
            StageManager.instance.OnStageClear();

        hpBarGo.SetActive(false);
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
    Vector3 isMoveRayPosition;
    private void trTranslate(Vector3 moveDir, Space spaceDir = Space.Self)
    {
        isMoveRayPosition = transform.position - new Vector3((boxCol2D.size.x * 0.5f + boxCol2D.offset.x + 0.1f) * -transform.forward.z
                                             , boxCol2D.size.y * 0.5f - boxCol2D.offset.y + 0.1f);
        var hit = Physics2D.Raycast(isMoveRayPosition, new Vector2(transform.forward.z, 0), 0.1f, wallLayer);
        if (hit.transform != null)
            transform.Translate(moveDir, spaceDir);
    }
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
        if (Player.Instance == null)
            return false;

        detectDistance = Vector3.Distance(transform.position, playerTr.position);
        return detectDistance < detectRange;
    }

    float attackDistance;
    [SerializeField] float attackRange;
    bool ChkAttackDistance()
    {
        // ���� ���� ������ true
        // ���� �ۿ� ������ false
        if (Player.Instance == null)
            return false;

        attackDistance = Vector3.Distance(transform.position, playerTr.position);
        return attackDistance < attackRange;
    }
    Vector3 dirforPlayer;
    private Vector3 DirForPlayer()
    {
        dirforPlayer = playerTr.position - transform.position;
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
            Gizmos.DrawWireSphere(transform.position, detectRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackCol.transform.position, attackCol.radius);
            Gizmos.color = Color.white;
            Gizmos.DrawRay(isMoveRayPosition, new Vector2(Mathf.Abs(transform.forward.z * 0.2f), 0));
        }
    }
    #endregion Methods
}

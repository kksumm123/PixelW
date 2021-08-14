using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//-------------
// 토요일
// clear HP UI만들기 및 연동 - maxHp 만들고, 좌측 상단에 만들기
// clear 몬스터 죽으면 동전 떨어트리기 https://youtu.be/a0Rf8C3UpdU?t=110
// clear 현재 소지 골드 표시하기
// clear 골드 먹을 때 value text 보여주기
// clear 공격시 앞으로 조금 전진하도록 (플레이어)
// clear 피격시 넉백 방향 이상한거 수정하기 (플레이어, 몬스터)
//-------------
// 일요일
// clear 버그 - 방패 올린 상태로 구를시, 속도가 안돌아옴 
// clear 버그 - WallSlide 발동 후 풀리지않음 
// clear 데미지 수치 랜덤화
// clear WallSlide 판단로직 수정하기, 캐릭터중앙, 발바닥
// clear 맵 크기 좀더 키움
// clear 몬스터가 플레이어 발견시 바로 rotate 하도록 수정
// clear 몬스터 마릿수 카운트 UI 완성
//------------
// 화요일
// clear Rolling 무적시간 축소
// clear 방패 올리고있으면 10% 데미지 받도록
// Clear 몬스터 HPBar 추가
//   ㄴ> 기본적으로 안보임, 피격시 2초 간 노출, 이후 다시 안보임
//------------
// 일요일
// clear 몬스터 다양하게
//   ㄴ Skeleton(0), Goblin(0), Mushroon(O)
// clear 몬스터 발판 아래로 안떨어지도록
//------------
// 목요일
// clear 조작키 알림 UI 만들기
// todo 다음 맵으로 넘어가기
//   ㄴ CenterNotifyUI 완성
// todo SFX들 추가하기 - Coin습득, 공격, 피격, Environmenmt SFX, BGM

public class Player : Actor
{
    #region Declare
    static Player m_instance = null;
    public static Player Instance { get => m_instance; }
    void Awake()
    {
        m_instance = this;
    }

    BoxCollider2D boxCol2D;
    Transform blockFlashTr;
    GameObject blockFlashEffectGo;
    readonly string blockFlashEffectString = "BlockFlashEffect";
    AudioSource runAudioSource;

    [SerializeField] StateType state;
    [SerializeField] AnimType anim = AnimType.Idle;
    [SerializeField] int gold = 0;
    float normalSpeed = 5f;
    float battleSpeed = 0.5f;
    #endregion Declare

    #region AboutRay
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position
            + new Vector3(0, groundRayOffsetY, 0)
            , Vector2.down * groundRayLength);
        Gizmos.DrawRay(transform.position
            + new Vector3(-groundRayOffsetX, groundRayOffsetY, 0)
            , Vector2.down * groundRayLength);
        Gizmos.DrawRay(transform.position
            + new Vector3(groundRayOffsetX, groundRayOffsetY, 0)
            , Vector2.down * groundRayLength);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + new Vector3(-slideRayOffsetX, slideRayOffsetY, 0)
            , Vector2.left * slideRayLength);
        Gizmos.DrawRay(transform.position + new Vector3(-slideRayOffsetX, 0, 0)
            , Vector2.left * slideRayLength);

        Gizmos.DrawRay(transform.position + new Vector3(slideRayOffsetX, slideRayOffsetY, 0)
                 , Vector2.right * slideRayLength);
        Gizmos.DrawRay(transform.position + new Vector3(slideRayOffsetX, 0, 0)
                 , Vector2.right * slideRayLength);
    }

    bool ChkRay(Vector3 pos, Vector2 dir, float length, LayerMask layer)
    {
        Debug.Assert(layer != 0, "레이어 지정안됨");
        var hit = Physics2D.Raycast(pos, dir, length, layer);
        return hit.transform;
    }
    #endregion AboutRay

    #region Start()
    float originSpeed;

    [SerializeField] int initMaxHp = 250;
    new void Start()
    {
        base.Start();
        boxCol2D = GetComponentInChildren<BoxCollider2D>();
        blockFlashTr = transform.Find("Sprite/BlockFlashPosition");
        blockFlashEffectGo = (GameObject)Resources.Load(blockFlashEffectString);
        attackBoxTr = transform.Find("Sprite/AttackBox");
        enemyLayer = 1 << LayerMask.NameToLayer("Monster");
        runAudioSource = GetComponentInChildren<AudioSource>();

        SetGroundRaySetting();
        originSpeed = normalSpeed;
        SetMaxHpAndHp(initMaxHp);

        void SetGroundRaySetting()
        {
            slideRayOffsetX = boxCol2D.size.x / 2;
            slideRayOffsetY = boxCol2D.size.y / 2;
            groundLayer = 1 << LayerMask.NameToLayer("Ground");
        }
    }
    #endregion Start()

    #region Update()
    bool isUpdatePhysics = false;
    void FixedUpdate()
    {
        isUpdatePhysics = true;
    }

    void Update()
    {
        StateUpdate();
        AnimForState();

        if (State != StateType.Hit && State != StateType.Death)
        {
            Move();
            Rolling();
            Jump();
            Attack();
            Block();
        }

        PlayRunningSound();
    }
    #endregion Update()

    #region StateUpdate
    void StateUpdate()
    {
        if (isUpdatePhysics == false)
            return;
        else if (isRolling == true)
            return;

        if (State != StateType.Hit && IsBattle() == false)
        {
            if (IsGound())
            {
                if (moveX == 0)
                {
                    State = StateType.Ground;
                    rigid.velocity = Vector2.zero;
                }
                else
                    State = StateType.Run;
            }
            else
            {
                if (IsWall() == false)
                {
                    var velo = rigid.velocity;

                    if (velo.y > 0)
                        State = StateType.Jump;

                    if (velo.y < 0)
                        State = StateType.Fall;
                }

                if (IsWallSlide())
                    State = StateType.WallSlide;
            }
            //if (State != StateType.Jump && State != StateType.Fall
            //    && State != StateType.WallSlide && moveX != 0)
            //    State = StateType.Run;
        }
    }
    #region IsAttackAndBlock
    private bool IsBattle()
    {
        switch (State)
        {
            case StateType.Attack1:
            case StateType.Attack2:
            case StateType.Attack3:
            case StateType.IdleBlock:
            case StateType.Block:
            case StateType.Hit:
            case StateType.Death:
                return true;
        }
        return false;
    }
    #endregion IsAttackAndBlock
    #region IsIdle
    private bool IsIdle()
    {
        switch (State)
        {
            case StateType.Ground:
            case StateType.Run:
                return true;
        }
        return false;
    }
    #endregion IsIdle
    #region IsBlocking
    private bool IsBlocking()
    {
        switch (State)
        {
            case StateType.IdleBlock:
            case StateType.Block:
                return true;
        }
        return false;
    }
    #endregion IsBlocking
    #region IsGound
    float groundRayOffsetX = 0.3f;
    float groundRayOffsetY = 0.2f;
    float groundRayLength = 0.2f;
    LayerMask groundLayer;
    private bool IsGound()
    {
        var velo = rigid.velocity;
        if (Mathf.Approximately(velo.y, 0) == true)
        {
            var pos = transform.position;
            if (ChkRay(pos + new Vector3(0, groundRayOffsetY, 0)
                , Vector2.down, groundRayLength, groundLayer))
                return true;
            if (ChkRay(pos + new Vector3(-groundRayOffsetX, groundRayOffsetY, 0)
                , Vector2.down, groundRayLength, groundLayer))
                return true;
            if (ChkRay(pos + new Vector3(groundRayOffsetX, groundRayOffsetY, 0)
                , Vector2.down, groundRayLength, groundLayer))
                return true;
        }
        return false;
    }
    #endregion IsGound
    #region IsWall
    float slideRayOffsetX = 0;
    float slideRayOffsetY = 0;
    float slideRayLength = 0.01f;
    private bool IsWallSlide()
    {
        if (State == StateType.Jump || State == StateType.Fall)
        {
            if (IsWall())
                return true;
        }

        return false;
    }

    bool IsWall()
    {
        if (transform.forward.z == 1)
        {
            if (IsWallRight())
                return true;
        }
        else
        {
            if (IsWallLeft())
                return true;
        }

        return false;
    }
    bool IsWallRight()
    {
        if (ChkRay(transform.position + new Vector3(slideRayOffsetX, slideRayOffsetY, 0)
                , Vector2.right, slideRayLength, groundLayer))
            if (ChkRay(transform.position + new Vector3(slideRayOffsetX, 0, 0)
                , Vector2.right, slideRayLength, groundLayer))
                return true;
        return false;
    }
    bool IsWallLeft()
    {
        if (ChkRay(transform.position + new Vector3(-slideRayOffsetX, slideRayOffsetY, 0)
            , Vector2.left, slideRayLength, groundLayer))
            if (ChkRay(transform.position + new Vector3(-slideRayOffsetX, 0, 0)
            , Vector2.left, slideRayLength, groundLayer))
                return true;
        return false;
    }
    #endregion IsWall

    #endregion StateUpdate

    #region Move
    float moveX = 0;
    private void Move()
    {
        if (State == StateType.WallSlide)
            return;
        else if (Mathf.Abs(rigid.velocity.x) > 4.9)
            return;
        else if (isRolling == true)
            return;

        moveX = 0;
        if (Input.GetKey(KeyCode.A))
            moveX = -1;
        if (Input.GetKey(KeyCode.D))
            moveX = 1;

        if (moveX != 0)
        {
            if (moveX == -1 && IsWallLeft() == true)
                return;
            if (moveX == 1 && IsWallRight() == true)
                return;

            if (IsBlocking() == false)
                transform.rotation = new Quaternion(0, moveX == -1 ? 180 : 0, 0, 0);
            var pos = transform.position;
            pos.x += moveX * normalSpeed * Time.deltaTime;
            transform.position = pos;
        }
    }
    #endregion Move

    #region Rolling
    bool isRolling = false;
    bool isInvincibility = false;
    private void Rolling()
    {
        if (isRolling)
        {
            var pos = transform.position;
            pos.x += transform.forward.z * normalSpeed * Time.deltaTime;
            transform.position = pos;
        }
        else if (IsGound() && Input.GetKey(KeyCode.LeftShift))
        {
            State = StateType.Roll;
            normalSpeed = originSpeed;
            StartCoroutine(RollingCo());
            StartCoroutine(RollingInvincibilityCo());
        }
    }

    float rollTime = 0.6f;
    private IEnumerator RollingCo()
    {
        isRolling = true;
        PlaySound(AudioType.Roll);
        yield return new WaitForSeconds(rollTime);
        isRolling = false;
    }
    float rollInvincibilityTime = 0.3f;
    IEnumerator RollingInvincibilityCo()
    {
        isInvincibility = true;
        yield return new WaitForSeconds(rollInvincibilityTime);
        isInvincibility = false;
    }
    #endregion Rolling

    #region Jump
    float jumpForce = 900f;
    float slideJumpForceX = 250f;
    float slideJumpForceY = 900f;
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // Normal Jump
            if (IsGound())
            {
                isUpdatePhysics = false;
                State = StateType.Jump;
                rigid.AddForce(new Vector2(0, jumpForce));
            }
            // Slide Jump
            else if (State == StateType.WallSlide)
            {
                isUpdatePhysics = false;
                State = StateType.Jump;

                rigid.velocity = Vector2.zero;

                var forZ = transform.forward.z;
                rigid.AddForce(new Vector2(slideJumpForceX * forZ * -1, slideJumpForceY));
                transform.rotation = Quaternion.Euler(0, transform.rotation.y == 0 ? 180 : 0, 0);
            }
        }
    }
    #endregion Jump

    #region Attack
    float attackApplyDelay = 0.15f;
    List<float> attackDelay = new List<float>() { 0.43f, 0.43f, 0.57f };
    float attackCurDelay = 0;
    int attackIdx = 0;
    int attackMaxIdx = 3;
    float attackIdxResetTime = 1.2f;
    float attackIdxResetCurTime = 0f;
    Coroutine attackCoHandle;
    Coroutine attackDelayCoHandle;
    Coroutine attackIndxResetCoHandle;
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsGound() && attackCurDelay <= 0)
            {
                if (attackIdx < attackMaxIdx)
                {
                    attackCurDelay = attackDelay[attackIdx];
                    attackIdxResetCurTime = attackIdxResetTime;
                    switch (attackIdx)
                    {
                        case 0:
                            State = StateType.Attack1;
                            break;
                        case 1:
                            State = StateType.Attack2;
                            break;
                        case 2:
                            State = StateType.Attack3;
                            break;
                    }
                    attackCoHandle = StopAndStartCoroutine(attackCoHandle, AttackCo(attackCurDelay));
                    attackDelayCoHandle = StopAndStartCoroutine(attackDelayCoHandle, AttackDelayCo());
                    attackIndxResetCoHandle = StopAndStartCoroutine(attackIndxResetCoHandle, AttackIndxResetCo());
                    attackIdx++;
                }
                else
                    attackIdx = 0;
            }
        }
    }
    Transform attackBoxTr;
    Vector2 attackBoxSize = new Vector2(1.9f, 1.5f);
    LayerMask enemyLayer;
    Collider2D[] attackedEnemies;

    IEnumerator AttackCo(float delay)
    {
        normalSpeed = battleSpeed;
        yield return new WaitForSeconds(attackApplyDelay);
        attackedEnemies = null;
        // 실제 공격 적용
        var point = new Vector2(attackBoxTr.transform.position.x
                        , attackBoxTr.transform.position.y);
        attackedEnemies = Physics2D.OverlapBoxAll(point, attackBoxSize, 90, enemyLayer);
        StartCoroutine(AttackMoveCo());
        foreach (var item in attackedEnemies)
        {
            item.GetComponent<NewMonster>().TakeHit(Power, transform.forward);
            WiggleScreen();
        }
        yield return new WaitForSeconds(delay);
        State = StateType.AttackAndHitExit;
        normalSpeed = originSpeed;
    }

    float attackTranslateSpeed = 4f;
    float attackTranslateTime = 0.1f;
    Vector3 attackMove;
    IEnumerator AttackMoveCo()
    {
        var endTime = Time.time + attackTranslateTime;
        while (Time.time < endTime)
        {
            attackMove = new Vector3(attackTranslateSpeed * Time.deltaTime * transform.forward.z, 0, 0);
            transform.Translate(attackMove, Space.World);
            yield return null;
        }
    }

    IEnumerator AttackDelayCo()
    {
        while (attackCurDelay > 0)
        {
            attackCurDelay -= Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator AttackIndxResetCo()
    {
        while (attackIdxResetCurTime > 0)
        {
            attackIdxResetCurTime -= Time.deltaTime;
            yield return null;
        }
        attackIdx = 0;
    }
    #endregion Attack

    #region Block
    float parryingTime = 0.3f;
    bool isParrying = false;
    Coroutine parrayingCoHendle;
    void Block()
    {
        if (IsIdle() || IsBlocking())
        {
            if (Input.GetMouseButtonDown(1))
            {
                State = StateType.IdleBlock;
                normalSpeed = battleSpeed;
                parrayingCoHendle = StopAndStartCoroutine(parrayingCoHendle, ParryingCo());
            }
            else if (Input.GetMouseButton(1) == false)
            {
                if (IsBlocking() == true)
                {
                    StopCo(parrayingCoHendle);
                    isParrying = false;
                    State = StateType.Ground;
                    normalSpeed = originSpeed;
                }
            }
        }
    }

    IEnumerator ParryingCo()
    {
        isParrying = true;
        yield return new WaitForSeconds(parryingTime);
        isParrying = false;
    }
    #endregion Block

    #region PlayRunningSound
    void PlayRunningSound()
    {
        if (State == StateType.Run)
            runAudioSource.enabled = true;
        else
            runAudioSource.enabled = false;
    }
    #endregion PlayRunningSound

    #region TakeHit
    public void TakeHit(int damage, Transform monsterTr)
    {
        if (Hp > 0)
        {
            // 무적이면 리턴
            if (isInvincibility == true)
                return;

            // 방패를 들어올리고, 정면이면
            if (IsBlocking() == true && FrontBlock(monsterTr) == true)
            {
                // 패링타임이면
                if (isParrying == true)
                {
                    Instantiate(blockFlashEffectGo, blockFlashTr.position, transform.rotation);
                    PlaySound(AudioType.Parrying);
                }
                else
                { // 패링타임이 지났으면
                    var roundDamage = Mathf.RoundToInt(damage * 0.1f);
                    Hp -= roundDamage;
                    TextObjectManager.instance.NewTextObject(transform, roundDamage.ToString(), Color.red);
                    TakeKnockBack(monsterTr.forward);
                    PlaySound(AudioType.Block);
                }
            }
            else
            {
                Hp -= damage;
                TextObjectManager.instance.NewTextObject(transform, damage.ToString(), Color.red);
                TakeKnockBack(monsterTr.forward);
                WiggleScreen();
                StartCoroutine(HitCo());
            }
        }
    }

    private bool FrontBlock(Transform monsterTr)
    {// true = Parrying, false = Fail Parrinying
        var distanceX = monsterTr.position.x - transform.position.x;

        return IsZero(transform.rotation.eulerAngles.y)
            ? IsPositive(distanceX) : IsNegative(distanceX);

        bool IsZero(float value)
        {
            return value == 0;
        }
        bool IsPositive(float value)
        {
            return value > 0;
        }
        bool IsNegative(float value)
        {
            return value < 0;
        }
    }

    float hitAnimationLenth = 0.273f;
    IEnumerator HitCo()
    {
        State = StateType.Hit;
        PlaySound(AudioType.Hit);
        yield return new WaitForSeconds(hitAnimationLenth);
        if (Hp > 0)
            State = StateType.AttackAndHitExit;
        else
            StartCoroutine(DeathCo());
    }

    float deathTime = 4;
    IEnumerator DeathCo()
    {
        State = StateType.Death;
        PlaySound(AudioType.Death);
        rigid.isKinematic = true;
        rigid.velocity = Vector2.zero;
        boxCol2D.enabled = false;
        yield return new WaitForSeconds(deathTime);
        GetComponentInChildren<Renderer>().enabled = false;
        //Destroy(gameObject);
    }
    #endregion TakeHit

    #region AnimForState
    private void AnimForState()
    {
        switch (State)
        {
            case StateType.Ground:
                Anim = AnimType.Idle;
                break;
            case StateType.Jump:
                Anim = AnimType.Jump;
                break;
            case StateType.Fall:
                Anim = AnimType.Fall;
                break;
            case StateType.Run:
                Anim = AnimType.Run;
                break;
            case StateType.WallSlide:
                Anim = AnimType.WallSlide;
                break;
            case StateType.Roll:
                Anim = AnimType.Roll;
                break;
            case StateType.Attack1:
                Anim = AnimType.Attack1;
                break;
            case StateType.Attack2:
                Anim = AnimType.Attack2;
                break;
            case StateType.Attack3:
                Anim = AnimType.Attack3;
                break;
            case StateType.IdleBlock:
                Anim = AnimType.IdleBlock;
                break;
            case StateType.Block:
                Anim = AnimType.Block;
                break;
            case StateType.Hit:
                Anim = AnimType.Hit;
                break;
            case StateType.Death:
                Anim = AnimType.Death;
                break;

        }
    }
    #endregion AnimForState
    #region AnimationPlay
    void AnimationPlay()
    {
        switch (Anim)
        {
            case AnimType.Idle:
                animator.Play("Idle");
                break;
            case AnimType.Run:
                animator.Play("Run");
                break;
            case AnimType.Jump:
                animator.Play("Jump");
                break;
            case AnimType.Fall:
                animator.Play("Fall");
                break;
            case AnimType.WallSlide:
                animator.Play("WallSlide");
                break;
            case AnimType.Roll:
                animator.Play("Roll");
                break;
            case AnimType.Attack1:
                animator.Play("Attack1");
                break;
            case AnimType.Attack2:
                animator.Play("Attack2");
                break;
            case AnimType.Attack3:
                animator.Play("Attack3");
                break;
            case AnimType.IdleBlock:
                animator.Play("IdleBlock");
                break;
            case AnimType.Block:
                animator.Play("Block");
                break;
            case AnimType.Hit:
                animator.Play("Hit");
                break;
            case AnimType.Death:
                animator.Play("Death");
                break;
        }
    }
    #endregion AnimationPlay
    #region StateType
    StateType State
    {
        get => state;
        set
        {
            if (state == value)
                return;
            Debug.Log($"StateType : {state} -> {value}");
            state = value;
        }
    }
    enum StateType
    {
        Ground,
        Jump,
        Fall,
        Run,
        WallSlide,
        Roll,
        Attack1,
        Attack2,
        Attack3,
        AttackAndHitExit,
        IdleBlock,
        Block,
        Hit,
        Death,
    }
    #endregion StateType
    #region AnimationType
    AnimType Anim
    {
        get { return anim; }
        set
        {
            if (anim == value)
                return;
            Debug.Log($"AnimType : {anim} -> {value}");
            anim = value;
            AnimationPlay();
        }
    }
    enum AnimType
    {
        Idle,
        Run,
        Fall,
        Jump,
        WallSlide,
        Roll,
        Attack1,
        Attack2,
        Attack3,
        IdleBlock,
        Block,
        Hit,
        Death,
    }
    #endregion AnimationType

    #region Methods
    void OnDestroy()
    {
        m_instance = null;
    }

    public void GetGold(int value)
    {
        gold += value;
        GoldUI.instance.AddValueText(value);
    }
    public float PlayersHPRate()
    {
        return (float)Hp / MaxHp;
    }
    public string PlayersHpText()
    {
        return $"{Hp} / {MaxHp}";
    }

    public string PlayersGold()
    {
        return $"{string.Format("{0:n0}", gold)} G";
    }
    #endregion Methods
}

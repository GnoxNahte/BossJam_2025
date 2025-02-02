using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;

public class BossEye : BossBase
{
    private enum State
    {
        DaggerPrepare,
        DaggerAttack,
        ShieldPrepare,
        ShieldAttack,
        Stunned,
        Dead,
        None, // Debug only
    }
    #region Serialized Variables
    [SerializeField] private State restrictState = State.None;
    [SerializeField] [ReadOnly] private State currState = State.DaggerPrepare;
    [SerializeField] [ReadOnly] private State nextState = State.DaggerPrepare;

    [Header("Dagger")] 
    [SerializeField] private AnimationCurve daggerSpeedCurve;
    [SerializeField] private float daggerAttackDuration;
    [SerializeField] private float daggerAttackCooldown;
    [SerializeField] private float targetDist;
    [SerializeField] private float followMoveTime;
    [SerializeField] private float followMoveTime_Hard;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxSpeed_Hard;
    // Bounds(-0.159999996,4.03999996,0,6.09000015,4.05000019,1)
    [SerializeField] private Bounds bounds;
    
    [Header("Shield")]
    [SerializeField] private float shieldAttackDuration;
    [SerializeField] private float shieldTargetDist;
    [SerializeField] private float attackFollowMoveTime;
    [SerializeField] private float attackFollowMoveTime_Hard;
    [SerializeField] private float stunnedDuration;
    [SerializeField] private float stunnedMoveDownSpeed;
    
    [Header("References")]
    [SerializeField] private DaggerCircle daggerCircle;
    [SerializeField] private Collider2D shieldCollider;
    
    [Header("Tracking")]
    [SerializeField] [ReadOnly]
    private float timeLeftInState;
    #endregion
    
    #region Private Variables

    private float _currDaggerAngle;
    private float _lastDaggerShotTime;
    private Vector2 _moveVelocity;
    private bool _stunnedHasHitFloor;
    
    // Cache references
    private Rigidbody2D _rb;
    private Animator _animator;

    private static readonly int AnimId_HasShield = Animator.StringToHash("HasShield");
    private static readonly int AnimId_IsStunned = Animator.StringToHash("IsStunned");
    private static readonly int AnimId_IsDead = Animator.StringToHash("IsDead");
    #endregion
    
    #region Public Methods
    public void Init(Player player, FillUI healthUI)
    {
        base.Init(player, healthUI);
        daggerCircle.Init(player, this);

        if (GameInitiator.IsGameCleared && GameInitiator.IsHardMode)
        {
            followMoveTime = followMoveTime_Hard;
            maxSpeed = maxSpeed_Hard;
            attackFollowMoveTime = attackFollowMoveTime_Hard;
        }
    }

    public void OnSpawnDaggersDone()
    {
        ChangeState(State.DaggerAttack);
    }

    public void OnShootingDaggersDone()
    {
        ChangeState(nextState);
    }

    public void OnShieldPrepDone()
    {
        ChangeState(nextState);
    }

    public override void TakeDamage(int damage, Vector2 position)
    {
        if (currState == State.ShieldAttack)
            return;
        
        base.TakeDamage(damage, position);
    }

    #endregion
    
    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

    }

    private void Update()
    {
        timeLeftInState -= Time.deltaTime;

        if (timeLeftInState < 0)
            ChangeState(nextState);
        
        switch (currState)
        {
            case State.DaggerAttack:
            {
                if (Time.time - _lastDaggerShotTime > daggerAttackCooldown)
                {
                    _lastDaggerShotTime = Time.time;
                    daggerCircle.ShootDagger(Player.transform);  
                } 
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        switch (currState)
        {
            case State.DaggerPrepare:
            case State.DaggerAttack:
            {
                Vector2 playerPos = Player.transform.position;
                Vector2 direction = _rb.position - playerPos;
                Vector2 targetPos = playerPos + direction.normalized * targetDist;
                // Debug.DrawRay(targetPos, Vector2.right * 0.5f, Color.red, 0.25f);
                // Debug.DrawRay(targetPos, Vector2.up * 0.5f, Color.red, 0.25f);

                // If target is outside of bounds
                if (!bounds.Contains(targetPos))
                {
                    bool ifOverX = targetPos.x < bounds.min.x || targetPos.x > bounds.max.x;
                    bool ifOverY = targetPos.y < bounds.min.y || targetPos.y > bounds.max.y;

                    targetPos = bounds.ClosestPoint(targetPos);
                    
                    // Debug.DrawRay(targetPos, Vector2.right * 0.5f, Color.blue, 0.25f);
                    // Debug.DrawRay(targetPos, Vector2.up * 0.5f, Color.blue, 0.25f);
                    // Try move to the center
                    if (ifOverX)
                    {
                        Vector2 newDir = Vector2.up * Mathf.Sign(bounds.center.y - _rb.position.y);
                        targetPos += newDir;
                    }
                    else if (ifOverY)
                    {
                        Vector2 newDir = Vector2.right * Mathf.Sign(bounds.center.x - _rb.position.x);
                        targetPos += newDir;
                    }
                }
                // Debug.DrawRay(targetPos, Vector2.right * 0.5f, Color.green, 0.25f);
                // Debug.DrawRay(targetPos, Vector2.up * 0.5f, Color.green, 0.25f);
                
                targetPos = Vector2.SmoothDamp(_rb.position, targetPos, ref _moveVelocity, followMoveTime, maxSpeed);
                _rb.MovePosition(targetPos);
                break;
            }
            case State.ShieldAttack:
            {
                Vector2 playerPos = Player.transform.position;
                Vector2 direction = playerPos - _rb.position;
                Vector2 targetPos = playerPos + direction.normalized * shieldTargetDist;
                
                targetPos = Vector2.SmoothDamp(_rb.position, targetPos, ref _moveVelocity, attackFollowMoveTime, maxSpeed);
                _rb.MovePosition(targetPos);
                break;
            }
            case State.Stunned:
            {
                if (!_stunnedHasHitFloor)
                    _rb.MovePosition(_rb.position + stunnedMoveDownSpeed * Time.deltaTime * Vector2.down);
                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (currState == State.Stunned)
            _stunnedHasHitFloor = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.color = Color.white;
    }

    #endregion
    
    #region Private Methods

    private void ChangeState(State newState)
    {
        print("Change state: " + newState);;
#if UNITY_EDITOR
        if (restrictState != State.None && 
            newState is State.DaggerPrepare or State.ShieldPrepare)
        {
            newState = restrictState;
        }
#endif

        _animator.SetBool(AnimId_HasShield, false);
        _animator.SetBool(AnimId_IsStunned, false);
        
        shieldCollider.gameObject.SetActive(false);
        
        switch (newState)
        {
            case State.DaggerPrepare:
            {
                nextState = State.DaggerAttack;
                timeLeftInState = daggerCircle.TotalDaggerSpawnTime;
                daggerCircle.SpawnDaggers();
                break;
            }
            case State.DaggerAttack:
            {
                nextState = State.ShieldPrepare;
                timeLeftInState = daggerAttackDuration;
                break;
            }
            case State.ShieldPrepare:
            {
                nextState = State.ShieldAttack;
                timeLeftInState = 3;
                
                _animator.SetBool(AnimId_HasShield, true);
                break;
            }
            case State.ShieldAttack:
            {
                nextState = State.Stunned;
                timeLeftInState = shieldAttackDuration;
                shieldCollider.gameObject.SetActive(true);
                
                break;
            }
            case State.Stunned:
            {
                _stunnedHasHitFloor = _rb.position.y < bounds.min.y;
                
                nextState = State.DaggerPrepare;
                timeLeftInState = stunnedDuration;
                _animator.SetBool(AnimId_IsStunned, true);
                break;
            }
        }
        
        currState = newState;
    }
    
    protected override void OnDead()
    {
        base.OnDead();
        
        daggerCircle.OnBossDead();
        _animator.SetBool(AnimId_IsDead, true);
        currState = nextState = State.Dead;
    }
    #endregion
}

using System;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class BossRock : BossBase
{
    public enum State
    {
        BombingPrepare,
        Bombing,
        GroundSlamPrepare,
        GroundSlamAttack,
        GroundSlamStun,
        Dead,
        None, // Debug only
    }
    #region Serialized Variables
    [SerializeField] private State restrictState = State.None;
    [SerializeField] [ReadOnly] private State currState = State.BombingPrepare;
    [SerializeField] [ReadOnly] private State nextState = State.BombingPrepare;

    [Header("General")]
    [SerializeField] private float prepareMoveTime = 0.5f;
    [SerializeField] private float prepareMoveMaxSpeed;
    [SerializeField] private Vector2 heightClamp;

    [Header("Bombing")] 
    [SerializeField] private float bombingPrepareDuration;
    [SerializeField] private float bombingDuration;
    [Range(0.0001f, 0.5f)]
    [SerializeField] private float bombingCooldown;
    [Range(1, 50)]
    [SerializeField] private int bombingStreams;
    [SerializeField] private float bombFireSpeed;
    [SerializeField] private float rotateSpeed;

    [Header("Ground Slam")]
    [SerializeField] private int groundSlamDamage;
    [SerializeField] private float[] groundSlamPrepareDuration;
    [SerializeField] private AnimationCurve groundSlamPrepareSpeedCurve;
    [SerializeField] private float groundSlamAttackFollowMaxSpeed;
    [SerializeField] private float groundSlamFlySpeed;
    [SerializeField] private float[] stunDuration;
    [Range(0f, 1f)]
    [SerializeField] private float comboChance = 0.5f;
    [SerializeField] private float bossHeight;
    
    [Header("References")]
    [SerializeField] private ObjectPool bombPool;
    [SerializeField] private SmoothHorizontalFollow horizontalFollow;
    [SerializeField] private Shockwave shockwave;
    [SerializeField] private ParticleSystem groundSlamParticles;
    
    [Header("Tracking")]
    [SerializeField] [ReadOnly]
    private float timeLeftInState;
    #endregion
    
    #region Private Variables
    private Vector2 _moveVelocity;
    private float _bombingCooldownLeft;
    private int _currShootStreamIndex;
    private float _currBombingAngle;
    private float _groundSlamTargetHeight;
    private bool _isGroundSlamFlying;
    private int _groundSlamCombo;

    private RaycastHit2D[] hits;

    // Cached references
    private Transform _levelCenter;

    private Animator _animator;
    private Rigidbody2D _rb;
    
    // AnimIds
    private static readonly int AnimId_OnHit = Animator.StringToHash("OnHit");
    private static readonly int AnimId_IsBombing = Animator.StringToHash("IsBombing");
    private static readonly int AnimId_IsGroundSlam = Animator.StringToHash("IsGroundSlam");
    private static readonly int AnimId_IsGroundSlamFlying = Animator.StringToHash("IsGroundSlamFlying");
    private static readonly int AnimId_IsDead = Animator.StringToHash("IsDead");
    private static readonly int AnimId_IsStun = Animator.StringToHash("IsStun");
    #endregion
    
    #region Public Methods
    public void Init(Player player, Transform levelCenter, FillUI healthUI)
    {
        base.Init(player, healthUI);
        _levelCenter = levelCenter;
        
        ChangeState(State.BombingPrepare);
        horizontalFollow.transform.parent = null;
        horizontalFollow.SetTarget(player.transform);
    }

    // called in anim events
    public void StartGroundSlam()
    {
        _isGroundSlamFlying = true;
        _animator.SetBool(AnimId_IsGroundSlamFlying, true);
    }

    // called in anim events
    public void OnGroundSlamHitAnimDone()
    {
        ChangeState(nextState);
        _isGroundSlamFlying = false;
        _animator.SetBool(AnimId_IsGroundSlamFlying, false);
    }
    #endregion
    
    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        hits = new RaycastHit2D[5];
        
        Debug.Assert(groundSlamPrepareDuration.Length == stunDuration.Length);
    }

    private void Update()
    {
        timeLeftInState -= Time.deltaTime;

        if (timeLeftInState < 0)
        {
            ChangeState(nextState);
        }
        else if (currState == State.Bombing)
        {
            _currBombingAngle += rotateSpeed * Time.deltaTime;
            _bombingCooldownLeft -= Time.deltaTime;
            if (_bombingCooldownLeft <= 0f)
            {
                _bombingCooldownLeft += bombingCooldown;

                FireBombs();
            }
        }
    }

    private void FixedUpdate()
    {
        switch (currState)
        {
            // Move to target
            case State.BombingPrepare:
            {
                Vector2 nextPosition = Vector2.SmoothDamp(_rb.position, _levelCenter.position, ref _moveVelocity, prepareMoveTime, prepareMoveMaxSpeed);
                _rb.MovePosition(nextPosition);
                break;
            }
            // Move to target
            case State.GroundSlamPrepare:
            {
                float maxSpeed = prepareMoveMaxSpeed *
                                 groundSlamPrepareSpeedCurve.Evaluate(1f - timeLeftInState / groundSlamPrepareDuration[_groundSlamCombo - 1]);
                Vector2 targetPosition = new Vector2(horizontalFollow.transform.position.x, heightClamp.y);
                Vector2 nextPosition = Vector2.SmoothDamp(_rb.position, targetPosition, ref _moveVelocity, prepareMoveTime, maxSpeed);
                _rb.MovePosition(nextPosition);
                break;
            }
            case State.GroundSlamAttack:
            {
                if (currState == State.GroundSlamAttack && _isGroundSlamFlying)
                {
                    Vector2 targetPosition = _rb.position;
                    targetPosition.x = Mathf.SmoothDamp(targetPosition.x, horizontalFollow.transform.position.x, ref _moveVelocity.x, prepareMoveTime, groundSlamAttackFollowMaxSpeed);
                    targetPosition.y -= groundSlamFlySpeed * Time.deltaTime;
                    _rb.MovePosition(targetPosition);
                }

                break;
            }
            // Don't do anything
            // case State.Bombing:
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlatformBase>())
        {
            if (currState == State.GroundSlamAttack)
            {
                _isGroundSlamFlying = false;
                _animator.SetBool(AnimId_IsGroundSlamFlying, false);
                Vector2 pos = _rb.position;
                pos.y = other.contacts[0].point.y + bossHeight;
                _rb.MovePosition(pos);
                shockwave.gameObject.SetActive(true);
                groundSlamParticles.Play();
                return;
            }
        }
        
        Player player = other.gameObject.GetComponent<Player>();
        if (currState == State.GroundSlamAttack && _isGroundSlamFlying && player && player.transform.position.y < transform.position.y)
        {
            player.TakeDamage(groundSlamDamage, other.contacts[0].point);
        }
    }

    #endregion
    
    #region Private Methods

    private void ChangeState(State newState)
    {
        print("Change state: " + newState);
        #if UNITY_EDITOR
        if (restrictState != State.None && 
            newState is State.BombingPrepare or State.GroundSlamPrepare)
        {
            newState = restrictState;
        }
        #endif
        _animator.SetBool(AnimId_IsBombing, false);
        _animator.SetBool(AnimId_IsGroundSlam, false);
        _animator.SetBool(AnimId_IsStun, false);
        
        switch (newState)
        {
            case State.BombingPrepare:
            {
                timeLeftInState = bombingPrepareDuration;
                nextState = State.Bombing;
                break;
            }
            case State.Bombing:
            {
                timeLeftInState = bombingDuration;
                nextState = State.GroundSlamPrepare;
                _bombingCooldownLeft = 0f;
                
                _animator.SetBool(AnimId_IsBombing, true);

                _currBombingAngle = 0f;
                break;
            }
            case State.GroundSlamPrepare:
            {
                ++_groundSlamCombo;
                timeLeftInState = groundSlamPrepareDuration[_groundSlamCombo - 1];
                nextState = State.GroundSlamAttack;
                _moveVelocity = Vector2.zero;
                break;
            }
            case State.GroundSlamAttack:
            {
                timeLeftInState = 5f;
                if (_groundSlamCombo < groundSlamPrepareDuration.Length)
                    nextState = Random.value < comboChance ? State.GroundSlamPrepare : State.GroundSlamStun;
                else
                    nextState = State.GroundSlamStun;
                
                int count = _rb.Cast(Vector2.down, hits);
                _groundSlamTargetHeight = heightClamp.x;
                // Get highest hit point
                for (int i = 0; i < count; i++)
                {
                    float height = hits[i].point.y;
                    if (height > _groundSlamTargetHeight)
                        _groundSlamTargetHeight = height;
                }

                _groundSlamTargetHeight += bossHeight;
                
                _animator.SetBool(AnimId_IsGroundSlam, true);
                _animator.SetBool(AnimId_IsGroundSlamFlying, true);
                _animator.SetBool(AnimId_IsStun, nextState == State.GroundSlamStun);
                break;
            }
            case State.GroundSlamStun:
            {
                timeLeftInState = stunDuration[_groundSlamCombo - 1];
                nextState = State.BombingPrepare;
                
                _groundSlamCombo = 0;
                _animator.SetBool(AnimId_IsStun, true);
                break;
            }
            case State.Dead:
            {
                nextState = State.Dead;
                timeLeftInState = 99999f;
                
                _animator.SetBool(AnimId_IsDead, true);
                break;
            }
        }
        
        currState = newState;
    }

    private void FireBombs()
    {
        GameObject bomb = bombPool.Get(transform.position);
        
        // Generate random initial bomb velocity
        float angle = (_currBombingAngle + _currShootStreamIndex * 360f / bombingStreams) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 initialVelocity = direction * bombFireSpeed;
        bomb.GetComponent<Bomb>().Init(initialVelocity, bombPool);
        _currShootStreamIndex = (_currShootStreamIndex + 1) % bombingStreams;

        // // Generate random initial bomb velocity
        // bool ifFireOnRight = Random.value < 0.5f;
        // float angle =  Mathf.Deg2Rad * (ifFireOnRight ? 
        //                 Random.Range(bombFireAngleRange_Right.x, bombFireAngleRange_Right.y):
        //                 Random.Range(bombFireAngleRange_Left.x, bombFireAngleRange_Left.y));
        // Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        // Vector2 initialVelocity = direction * bombFireSpeed;
        // bomb.GetComponent<Bomb>().Init(initialVelocity, bombPool);
    }

    protected override void OnDead()
    {
        base.OnDead();
        
        ChangeState(State.Dead);
    }

    #endregion
}

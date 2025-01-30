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
    [SerializeField] private float maxHeight;

    [Header("Bombing")] 
    [SerializeField] private float bombingPrepareDuration;
    [SerializeField] private float bombingDuration;
    [Range(0.0001f, 0.5f)]
    [SerializeField] private float bombingCooldown;
    [Range(1, 50)]
    [SerializeField] private int bombingStreams;
    [SerializeField] private float bombFireSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] [MinMaxSlider(0f, 360f)]
    private Vector2 bombFireAngleRange_Left;
    [SerializeField] [MinMaxSlider(0f, 360f)]
    private Vector2 bombFireAngleRange_Right;

    [Header("Ground Slam")] 
    [SerializeField] private float groundSlamPrepareDuration;
    [SerializeField] private AnimationCurve groundSlamPrepareSpeedCurve;
    [SerializeField] private float groundSlamFlySpeed;
    
    [Header("References")]
    [SerializeField] private ObjectPool bombPool;
    [SerializeField] private SmoothHorizontalFollow horizontalFollow;
    
    [Header("Tracking")]
    [SerializeField] [ReadOnly]
    private float timeLeftInState;
    #endregion
    
    #region Private Variables
    private Vector2 _moveVelocity;
    private float _bombingCooldownLeft;
    private int _currShootStreamIndex;
    private float _currBombingAngle;
    private bool _isGroundSlamFlying;

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
                _moveVelocity *= groundSlamPrepareSpeedCurve.Evaluate(1f - timeLeftInState / groundSlamPrepareDuration);
                Vector2 targetPosition = new Vector2(horizontalFollow.transform.position.x, maxHeight);
                Vector2 nextPosition = Vector2.SmoothDamp(_rb.position, targetPosition, ref _moveVelocity, prepareMoveTime, prepareMoveMaxSpeed);
                _rb.MovePosition(nextPosition);
                break;
            }
            case State.GroundSlamAttack:
            {
                if (_isGroundSlamFlying)
                    _rb.MovePosition(_rb.position + groundSlamFlySpeed * Time.deltaTime * Vector2.down);

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
            }
        }
    }

    #endregion
    
    #region Private Methods

    private void ChangeState(State newState)
    {
        #if UNITY_EDITOR
        if (restrictState != State.None && 
            newState is State.BombingPrepare or State.GroundSlamPrepare)
        {
            newState = restrictState;
        }
        #endif
        _animator.SetBool(AnimId_IsBombing, false);
        _animator.SetBool(AnimId_IsGroundSlam, false);
        
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
                timeLeftInState = groundSlamPrepareDuration;
                nextState = State.GroundSlamAttack;
                break;
            }
            case State.GroundSlamAttack:
            {
                timeLeftInState = 5f;
                nextState = Random.value < 0.5f ? State.GroundSlamPrepare : State.BombingPrepare;
                
                _animator.SetBool(AnimId_IsGroundSlam, true);
                _animator.SetBool(AnimId_IsGroundSlamFlying, true);
                break;
            }
        }
        
        currState = newState;
    }

    private void FireBombs()
    {
        GameObject bomb = bombPool.Get(transform.position);
        
        // Generate random initial bomb velocity
        bool ifFireOnRight = Random.value < 0.5f;
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
    #endregion
}

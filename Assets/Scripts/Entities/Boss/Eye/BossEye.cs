using System;
using UnityEngine;
using VInspector;

public class BossEye : BossBase
{
    private enum State
    {
        DaggerPrepare,
        DaggerAttack,
        LightningPrepare,
        LightningAtack,
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
    [SerializeField] private float maxSpeed;
    
    [Header("Lightning")]
    [SerializeField] private float lightingPrepareDuration;
    [SerializeField] private int lightningCount;
    
    [Header("References")]
    [SerializeField] private DaggerCircle daggerCircle;
    
    [Header("Tracking")]
    [SerializeField] [ReadOnly]
    private float timeLeftInState;
    #endregion
    
    #region Private Variables

    private float _currDaggerAngle;
    private float _lastDaggerShotTime;
    private Vector2 _moveVelocity;
    
    // Cache references
    private Rigidbody2D _rb;
    private Animator _animator;
    #endregion
    
    #region Public Methods
    public void Init(Player player, FillUI healthUI)
    {
        base.Init(player, healthUI);
        daggerCircle.Init(player, this);
    }

    public void OnSpawnDaggersDone()
    {
        ChangeState(State.DaggerAttack);
    }

    public void OnShootingDaggersDone()
    {
        return;
        ChangeState(nextState);
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
                // Debug.DrawLine(transform.position, targetPos, Color.red, 0.5f);
                // Debug.DrawLine(transform.position, direction, Color.green, 0.5f);
                Debug.DrawRay(targetPos, Vector2.right, Color.red, 0.5f);
                Debug.DrawRay(targetPos, Vector2.up, Color.red, 0.5f);
                targetPos = Vector2.SmoothDamp(_rb.position, targetPos, ref _moveVelocity, followMoveTime, maxSpeed);
                _rb.MovePosition(targetPos);
                break;
            }
        }
    }

    #endregion
    
    #region Private Methods

    private void ChangeState(State newState)
    {
        print("Change state: " + newState);;
#if UNITY_EDITOR
        if (restrictState != State.None && 
            newState is State.DaggerPrepare or State.LightningPrepare)
        {
            newState = restrictState;
        }
#endif

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
                nextState = State.DaggerPrepare;
                timeLeftInState = daggerAttackDuration;
                break;
            }
            case State.LightningPrepare:
            {
                nextState = State.LightningAtack;
                timeLeftInState = lightingPrepareDuration;
                
                break;
            }
        }
        
        currState = newState;
    }
    #endregion
}

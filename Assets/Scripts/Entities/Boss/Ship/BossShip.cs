using System;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class BossShip : BossBase
{
    public enum State
    {
        Preparing,
        Bombing,
        Firing,
        Dead,
    }
    
    #region Serialized Variables
    [SerializeField] [ReadOnly] private State currState = State.Preparing;
    [SerializeField] [ReadOnly] private State nextState = State.Preparing;
    [Header("Preparing")]
    [SerializeField] private float prepareTime = 1.5f;
    [SerializeField] private GameObject attackWarningPrefab;
    [Header("Attacking - Bombing")] 
    [SerializeField] private Transform bombingSpawnPos;
    [SerializeField] private float bombingMoveSpeed;
    [SerializeField] private float bombingCooldown;
    [SerializeField] private Vector2 bombingReleaseVelocity;

    [Header("Attacking - Fire")] 
    [SerializeField] private GameObject fire;
    [SerializeField] private float firingCooldown;
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject lighting;

    [SerializeField] private ObjectPool bombPool;
    #endregion
    
    #region Private Variables

    private bool _isMovingRight;
    private float _prepareTimeLeft;
    private AttackWarningFlashing _attackWarning;
    
    private float _attackCooldownLeft;

    private bool _isInitDone = false;
    
    private GameObject _leftBorder, _rightBorder;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    
    // AnimIds
    private static readonly int AnimId_OnHit = Animator.StringToHash("OnHit");
    private static readonly int AnimId_IsBombing = Animator.StringToHash("IsBombing");
    private static readonly int AnimId_IsFiring = Animator.StringToHash("IsFiring");

    #endregion

    #region Public Methods

    public void Init(Player player, GameObject leftBorder, GameObject rightBorder, FillUI healthUI)
    {
        base.Init(player);
        print("Init boss ship");   
        _attackWarning.follow.SetTargetAndPosition(Player.TargetShipBombing);
        ChangeState(State.Preparing);
        
        _leftBorder = leftBorder;
        _rightBorder = rightBorder;
        
        _isInitDone = true;
        
        Health.LinkFillUI(healthUI);
    }

    #endregion
    
    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();
        
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
        Camera gameCamera = Camera.main;
        GameObject attackWarningGameObj = Instantiate(attackWarningPrefab, gameCamera?.transform);
        _attackWarning = attackWarningGameObj.GetComponent<AttackWarningFlashing>();

    }

    private void Start()
    {
        bombPool.transform.SetParent(null);
    }

    private void Update()
    {
        if (currState is State.Bombing)
        {
            _attackCooldownLeft -= Time.deltaTime;
            if (_attackCooldownLeft <= 0f)
            {
                _attackCooldownLeft += bombingCooldown;
                
                GameObject bomb = bombPool.Get(bombingSpawnPos.position);
                bomb.GetComponent<Bomb>().Init(bombingReleaseVelocity, bombPool);
            }
        }
    }

    private void FixedUpdate()
    {
        switch (currState)
        {
            case State.Preparing:
            {
                _prepareTimeLeft -= Time.deltaTime;
                _attackWarning.SetTimeLeftPercentage(_prepareTimeLeft / prepareTime);
                if (_prepareTimeLeft <= 0)
                {
                    // ChangeState(State.Firing);
                    // ChangeState(Random.value < 0.5f ? State.Bombing : State.Firing);
                    ChangeState(nextState);
                }
                break;
            }
            case State.Bombing:
            case State.Firing:
                Vector2 displacement = bombingMoveSpeed * Time.fixedDeltaTime * (_isMovingRight ? 1f : -1f) * Vector2.right;
                _rb.MovePosition(_rb.position + displacement);
                break;
        }
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Player player = other.gameObject.GetComponent<Player>();
    //     if (player != null)
    //     {
    //         _animator.SetTrigger(AnimId_OnHit);
    //         Health.TakeDamage(player.SpinAttackDamage);
    //         return;
    //     }
    // }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     Player player = other.gameObject.GetComponent<Player>();
    //     if (player != null)
    //     {
    //         _animator.SetTrigger(AnimId_OnHit);
    //         TakeDamage(player.SpinAttackDamage, other.contacts[0].point);
    //     }
    // }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CameraBorders") && currState is State.Bombing or State.Firing)
        {
            ChangeState(State.Preparing);
        }
    }

    #endregion
    
    #region Private Methods

    private void ChangeState(State newState)
    {
#if UNITY_EDITOR
        if (_isInitDone)
        {
            // print("Changing state: " + currState + " -> " + newState);
            Debug.Assert(newState != currState, "Setting next state to the same current state: " + currState);
        }  
#endif
        
        _attackCooldownLeft = -1f;
        _animator.SetBool(AnimId_IsBombing, false);
        _animator.SetBool(AnimId_IsFiring, false);
        
        switch (newState)
        {
            case State.Preparing: 
                _prepareTimeLeft = prepareTime;
                nextState = Random.value < 0.5f ? State.Bombing : State.Firing;
                _attackWarning.follow.SetTargetAndPosition(nextState == State.Bombing ? Player.TargetShipBombing : Player.TargetCenter);
                break;
            case State.Bombing:
            case State.Firing:
                _isMovingRight = Random.value > 0.5f;
                _leftBorder.SetActive(!_isMovingRight);
                _rightBorder.SetActive(_isMovingRight);
                // _rb.simulated = false;
                transform.localScale = new Vector3(_isMovingRight ? -1f : 1f, transform.localScale.y, transform.localScale.z);
                Vector2 pos = new Vector2(Camera.main.ViewportToWorldPoint(Vector2.right * (_isMovingRight ? -0.2f : 1.2f)).x,
                    _attackWarning.transform.position.y);
                // _rb.MovePosition(pos);
                transform.position = pos;
                // _rb.simulated = true;
                
                fire.gameObject.SetActive(newState == State.Firing);

                if (newState == State.Bombing)
                {
                    _attackCooldownLeft = bombingCooldown;
                    _animator.SetBool(AnimId_IsBombing, true);
                    // _attackWarning.SetTargetAndPosition(Player.TargetShipBombing);
                }
                else
                {
                    _attackCooldownLeft = firingCooldown;
                    _animator.SetBool(AnimId_IsFiring, true);
                    // _attackWarning.SetTargetAndPosition(Player.TargetCenter);
                }
                break;
        }
        
        bool isPreparing = newState == State.Preparing;
        sr.enabled = !isPreparing;
        lighting.SetActive(!isPreparing);
        _rb.simulated = !isPreparing;
        _attackWarning.gameObject.SetActive(isPreparing);
        currState = newState;
    }
    #endregion
}

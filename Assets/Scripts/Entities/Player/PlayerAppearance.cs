using System;
using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{

    #region Serialized Variables

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Transform attackParent;
    #endregion
    
    #region Private Variables
	private Animator _animator;
    private PlayerMovement _playerMovement;
    
    // Anim IDs
    // TODO: Consider calling animation states directly instead of using Triggers
    private static readonly int AnimId_InAir = Animator.StringToHash("InAir");
    private static readonly int AnimId_IsSpinning = Animator.StringToHash("IsSpinning");
    private static readonly int AnimId_IsFacingUp = Animator.StringToHash("IsFacingUp");
    private static readonly int AnimId_IsDead = Animator.StringToHash("IsDead");
    private static readonly int AnimId_DirectionX = Animator.StringToHash("DirectionX");
    private static readonly int AnimId_DirectionY = Animator.StringToHash("DirectionY");
    private static readonly int AnimId_OnHit = Animator.StringToHash("OnHit");
    private static readonly int AnimId_OnDamage = Animator.StringToHash("OnDamage");
    private static readonly int AnimId_OnAttack = Animator.StringToHash("OnAttack");
    #endregion
    
    #region Public Methods
    // ReSharper triggers for Debug.LogError
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnActivateAbility(PlayerAbilitySystem.Type type)
    {
        switch (type)
        {
            case PlayerAbilitySystem.Type.Attack: _animator.SetTrigger(AnimId_OnAttack); break;
            // case PlayerAbilitySystem.Type.Dash: _animator.SetTrigger(AnimID_OnDash); break;
            // case PlayerAbilitySystem.Type.SpinCharge: _animator.SetTrigger(AnimId_OnSpinCharge); break;
            // case PlayerAbilitySystem.Type.Spin: _animator.SetTrigger(AnimId_OnSpin); break;
            // default: Debug.LogError("Player Animator: Activating unknown ability - " + type); break;
        }
    }
    
    public void OnAbilityEnd(PlayerAbilitySystem.Type type)
    {
        // switch (type)
        // {
        //     case PlayerAbilitySystem.Type.Spin: _animator.SetTrigger(AnimID_OnSpinEnd); break;
        //     // default: Debug.LogError("Player Animator: Activating unknown ability - " + type); break;
        // }
    }

    public void Init(PlayerMovement playerMovement, Health health)
    {
        _playerMovement = playerMovement;
        
        OnDisable();
        OnEnable();
    }

    private void OnEnable()
    {
        if (_playerMovement)
            _playerMovement.OnHit += OnHit;
    }

    private void OnDisable()
    {
        if (_playerMovement)
            _playerMovement.OnHit -= OnHit; 
    }

    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // float angle = Mathf.Atan2(_playerMovement.FacingDirection.y, _playerMovement.FacingDirection.x) * Mathf.Rad2Deg;
        // sr.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        bool ifFacingLeft = _playerMovement.FacingDirection.x < 0;
        Vector3 newScale = new Vector3(ifFacingLeft ? 1f : -1f, 1f, 1f);
        sr.flipX = ifFacingLeft;
        // attackParent.localScale = newScale;
        // attackParent.localPosition = new Vector3(
        //     (_playerMovement.FacingDirection.x > 0 ? 1f : -1f) * Mathf.Abs(attackParent.localPosition.x),
        //     attackParent.localPosition.y,
        //     0f);
        
        _animator.SetBool(AnimId_InAir, _playerMovement.IsInAir);
        _animator.SetBool(AnimId_IsSpinning, _playerMovement.IsSpinning);
        _animator.SetBool(AnimId_IsDead, _playerMovement.IsDead);
        _animator.SetBool(AnimId_IsFacingUp, _playerMovement.FacingDirection.y > 0);
        
        _animator.SetFloat(AnimId_DirectionX, _playerMovement.Velocity.x);
        _animator.SetFloat(AnimId_DirectionY, _playerMovement.Velocity.y);
    }

    #endregion
    
    #region Private Methods
    private void OnDeath()
    {
        _animator.SetBool(AnimId_IsDead, _playerMovement.IsDead);
    }

    private void OnHit()
    {
        _animator.SetTrigger(AnimId_OnHit);
    }
    #endregion
}

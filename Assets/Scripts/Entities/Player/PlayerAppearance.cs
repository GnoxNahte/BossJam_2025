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
    private readonly int _onAttackAnimID = Animator.StringToHash("OnAttack");
    private readonly int _onDashAnimID = Animator.StringToHash("OnDash");
    private readonly int _onSpinChargeAnimID = Animator.StringToHash("OnSpinCharge");
    private readonly int _onSpinAnimID = Animator.StringToHash("OnSpin");
    private readonly int _onSpinEndId = Animator.StringToHash("OnSpinEnd");
    #endregion
    
    #region Public Methods
    // ReSharper triggers for Debug.LogError
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnActivateAbility(PlayerAbilitySystem.Type type)
    {
        switch (type)
        {
            case PlayerAbilitySystem.Type.Attack: _animator.SetTrigger(_onAttackAnimID); break;
            case PlayerAbilitySystem.Type.Dash: _animator.SetTrigger(_onDashAnimID); break;
            case PlayerAbilitySystem.Type.SpinCharge: _animator.SetTrigger(_onSpinChargeAnimID); break;
            case PlayerAbilitySystem.Type.Spin: _animator.SetTrigger(_onSpinAnimID); break;
            default: Debug.LogError("Player Animator: Activating unknown ability - " + type); break;
        }
    }
    
    public void OnAbilityEnd(PlayerAbilitySystem.Type type)
    {
        switch (type)
        {
            case PlayerAbilitySystem.Type.Spin: _animator.SetTrigger(_onSpinEndId); break;
            // default: Debug.LogError("Player Animator: Activating unknown ability - " + type); break;
        }
    }

    public void Init(PlayerMovement playerMovement)
    {
        _playerMovement = playerMovement;
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
        attackParent.localScale = newScale;
        attackParent.localPosition = new Vector3(
            (_playerMovement.FacingDirection.x > 0 ? 1f : -1f) * Mathf.Abs(attackParent.localPosition.x),
            attackParent.localPosition.y,
            0f);
    }

    #endregion
    
    #region Private Methods
    
    #endregion
}

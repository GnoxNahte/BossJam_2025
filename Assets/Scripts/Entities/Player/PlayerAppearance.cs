using System;
using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] private float invincibilityAlpha;
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
    private static readonly int AnimId_IsBossDefeated = Animator.StringToHash("IsBossDefeated");
    private static readonly int AnimId_OnHitAttack = Animator.StringToHash("OnHitAttack");
    #endregion
    
    #region Public Methods
    public void Init(PlayerMovement playerMovement, Health health)
    {
        _playerMovement = playerMovement;
        
        OnDisable();
        OnEnable();
    }

    public void OnInvincibilityChange(bool isInvincible)
    {
        Color color = sr.color;
        color.a = isInvincible ? invincibilityAlpha : 1f;
        sr.color = color;
    }

    public void OnHitAttack()
    {
        _animator.SetTrigger(AnimId_OnHitAttack);
    }
    #endregion
    
    #region Unity Methods
    
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

    private void OnHit(int damage, Vector2 position)
    {
        _animator.SetTrigger(AnimId_OnHit);
    }
    #endregion

    public void OnDefeatBoss()
    {
        _animator.SetBool(AnimId_IsBossDefeated, true);
    }
}

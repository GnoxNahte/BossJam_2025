using System;
using UnityEngine;

public class Player : EntityBase
{
    #region Public Variables
    public const string PlayerTag = "Player";
    
    public PlayerMovement PlayerMovement => _playerMovement;
    public PlayerAppearance PlayerAppearance => _playerAppearance;
    #endregion
    
    #region Serialized Variables

    [field: SerializeField] public Transform TargetCenter { get; private set; }
    [field: SerializeField] public Transform TargetShipBombing { get; private set; }
    [field: SerializeField] public Transform GemTarget { get; private set; }
    #endregion
    
    #region Private Variables

    private PlayerMovement _playerMovement;
    private PlayerAppearance _playerAppearance;
    private GameUIManager _gameUIManager;

    private bool _hasDefeatedBoss = false;
    
    #endregion
    
    #region Public Methods
    
    public void Init(InputManager input, GameUIManager gameUIManager)
    {
        _playerMovement.Init(input);
        _playerAppearance.Init(_playerMovement, Health);
        
        _gameUIManager = gameUIManager;
        Health.LinkFillUI(gameUIManager.PlayerHealthUI);
    }
    
    // Called in Animation events
    public void OnDeathAnimDone()
    {
        _gameUIManager.OnDeath();
    }

    public void OnDefeatBoss()
    {
        _playerAppearance.OnDefeatBoss();
        _playerMovement.OnGemGrab();
        _gameUIManager.OnBossDefeated();

        _hasDefeatedBoss = true;
    }

    public override void TakeDamage(int damage, Vector2 position)
    {
        base.TakeDamage(damage, position);
        
        AudioManager.PlaySFX(AudioManager.SFX.PlayerTakeDamage);
    }

    #endregion
    
    #region Unity Methods

    private void OnEnable()
    {
        _playerMovement.OnHit += TakeDamage;
    }

    private void OnDisable()
    {
        _playerMovement.OnHit -= TakeDamage;
    }

    protected override void Awake()
    {
        base.Awake();
        
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAppearance = GetComponent<PlayerAppearance>();
    }

    #endregion
    
    #region Private/Protected Methods

    protected override void OnDead()
    {
        if (_playerMovement.IsDead || _hasDefeatedBoss)
            return;
        
        base.OnDead();

        _gameUIManager.OnDeath();
        _playerMovement.OnDeath();
    }

    #endregion
}

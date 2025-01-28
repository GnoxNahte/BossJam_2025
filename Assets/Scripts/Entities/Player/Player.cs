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
    #endregion
    
    #region Private Variables

    private PlayerMovement _playerMovement;
    private PlayerAppearance _playerAppearance;
    
    #endregion
    
    #region Public Methods
    
    public void Init(InputManager input, FillUI playerHealthUI)
    {
        _playerMovement.Init(input);
        _playerAppearance.Init(_playerMovement, Health);
        
        Health.LinkFillUI(playerHealthUI);
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
    
    #region Private Methods
    
    #endregion
}

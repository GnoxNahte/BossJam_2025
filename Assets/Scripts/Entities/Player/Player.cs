using UnityEngine;

public class Player : EntityBase
{
    #region Public Variables
    public const string PlayerTag = "Player";
    
    public PlayerMovement PlayerMovement => _playerMovement;
    public PlayerAppearance PlayerAppearance => _playerAppearance;
    public PlayerAbilitySystem PlayerAbilitySystem => _playerAbilitySystem;
    #endregion
    
    #region Serialized Variables

    [field: SerializeField] public int SpinAttackDamage { get; private set; }
    [field: SerializeField] public Transform TargetCenter { get; private set; }
    [field: SerializeField] public Transform TargetShipBombing { get; private set; }
    #endregion
    
    #region Private Variables

    private PlayerMovement _playerMovement;
    private PlayerAppearance _playerAppearance;
    private PlayerAbilitySystem _playerAbilitySystem;
    
    #endregion
    
    #region Public Methods
    
    public void Init(InputManager input)
    {
        _playerMovement.Init(_playerAbilitySystem, input);
        _playerAbilitySystem.Init(this);
        _playerAppearance.Init(_playerMovement, Health);
    }

    public void TakeDamage(int damage)
    {
        Health.TakeDamage(damage);
    }
    #endregion
    
    #region Unity Methods

    protected override void Awake()
    {
        base.Awake();
        
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAppearance = GetComponent<PlayerAppearance>();
        _playerAbilitySystem = GetComponent<PlayerAbilitySystem>();
    }

    #endregion
    
    #region Private Methods
    
    #endregion
}

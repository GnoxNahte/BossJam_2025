public class Player : EntityBase
{
    #region Public Variables
    public const string PlayerTag = "Player";
    
    public PlayerMovement PlayerMovement => _playerMovement;
    public PlayerAppearance PlayerAppearance => _playerAppearance;
    public PlayerAbilitySystem PlayerAbilitySystem => _playerAbilitySystem;
    #endregion
    
    #region Serialized Variables
    
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
    }
    #endregion
    
    #region Unity Methods

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAppearance = GetComponent<PlayerAppearance>();
        _playerAbilitySystem = GetComponent<PlayerAbilitySystem>();
    }

    #endregion
    
    #region Private Methods
    
    #endregion
}

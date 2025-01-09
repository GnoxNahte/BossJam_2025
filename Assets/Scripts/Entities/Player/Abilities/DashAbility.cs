public class DashAbility : PlayerAbilityBase
{
    #region Private Variables
    private PlayerMovement _playerMovement; // cache
    #endregion

    #region Public Methods
    public override void Init(Player player)
    {
        _playerMovement = player.PlayerMovement;
    }
    #endregion
    
    #region Unity Methods
    protected override void OnValidate()
    {
        Name = "Dash";
        Type = PlayerAbilitySystem.Type.Dash;
    }
    #endregion

    // NOTE: canceledAbility can be null
    protected override void Activate(AbilityBase<PlayerAbilitySystem.Type> canceledAbility)
    {
        base.Activate(canceledAbility);
        _playerMovement.Dash();
    }

    public override void CancelAbility(PlayerAbilitySystem.Type nextAbility)
    {
        _playerMovement.CancelDash();
    }
}

using UnityEngine;

public class SpinChargeAbility : PlayerAbilityBase
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
        Name = "Spin - Charge";
        Type = PlayerAbilitySystem.Type.SpinCharge;
    }
    #endregion

    // NOTE: canceledAbility can be null
    protected override void Activate(AbilityBase<PlayerAbilitySystem.Type> canceledAbility)
    {
        base.Activate(canceledAbility);
        _playerMovement.ChargeSpin();
    }

    protected override bool CanUseAbility()
    {
        return _playerMovement.IsGrounded();
    }
}

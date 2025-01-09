using UnityEngine;

public class AttackAbility : PlayerAbilityBase
{
    #region Public Variables
    
    #endregion
    
    #region Serialized Variables
    
    #endregion
    
    #region Private Variables
	PlayerAppearance _playerAppearance;
    #endregion
    
    #region Public Methods
    public override void Init(Player player)
    {
        _playerAppearance = player.PlayerAppearance;
    }
    
    public override void CancelAbility(PlayerAbilitySystem.Type nextAbility)
    {
        print("Canceled attack");
    }

    public override void OnEnd()
    {
        base.OnEnd();
        
        _playerAppearance.SetColor(Color.white);
    }

    #endregion
    
    #region Unity Methods
    
    protected override void OnValidate()
    {
        Type = PlayerAbilitySystem.Type.Attack;
        Name = "Attack";
    }
    
    #endregion
    
    #region Private Methods
    
    // NOTE: canceledAbility can be null
    protected override void Activate(AbilityBase<PlayerAbilitySystem.Type> canceledAbility)
    {
        base.Activate(canceledAbility);
        if (canceledAbility)
            _playerAppearance.SetColor(Color.red);
    }
    
    #endregion
}

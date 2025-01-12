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

    #endregion
    
    #region Unity Methods
    
    protected override void OnValidate()
    {
        Type = PlayerAbilitySystem.Type.Attack;
        Name = "Attack";
    }
    
    #endregion
    
    #region Private Methods
    
    #endregion
}

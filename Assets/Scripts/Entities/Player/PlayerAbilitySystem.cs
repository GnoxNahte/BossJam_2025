using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class PlayerAbilitySystem : AbilitySystem
{
    public enum Type
    {
        Dash,
        Attack,
    }
    
    [SerializeField] [ReadOnly]
    private PlayerAbilityBase currAbility;

    private PlayerAppearance _playerAppearance;
    
    #region Public Methods
    public void Init(Player player)
    {
        _playerAppearance = player.PlayerAppearance;
        
        foreach (var ability in Abilities)
            ability.Value.Init(player);
    }

    public bool TryActivateAbility(Type type)
    {
        PlayerAbilityBase nextAbility = Abilities[type]; 
        Debug.Assert(nextAbility, "nextAbility == null");
        
        // Reject if current ability is still active AND
        // Next ability cannot cancel current ability
        if (currAbility && currAbility.IsActive && !nextAbility.IsCancelableFromAbility(currAbility.Type))
            return false;

        // Don't activate this ability if cannot activate
        bool isAbleToActivate = nextAbility.TryActivate(currAbility);
        if (!isAbleToActivate)
            return false;

        if (currAbility)
            currAbility.CancelAbility(nextAbility.Type);
        
        currAbility = nextAbility;
        _playerAppearance.OnActivateAbility(type);
        return true;
    }

    public void OnAbilityEnd()
    {
        currAbility.OnEnd();
        currAbility = null;
    }
    
    [Button]
    public void GetAbilities()
    {
        PlayerAbilityBase[] abilities = GetComponentsInChildren<PlayerAbilityBase>();
        foreach (PlayerAbilityBase ability in abilities)
        {
            Abilities.Add(ability.Type, ability);
        }
    }
    #endregion
}

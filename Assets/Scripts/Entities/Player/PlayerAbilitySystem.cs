using UnityEngine;
using VInspector;

public class PlayerAbilitySystem : AbilitySystem
{
    public enum Type
    {
        Dash,
        Attack,
        SpinCharge,
        Spin,
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
        // print("Activate Ability: " + type);
        
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
        {
            currAbility.CancelAbility(nextAbility.Type);
            OnAbilityEnd(currAbility.Type);
        }
        
        currAbility = nextAbility;
        _playerAppearance.OnActivateAbility(type);
        return true;
    }

    // Pass the type to confirm it's referring to the correct ability
    public void OnAbilityEnd(Type type)
    {
        // print("OnAbilityEnd: " + type);

        if (!currAbility)
            return;
        
        currAbility.OnEnd(false);
        
        _playerAppearance.OnAbilityEnd(type);

        if (currAbility.ChainAbility && currAbility.ChainAbility.TryActivate(null))
        {
            currAbility = currAbility.ChainAbility as PlayerAbilityBase;
            _playerAppearance.OnActivateAbility(currAbility.Type);
        }
        else
        {
            currAbility = null;
        }
    }
    
    [Button]
    public void GetAbilities()
    {
        Abilities.Clear();
        PlayerAbilityBase[] abilities = GetComponentsInChildren<PlayerAbilityBase>();
        foreach (PlayerAbilityBase ability in abilities)
        {
            Abilities.Add(ability.Type, ability);
        }
    }
    #endregion
}


using UnityEngine;
using VInspector;

public abstract class PlayerAbilityBase : AbilityBase<PlayerAbilitySystem.Type>
{
    [field: SerializeField, ReadOnly] public bool IfShowInUI { get; protected set; } = true;
    
    public virtual void Init(Player player) {}
}

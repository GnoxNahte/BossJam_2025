using UnityEngine;

public abstract class BossBase : EntityBase
{
    protected Player Player;
    [field: SerializeField] public Vector2 PlayerKnockbackSpeed { get; private set; }
    
    public virtual void Init(Player player)
    {
        Player = player;
    }
}

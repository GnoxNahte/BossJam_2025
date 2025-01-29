using UnityEngine;

public abstract class BossBase : EntityBase
{
    protected Player Player;
    public virtual void Init(Player player)
    {
        Player = player;
    }

    protected override void OnDead()
    {
        base.OnDead();
        Player.OnDefeatBoss();
    }
}

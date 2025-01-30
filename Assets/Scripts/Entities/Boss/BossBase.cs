using UnityEngine;

public abstract class BossBase : EntityBase
{
    protected Player Player;
    public virtual void Init(Player player, FillUI healthUI)
    {
        Player = player;
        
        Health.LinkFillUI(healthUI);
    }

    protected override void OnDead()
    {
        base.OnDead();
        Player.OnDefeatBoss();
    }
}

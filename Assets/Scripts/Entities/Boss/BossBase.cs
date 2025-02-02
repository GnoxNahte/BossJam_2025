using UnityEngine;

public abstract class BossBase : EntityBase
{
    [SerializeField] private int hardHealth;
    protected Player Player;
    public virtual void Init(Player player, FillUI healthUI)
    {
        Player = player;
        

        if (GameInitiator.IsGameCleared && GameInitiator.IsHardMode)
        {
            Health.SetMaxHealth(hardHealth);
        }
        
        Health.LinkFillUI(healthUI);
    }

    protected override void OnDead()
    {
        base.OnDead();
        Player.OnDefeatBoss();
    }
}

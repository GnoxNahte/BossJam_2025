using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EntityBase : MonoBehaviour
{
    protected Health Health;

    [field: SerializeField] public Vector2 KnockbackSpeed { get; private set; }

    private bool _isPlayer;

    public void TakeDamage(int damage, Vector2 position)
    {
        Health.TakeDamage(damage);
        DamageTextManager.OnDamage(damage, position, _isPlayer);
    }

    protected virtual void OnDead()
    {
        print(name + " is Dead");
    }
    
    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
        _isPlayer = GetComponent<Player>() != null;
        
        Health.OnDeath += OnDead;
    }
}
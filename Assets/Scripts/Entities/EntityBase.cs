using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EntityBase : MonoBehaviour
{
    protected Health Health;

    public void TakeDamage(int damage)
    {
        Health.TakeDamage(damage);
    }
    
    protected virtual void Awake()
    {
        Health = GetComponent<Health>();
    }
}
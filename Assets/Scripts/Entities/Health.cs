using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [field: SerializeField] public int Amt { get; private set; } = 100;

    public Action OnDeath;
    
    public void TakeDamage(int damage)
    {
        Amt -= damage;
        
        if (Amt <= 0)
            OnDeath?.Invoke();
    }
}

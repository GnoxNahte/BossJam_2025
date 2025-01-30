using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [field: SerializeField] public int CurrHealth { get; private set; }

    private FillUI _fillUI;
    
    public Action OnDeath;

    private void Start()
    {
        CurrHealth = maxHealth;
    }

    public void LinkFillUI(FillUI fillUI)
    {
        _fillUI = fillUI;
        _fillUI.Init(maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        CurrHealth = Mathf.Clamp(CurrHealth - damage, 0, maxHealth);

        _fillUI?.UpdateFill(CurrHealth);
        
        if (CurrHealth <= 0)
            OnDeath?.Invoke();
    }
}

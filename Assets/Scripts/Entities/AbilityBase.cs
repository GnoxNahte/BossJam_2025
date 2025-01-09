using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using VInspector;

public abstract class AbilityBase<T> : MonoBehaviour where T : System.Enum
{
    #region Public Variables

    [field: SerializeField] public string Name { get; protected set; }
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public float Cooldown { get; protected set; }

    // Only can cancel abilities from these types. 
    // Cancel ability from type -> activate this ability
    [field: SerializeField] public PlayerAbilitySystem.Type[] CancelFromTypes { get; protected set; }

    [Header("Tracking variables")]
    [field: SerializeField] [field: ReadOnly]
    public T Type { get; protected set; }
    
    [field: SerializeField] [field: ReadOnly]
    public float CooldownTimeLeft { get; private set; }

    [field: SerializeField] [field: ReadOnly]
    public bool IsActive { get; private set; }
    
    public Action OnCooldownTimeLeftChanged;
    
    #endregion

    #region Private Variables

    private Coroutine _cooldownCountdownCoroutine;

    #endregion

    #region Public Methods

    // NOTE: canceledAbility can be null
    public bool TryActivate(AbilityBase<T> canceledAbility)
    {
        if (CooldownTimeLeft > 0f) 
            return false;
        
        Debug.Assert(_cooldownCountdownCoroutine == null, "Ability Cooldown countdown coroutine != null");
        Activate(canceledAbility);
        
        return true;
    }

    // Called when:
    // - Ability Ended (Without cancelling)
    // - Ability Cancelled
    public virtual void OnEnd()
    {
        IsActive = false;
    }

    public bool IsCancelableFromAbility(PlayerAbilitySystem.Type previousAbility)
    {
        return CancelFromTypes.Contains(previousAbility);
    }

    public abstract void CancelAbility(PlayerAbilitySystem.Type nextAbility);

    #endregion

    #region Protected Variables

    // NOTE: canceledAbility can be null
    protected virtual void Activate(AbilityBase<T> canceledAbility)
    {
        CooldownTimeLeft = Cooldown;
        IsActive = true;
        
        _cooldownCountdownCoroutine = StartCoroutine(CooldownCountdown());
    }
    
    // Set type and name here
    protected abstract void OnValidate();

    protected virtual IEnumerator CooldownCountdown()
    {
        while (CooldownTimeLeft > 0f)
        {
            yield return null;
            CooldownTimeLeft -= Time.deltaTime;
            OnCooldownTimeLeftChanged.Invoke();
        }

        CooldownTimeLeft = -1f;
        _cooldownCountdownCoroutine = null;
    }

    #endregion
}

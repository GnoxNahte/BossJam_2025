using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private GameObject abilityUIPrefab;
    [SerializeField] private Transform abilityUIParent;

    [field: SerializeField] public FillUI BossHealthUI { get; private set; }
    [field: SerializeField] public FillUI PlayerHealthUI { get; private set; }
    #endregion
    
    #region Public Methods
    public void Init(PlayerAbilitySystem playerAbilitySystem)
    {
        foreach (var ability in playerAbilitySystem.Abilities)
        {
            if (!ability.Value.IfShowInUI)
                continue;
            
            GameObject abilityUI_GO = Instantiate(abilityUIPrefab, abilityUIParent);
            AbilityUI abilityUI = abilityUI_GO.GetComponent<AbilityUI>();

            abilityUI.Init(ability.Value);
        }
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

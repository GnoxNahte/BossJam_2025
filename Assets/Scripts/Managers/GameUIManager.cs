using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private GameObject abilityUIPrefab;
    [SerializeField] private Transform abilityUIParent;
    #endregion
    
    #region Public Methods
    public void Init(PlayerAbilitySystem playerAbilitySystem)
    {
        foreach (var ability in playerAbilitySystem.Abilities)
        {
            GameObject abilityUI_GO = Instantiate(abilityUIPrefab, abilityUIParent);
            AbilityUI abilityUI = abilityUI_GO.GetComponent<AbilityUI>();

            abilityUI.Init(ability.Value);
        }
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

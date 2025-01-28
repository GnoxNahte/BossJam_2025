using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    #region Serialized Variables
    [field: SerializeField] public FillUI BossHealthUI { get; private set; }
    [field: SerializeField] public FillUI PlayerHealthUI { get; private set; }
    #endregion
    
    #region Public Methods
    public void Init(bool isMainMenuLoaded)
    {
        if (isMainMenuLoaded)
        {
            BossHealthUI.Disable();
            PlayerHealthUI.Disable();
        }
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

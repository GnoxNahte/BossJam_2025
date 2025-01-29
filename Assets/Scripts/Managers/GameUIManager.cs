using System.Collections;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    #region Serialized Variables
    [field: SerializeField] public FillUI BossHealthUI { get; private set; }
    [field: SerializeField] public FillUI PlayerHealthUI { get; private set; }
    
    [SerializeField] private TransitionToGame restartGameTransition;

    [SerializeField] private float fadeAnimDuration;
    [SerializeField] private CanvasGroup deathScreen;

    private Coroutine _currAnim;
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

    public void OnDeath()
    {
        Debug.Assert(_currAnim == null);
        _currAnim = StartCoroutine(FadeDeathScreen());
    }

    // UI button event
    public void OnRestartClicked()
    {
        restartGameTransition.TriggerTransition();
    }
    #endregion
    
    #region Private Methods

    private IEnumerator FadeDeathScreen()
    {
        float currAnimTime = 0f;
        deathScreen.alpha = 0f;
        deathScreen.gameObject.SetActive(true);

        while (currAnimTime < fadeAnimDuration)
        {
            print("Curr: " + currAnimTime + " / " + fadeAnimDuration);
            currAnimTime += Time.deltaTime;
            
            deathScreen.alpha = currAnimTime / fadeAnimDuration;
            
            yield return null;
        }
        
        deathScreen.alpha = 1f;

        _currAnim = null;
    }
    #endregion
}

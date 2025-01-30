using System.Collections;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    #region Serialized Variables
    [field: SerializeField] public FillUI BossHealthUI { get; private set; }
    [field: SerializeField] public FillUI PlayerHealthUI { get; private set; }
    
    [SerializeField] private TransitionToGame restartGameTransition;
    [SerializeField] private TransitionToGame nextGameTransition;

    [SerializeField] private float fadeAnimDuration;
    [SerializeField] private CanvasGroup deathScreen;

    private Coroutine _currAnim;
    #endregion
    
    #region Public Methods
    public void Init(bool isMainMenuLoaded, string currSceneName, string nextSceneName)
    {
        if (isMainMenuLoaded)
        {
            BossHealthUI.Disable();
            PlayerHealthUI.Disable();
        }
        
        restartGameTransition.SetTransitionSceneName(currSceneName);
        nextGameTransition.SetTransitionSceneName(nextSceneName);
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

    public void OnBossDefeated()
    {
        print("Defated");
        StartCoroutine(WaitWinTransition());
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

    private IEnumerator WaitWinTransition()
    {
        print("wait Defated");
        yield return new WaitForSeconds(5);
        print("wait Defated 2");
        nextGameTransition.TriggerTransition();
    }
    #endregion
}

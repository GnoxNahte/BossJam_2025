using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    #region Serialized Variables
    [field: SerializeField] public FillUI BossHealthUI { get; private set; }
    [field: SerializeField] public FillUI PlayerHealthUI { get; private set; }
    
    [SerializeField] private TransitionToGame restartGameTransition;
    [SerializeField] private TransitionToGame nextGameTransition;
    [SerializeField] private TransitionToGame mainMenuTransition;

    [SerializeField] private float fadeAnimDuration;
    [SerializeField] private CanvasGroup deathScreen;
    [SerializeField] private CanvasGroup blackScreen;

    [SerializeField] private CanvasGroup gameCompleteScreen;

    private Coroutine _currAnim;
    private bool _isFinalLevel;
    #endregion
    
    #region Public Methods
    public void Init(bool isMainMenuLoaded, LevelManager levelManager)
    {
        if (isMainMenuLoaded)
        {
            BossHealthUI.Disable();
            PlayerHealthUI.Disable();
        }
        
        restartGameTransition.SetTransitionSceneName(levelManager.CurrSceneName);
        nextGameTransition.SetTransitionSceneName(levelManager.NextSceneName);

        _isFinalLevel = levelManager.IsFinalLevel;
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

    // UI button event
    public void OnReturnToMainMenuClicked()
    {
        mainMenuTransition.TriggerTransition();
    }

    public void OnBossDefeated()
    {
        print("Defated");
        if (_isFinalLevel)
            _currAnim = StartCoroutine(GameCompleteTransition());
        else
            _currAnim = StartCoroutine(WaitWinTransition());
    }
    #endregion

    #region Unity Methods

    private void Start()
    {
        _currAnim = StartCoroutine(FadeBlackScreen(false));
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
        yield return FadeBlackScreen(true);
        
        nextGameTransition.TriggerTransition();
    }

    private IEnumerator GameCompleteTransition()
    {
        print("wait Defated");
        yield return new WaitForSeconds(5);
        print("wait Defated 2");
        yield return FadeBlackScreen(true);
        
        float currAnimTime = 0f;
        gameCompleteScreen.alpha = 0f;
        gameCompleteScreen.gameObject.SetActive(true);

        while (currAnimTime < fadeAnimDuration)
        {
            currAnimTime += Time.deltaTime;
            
            gameCompleteScreen.alpha = currAnimTime / fadeAnimDuration;
            
            yield return null;
        }
        
        gameCompleteScreen.alpha = 1f;

        _currAnim = null;
    }

    private IEnumerator FadeBlackScreen(bool toBlack)
    {
        float currAnimTime = 0f;
        blackScreen.alpha = toBlack ? 0f : 1f;
        blackScreen.gameObject.SetActive(true);

        while (currAnimTime < fadeAnimDuration)
        {
            currAnimTime += Time.deltaTime;
            
            float alpha = currAnimTime / fadeAnimDuration;
            if (!toBlack)
                alpha = 1f - alpha;
            
            blackScreen.alpha = alpha;
            
            yield return null;
        }
        
        blackScreen.alpha = toBlack ? 1f : 0f;

        _currAnim = null;
    }
    #endregion
}

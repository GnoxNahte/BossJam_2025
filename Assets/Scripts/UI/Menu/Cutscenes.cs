using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Cutscenes : MonoBehaviour
{
    [SerializeField] private string[] cutsceneText;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private Image cutsceneImage;

    [SerializeField] private float fadeDuration;
    [SerializeField] private float timeBetweenLetter;

    [SerializeField] [ReadOnly] 
    private int currCutsceneIndex = 0;

    [SerializeField] [ReadOnly] 
    private bool isDoneTyping;

    [SerializeField] [ReadOnly] 
    private bool isFadeDone = false;
    
    private CanvasGroup _canvasGroup;
    private Animator _animator;
    private TransitionToGame _transitionToGame;
    
    private WaitForSeconds _typingWait;
    Coroutine _currCoroutine;
    
    private static readonly int OnTransition = Animator.StringToHash("OnTransition");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _transitionToGame = GetComponent<TransitionToGame>();
        
        _typingWait = new WaitForSeconds(timeBetweenLetter);

        _currCoroutine = StartCoroutine(FadeAnim());
        
        currCutsceneIndex = 0;
        isFadeDone = false;
        uiText.text = "";
    }

    private IEnumerator FadeAnim()
    {
        float fadeTime = 0f;
        
        cutsceneImage.color = new Color(1f, 1f, 1f, 0f);

        // Fade canvas
        while (fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;
            
            _canvasGroup.alpha = fadeTime / fadeDuration;
            yield return null;
        }

        fadeTime = 0f;
        Color color = Color.white;
        // Fade canvas
        while (fadeTime < fadeDuration)
        {
            fadeTime += Time.deltaTime;
            
            color.a = fadeTime / fadeDuration;
            cutsceneImage.color = color;
            yield return null;
        }
        
        isFadeDone = true;
        _currCoroutine = null;
        StartCutscene();
    }

    public void OnContinuePressed()
    {
        print("1");
        if (!isFadeDone)
            return;
        
        print("2");
        // Next cutscene
        if (isDoneTyping)
        {
        print("3");
            ++currCutsceneIndex;
            if (currCutsceneIndex >= cutsceneText.Length)
            {
        print("4");
                _transitionToGame.TriggerTransition();
            }
            else
            {
        print("5");
                _animator.SetTrigger(OnTransition);
                StartCutscene();
            }
        }
        // Skip curr dialogue
        else
        {
            print("6");
            uiText.text = cutsceneText[currCutsceneIndex];
            isDoneTyping = true;
            
            if (_currCoroutine != null)
            {
            print("7");
                StopCoroutine(_currCoroutine);
                _currCoroutine = null;
            }
        }
    }

    private void StartCutscene()
    {
        if (_currCoroutine != null)
            StopCoroutine(_currCoroutine);
        print("StartCutscene + " + currCutsceneIndex);
        isDoneTyping = false;
        _currCoroutine = StartCoroutine(TextAnim(cutsceneText[currCutsceneIndex]));
    }

    private IEnumerator TextAnim(string text)
    {
        int currLen = 0;
        
        while (currLen < text.Length)
        {
            uiText.text = text.Substring(0, ++currLen);
            print("Update text: " + uiText.text);
            yield return _typingWait;
        }
        
        // yield return _breakWait;
        isDoneTyping = true;
        // StartNextCutscene();
    }

    private void OnValidate()
    {
        _typingWait = new WaitForSeconds(timeBetweenLetter);
    }
}

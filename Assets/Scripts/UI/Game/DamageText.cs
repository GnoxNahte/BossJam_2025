using System;
using System.Collections;
using TMPro;
using UnityEngine;
using VInspector;

public class DamageText : MonoBehaviour
{
    // [ShowInInspector] public static AnimationCurve AnimCurve;
    // [ShowInInspector] public static Color PlayerDamageColor;
    // [ShowInInspector] public static Color EnemyDamageColor;

    // Will be at max font size at reference damage. Scales the bigger/smaller the damage is
    [SerializeField] private float referenceDamage; 
    [SerializeField] private float maxFontSize;
    // Based on damage and ref damage
    [SerializeField] private AnimationCurve fontSizeCurve;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private Color playerDamageColor;
    [SerializeField] private Color enemyDamageColor;
    
    [SerializeField] private float duration;
    
    private Coroutine _currCoroutine;
    private TextMeshPro _text;
    private ObjectPool _pool;

    public void SetDamageText(int damage, bool isPlayer, ObjectPool pool)
    {
        _pool = pool;
        
        _text.text = damage.ToString();
        _text.color = isPlayer ? playerDamageColor : enemyDamageColor;
        
        _currCoroutine = StartCoroutine(AnimateDamageText(damage));
    }

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
    }

    private IEnumerator AnimateDamageText(int damage)
    {
        float timeLeft = 0;
        float currMaxFontSize = fontSizeCurve.Evaluate(damage / referenceDamage) * maxFontSize;

        while (timeLeft < duration)
        {
            timeLeft += Time.deltaTime;
            _text.fontSize = animCurve.Evaluate(timeLeft / duration) * currMaxFontSize;
            yield return null;
        }
        
        _pool.Release(gameObject);
    }
}

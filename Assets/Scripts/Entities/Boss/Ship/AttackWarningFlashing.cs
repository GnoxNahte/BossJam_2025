using System;
using UnityEngine;
using VInspector;

public class AttackWarningFlashing : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField, MinMaxSlider(0f, 1f)] 
    private Vector2 range;

    [SerializeField] private AnimationCurve durationCurve;
    [field: SerializeField] public SmoothVerticalFollow follow { get; private set; }
    #endregion
    
    #region Private Variables
    private bool _isIncreasingOpacity;
    private float _timeLeft;
    
    private SpriteRenderer _sr;
    #endregion

    #region Public Methods

    public void SetTimeLeftPercentage(float timeLeft)
    {
        _timeLeft = timeLeft;
    }

    #endregion
    
    #region Unity Methods

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        follow = GetComponent<SmoothVerticalFollow>();
    }

    private void Update()
    {
        var color = _sr.color;
        color.a += Time.deltaTime / durationCurve.Evaluate(_timeLeft) * (_isIncreasingOpacity ? 1f : -1f);
        _sr.color = color;
        
        if (color.a <= range.x)
            _isIncreasingOpacity = true;
        else if (color.a >= range.y)
            _isIncreasingOpacity = false;
    }

    #endregion
}

using System;
using UnityEngine;

public class FillUI : MonoBehaviour
{
    private RectTransform _rectTransform;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateFill(float percentage)
    {
        _rectTransform.anchorMax = new Vector2(percentage, 1f);
    }
}

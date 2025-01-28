using System;
using UnityEngine;

public class FillUI : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    private RectTransform _rectTransform;

    public void UpdateFill(float percentage)
    {
        _rectTransform.anchorMax = new Vector2(percentage, 1f);
    }

    public void Disable()
    {
        parent.SetActive(false);
    }
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}

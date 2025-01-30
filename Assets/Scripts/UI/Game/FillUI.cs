using System;
using TMPro;
using UnityEngine;

public class FillUI : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform _rectTransform;

    private int _maxHp;

    public void Init(int maxHp)
    {
        _maxHp = maxHp;
        text.text = $"{maxHp} / {_maxHp}";
    }

    public void UpdateFill(int currHp)
    {
        _rectTransform.anchorMax = new Vector2((float)currHp / _maxHp, 1f);
        text.text = $"{currHp} / {_maxHp}";
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

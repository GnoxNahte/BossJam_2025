using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image abilityIcon;
    [SerializeField] private RectTransform abilityFill;
    [SerializeField] private TextMeshProUGUI abilityName;
    private AbilityBase<PlayerAbilitySystem.Type> _ability;

    public void Init(PlayerAbilityBase ability)
    {
        _ability = ability;
        abilityIcon.sprite = ability.Icon;
        abilityName.text = ability.Name;

        ability.OnCooldownTimeLeftChanged -= UpdateCooldownUI;
        ability.OnCooldownTimeLeftChanged += UpdateCooldownUI;

        UpdateCooldownUI();
    }

    private void OnEnable()
    {
        if (_ability != null)
        {
            _ability.OnCooldownTimeLeftChanged -= UpdateCooldownUI;
            _ability.OnCooldownTimeLeftChanged += UpdateCooldownUI;
        }
    }

    private void OnDisable() 
    {
        if (_ability != null)
            _ability.OnCooldownTimeLeftChanged -= UpdateCooldownUI;
    }

    private void UpdateCooldownUI()
    {
        float percentage = _ability.CooldownTimeLeft / _ability.Cooldown;
        abilityFill.anchorMax = new Vector2(abilityFill.anchorMax.x, percentage);
    }
}

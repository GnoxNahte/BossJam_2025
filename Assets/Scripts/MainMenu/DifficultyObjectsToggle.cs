using System;
using TMPro;
using UnityEngine;

public class DifficultyObjectsToggle : MonoBehaviour
{
    [SerializeField] private PlatformBase platform;
    [SerializeField] private GameObject text;
    [SerializeField] private Collider2D hardTriggerCollider;
    [SerializeField] private Collider2D easyTriggerCollider;

    [SerializeField] private Collider2D easyTriggerCollider_Uncleared;

    [SerializeField] private TextMeshProUGUI completedText;
    [SerializeField] private TextMeshProUGUI completedHardText;
    
    private void Start()
    {
        platform.gameObject.SetActive(GameInitiator.IsGameCleared);
        text.SetActive(GameInitiator.IsGameCleared);
        hardTriggerCollider.gameObject.SetActive(GameInitiator.IsGameCleared);
        easyTriggerCollider.gameObject.SetActive(GameInitiator.IsGameCleared);
        
        easyTriggerCollider_Uncleared.gameObject.SetActive(!GameInitiator.IsGameCleared);

        if (GameInitiator.IsGameCleared_Hard)
            completedHardText.gameObject.SetActive(true);
        else if (GameInitiator.IsGameCleared)
            completedText.gameObject.SetActive(true);
    }
}

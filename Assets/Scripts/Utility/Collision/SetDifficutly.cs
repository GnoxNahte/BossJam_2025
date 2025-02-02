
using System;
using UnityEngine;

public class SetDifficutly : MonoBehaviour
{
    [SerializeField] private bool isHardMode;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameInitiator.IsHardMode = isHardMode;
        print("Hard 1: "+ isHardMode);
        print("Hard 2: "+ GameInitiator.IsHardMode);
    }
}

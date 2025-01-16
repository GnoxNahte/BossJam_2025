using System;
using Unity.Cinemachine;
using UnityEngine;

public class ActivateGameObjectOnTrigger : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private bool ifActivate;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        obj.gameObject.SetActive(ifActivate);
    }
}

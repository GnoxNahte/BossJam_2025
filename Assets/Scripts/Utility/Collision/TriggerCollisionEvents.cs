using System;
using UnityEngine;

public class TriggerCollisionEvents : MonoBehaviour
{
    #region Public Variables

    public Action OnTriggerEnter;
    public Action OnTriggerExit;
    #endregion
    
    #region Unity Methods

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExit?.Invoke();
    }

    #endregion
}

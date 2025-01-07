using System;
using UnityEngine;

public class Player : EntityBase
{
    #region Public Variables
    public const string PlayerTag = "Player";
    #endregion
    
    #region Serialized Variables
    
    #endregion
    
    #region Private Variables

    private PlayerMovement _playerMovement;
    
    #endregion
    
    #region Public Methods
    
    public void Init(InputManager input)
    {
        _playerMovement.Init(input);
    }
    #endregion
    
    #region Unity Methods

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    #endregion
    
    #region Private Methods
    
    #endregion
}

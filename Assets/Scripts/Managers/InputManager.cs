using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    #region Public Variables
    [field: SerializeField] public Vector2 MoveDir { get; private set; }
    [field: SerializeField] public bool IsJumping { get; private set; }
    [field: SerializeField] public bool JumpPressedThisFrame { get; private set; }
    [field: SerializeField] public bool IsCharging { get; private set; }
    [field: SerializeField] public bool IsDashing { get; private set; }
    [field: SerializeField] public bool IsAttacking { get; private set; }

    public event Action OnPausePressed;
    #endregion
    
    #region Serialized Variables
    #endregion
    
    #region Private Variables
    private PlayerControls _playerControls;
    
    private InputAction _move;
    private InputAction _jump;
    private InputAction _charge;
    private InputAction _dash;
    private InputAction _attack;
    private InputAction _pause;
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _playerControls = new PlayerControls();
        
        _move = _playerControls.Player.Move;
        _jump = _playerControls.Player.Jump;
        _charge = _playerControls.Player.Charge;
        _dash = _playerControls.Player.Dash;
        _attack = _playerControls.Player.Attack;
        _pause = _playerControls.Player.Pause;
    }

    private void OnEnable()
    {
        _move.Enable();
        _jump.Enable();
        _charge.Enable();
        _dash.Enable();
        _attack.Enable();
        _pause.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _jump.Disable();
        _charge.Disable();
        _dash.Disable();
        _attack.Disable();
        _pause.Disable();
    }

    private void Update()
    {
        MoveDir = _move.ReadValue<Vector2>();
        IsJumping = _jump.IsPressed();
        JumpPressedThisFrame = _jump.WasPressedThisFrame();
        IsCharging = _charge.IsPressed();
        IsDashing = _dash.IsPressed();
        IsAttacking = _attack.IsPressed();
    }
    #endregion
}

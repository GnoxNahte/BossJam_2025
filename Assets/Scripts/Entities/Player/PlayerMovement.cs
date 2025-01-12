using UnityEngine;
using VInspector;

public class PlayerMovement : MonoBehaviour
{
    #region Public Variables
    
    public Vector2 Velocity => velocity; 
    public Vector2 FacingDirection => _facingDirection; 
    #endregion
    
    #region Serialized Variables

    [SerializeField] private PlayerMovementStats stats;

    [Header("References")]
    [SerializeField] protected PlatformCollisionTracker ceilingChecker;
    [SerializeField] protected PlatformCollisionTracker groundChecker;
    [SerializeField] protected PlatformCollisionTracker leftWallChecker;
    [SerializeField] protected PlatformCollisionTracker rightWallChecker;
    
    [SerializeField] protected AudioSource landSFX;
    
    // Read only, for debugging
    [Header("Tracking Variables")]

    [SerializeField] [ReadOnly]
    private Vector2 velocity;

    [SerializeField] [ReadOnly]
    private float lastJumpPressed;
    [SerializeField] [ReadOnly]
    private float lastGroundedTime;
    [SerializeField] [ReadOnly]
    private float lastJumpTime;

    // If the player has released the jump button after jumping
    [SerializeField] [ReadOnly]
    private bool ifReleaseJumpAfterJumping;

    [SerializeField] [ReadOnly]
    private bool isInAir;

    [SerializeField] [ReadOnly]
    private float dashTimeLeft;

    // Stops movement if using ability
    [SerializeField] [ReadOnly]
    private bool isCharging;
    [SerializeField] [ReadOnly]
    private bool isSpinning;
    
    #endregion
    
    #region Private Variables

    private InputManager _input;
    private Rigidbody2D _rb;
    private PlayerAbilitySystem _abilitySystem;
    
    // Only change when there's input
    private Vector2 _facingDirection;
    
    #endregion
    
    #region Public Methods
    public void Init(PlayerAbilitySystem abilitySystem, InputManager input)
    {
        _input = input;
        _abilitySystem = abilitySystem;
        
        ResetPlayer();
    }

    public bool IsGrounded()
    {
        return !isInAir;
    }
    
    public void Dash()
    {
        dashTimeLeft = stats.DashTime;
    }
    
    public void CancelDash()
    {
        dashTimeLeft = -1f;
        velocity = stats.DashCurve.Evaluate(1f) * _facingDirection;
    }
    
    // Returns if can start charging spin
    public void ChargeSpin()
    {
        isCharging = true;
        velocity = Vector2.zero;
    }

    public void Spin()
    {
        isCharging = false;
        
        isSpinning = true;
        velocity = new Vector2(stats.SpinHorizontalSpeed * (FacingDirection.x > 0 ? 1 : -1), 0f);
    }
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float realtime = Time.realtimeSinceStartup;
        
        if (_input.JumpPressedThisFrame) 
            lastJumpPressed = realtime;
        else
            ifReleaseJumpAfterJumping = true;

        if (_input.MoveDir.x != 0 && !(isCharging || isSpinning))
            _facingDirection = _input.MoveDir;
    }

    private void FixedUpdate()
    {
        // Updates the velocity for horizontal and vertical movement
        if (isSpinning)
        {
            SpinningMovement();
        }
        else
        {
            HorizontalMovement();
            VerticalMovement();
        }

        _rb.linearVelocity = Vector2.zero;
        
        Vector2 platformMoveAmt = groundChecker.GetPlatformMoveAmt();
        Vector2 totalMoveAmt = platformMoveAmt + velocity * Time.deltaTime;
        _rb.MovePosition(_rb.position + totalMoveAmt);
    }

    #endregion
    
    #region Private Methods

    // Updates horizontal velocity
    private void HorizontalMovement()
    {
        if (isCharging)
            return;
        
        if (dashTimeLeft > 0f)
        {
            dashTimeLeft -= Time.deltaTime;
            
            // On finish dash
            if (dashTimeLeft < 0f)
            {
                _abilitySystem.OnAbilityEnd(PlayerAbilitySystem.Type.Dash);
                dashTimeLeft = -1f;
                return;
            }
            
            // Negate 1f - percentage so that the curve goes from left to right.
            velocity = stats.DashCurve.Evaluate(1f - dashTimeLeft / stats.DashTime) * _facingDirection;
            
            return;
        }
        
        float xInput = _input.MoveDir.x;
        bool isGrounded = !isInAir;
        
        // Slow down the player if not pressing any buttons 
        if (xInput == 0f)
        {
            float stopVelocityAmt = stats.StopAcceleration * Time.deltaTime;
            // negate stopVelocityAmt because stats.stopAcceleration is always < 0 
            if (Mathf.Abs(velocity.x) > -stopVelocityAmt)
                velocity.x += stopVelocityAmt * (velocity.x > 0f ? 1f : -1f);
            else
                velocity.x = 0f;
        }
        else
        {
            // Comparing input dir and velocity, if...
            // Moving in the same direction
            if (xInput * velocity.x >= 0)
            {
                velocity.x += stats.MoveAcceleration * xInput * Time.deltaTime;
                
                // Clamp horizontal speed depending if it's colliding with the ground
                float maxSpeed = isGrounded ? stats.MaxSpeed : stats.AirStrafeMaxSpeed;
                velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
            }
            // Moving in opposite direction/turning (Grounded)
            else if (isGrounded)
                velocity.x -= stats.TurnAcceleration * (xInput) * Time.deltaTime;
            // Moving in opposite direction/turning (In air)
            else
                velocity.x -= stats.InAirTurnAcceleration * (xInput) * Time.deltaTime;
        }
    }
    
    // Updates vertical velocity
    private void VerticalMovement()
    {
        HandleLanding();

        if (dashTimeLeft > 0)
            return;
        
        HandleGravity();
        
        if (!isCharging)
            HandleJump();
    }

    private void SpinningMovement()
    {
        if (HandleJump())
        {
            isSpinning = false;
        }
        // Stop and apply knockback if collided
        if (leftWallChecker.IsColliding || rightWallChecker.IsColliding)
        {
            velocity = new Vector2(stats.WallJumpHorizontalVelocity, stats.JumpVelocity);
            if (rightWallChecker.IsColliding)
                velocity.x *= -1; // Flip horizontal velocity
            
            isSpinning = false;
        }

        if (!isSpinning)
        {
            _abilitySystem.OnAbilityEnd(PlayerAbilitySystem.Type.Spin);
            return;
        }
        
        HandleGravity();
        HandleLanding();
    }

    private void HandleLanding()
    {
        if (!groundChecker.IsColliding || velocity.y > 0f) 
            return;
        
        // If player is holding down while falling,
        // Set one-way platform to be flipped (pass through from top)
        if (_input.MoveDir.y < 0)
        {
            PlatformBase platform = groundChecker.GetCollidingPlatformWithType(PlatformBase.Type.OneWay);
            if (!platform) 
                return;
            
            OneWayPlatformModifier oneWayModifier = platform.GetPlatformModifier(PlatformBase.Type.OneWay) as OneWayPlatformModifier;
            oneWayModifier?.SetFlipped(true);
        }
        // Land on ground
        else
        {
            // TODO: Check if need this if statement
            if (isInAir)
            {
                // landSFX.Play();
            }

            lastJumpTime = float.MinValue;
            lastGroundedTime = Time.realtimeSinceStartup;
        }
    }

    private void HandleGravity()
    {
        float realtime = Time.realtimeSinceStartup;
        
        isInAir = true;

        // If moving up
        if (velocity.y > 0f)
        {
            // If hit ceiling
            if (ceilingChecker.IsCollidingWith(PlatformBase.Type.Everything, PlatformBase.Type.OneWay))
                velocity.y = 0f;
            // Increased gravity if:
            // - player is moving up and not inputting jump
            // - AND it has jumped more than the minimum time
            else if (!_input.IsJumping && (realtime - lastJumpPressed) > stats.MinJumpTime)
                velocity.y += stats.Gravity * Time.deltaTime * stats.GravityMultiplierWhenRelease;
            else 
                velocity.y += stats.Gravity * Time.deltaTime;
        }
        // If on ground
        else if (groundChecker.IsGrounded())
        {
            isInAir = false;
            velocity.y = 0f;
        }
        // If wall sliding
        else if (leftWallChecker.IsColliding || rightWallChecker.IsColliding)
        {
            velocity.y = Mathf.Max(velocity.y + stats.WallSlideAcceleration * Time.deltaTime, stats.WallSlideMaxSpeed);
        }
        // Else, player's falling
        else
        {
            velocity.y = Mathf.Max(velocity.y + stats.FallingGravity * Time.deltaTime, stats.MaxFallVelocity);
        }
    }

    // Returns if was able to jump
    private bool HandleJump()
    {
        float realtime = Time.realtimeSinceStartup;

        bool isJumpBufferActive = realtime - lastJumpPressed < stats.JumpBuffer;
        // NOTE: this doesn't include the checks if it has a platform to jump off from
        // ifReleaseJumpAfterJumping - ensures that it doesn't keep jumping if player holds jump and player lands
        bool ifJump = isJumpBufferActive && ifReleaseJumpAfterJumping;
        
        bool isLeftWallColliding = leftWallChecker.IsColliding;
        bool isRightWallColliding = rightWallChecker.IsColliding;
        
        // Wall jumps
        if (ifJump &&
            // If player is colliding with either walls
            (isLeftWallColliding || isRightWallColliding) &&
            // If player is either
            // - In Air
            // - On Ground AND Horizontal Input != 0
            (isInAir || _input.MoveDir.x != 0f))
        {
            Jump();
            
            // Add some horizontal input depending on the horizontal input direction and which side the wall is on.
            bool isInputTowardsWall = _input.MoveDir.x != 0 &&
                                      _input.MoveDir.x < 0 ? isLeftWallColliding : !isLeftWallColliding;
            velocity.x = (isLeftWallColliding ? 1f : -1f) * 
                         (isInputTowardsWall ? stats.WallJumpHorizontalVelocityTowardsWall : stats.WallJumpHorizontalVelocity);

            return true;
        }
        // Normal wall jump. 
        // Also checks if player isGrounded and coyoteTime
        else if (ifJump && (realtime - lastGroundedTime < stats.CoyoteTime))
        {
            Jump();

            return true;
        }

        return false;
    }

    private void Jump()
    {
        velocity.y = stats.JumpVelocity;
        lastJumpPressed = float.MinValue; // Prevent jump buffer from triggering again
        lastGroundedTime = float.MinValue;
        lastJumpTime = Time.realtimeSinceStartup;
        ifReleaseJumpAfterJumping = false;
    }
    
    private void ResetPlayer()
    {
        velocity = Vector2.zero;
        
        lastJumpPressed = float.MinValue;
        lastGroundedTime = float.MinValue;
        lastJumpTime = float.MinValue;

        ifReleaseJumpAfterJumping = true;

        isInAir = false;
    }
    #endregion
}
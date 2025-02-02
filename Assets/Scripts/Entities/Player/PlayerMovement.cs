using System;
using System.Collections;
using UnityEngine;
using VInspector;

public class PlayerMovement : MonoBehaviour
{
    #region Public Variables
    public bool IsInAir => isInAir;
    public Vector2 Velocity => velocity; 
    public Vector2 FacingDirection => _facingDirection;
    public bool IsSpinning => isSpinning;
    public bool IsDead => isDead;
    public bool IsGemGrab => isGemGrab;
    public bool IsDisabledMovement => isDead || isGemGrab;
    public bool IsDashing => dashTimeLeft > 0f;
    
    // Takes in damage and position
    public Action<int, Vector2> OnHit;
    #endregion
    
    #region Serialized Variables

    [SerializeField] private PlayerMovementStats stats;
    [SerializeField] private int normalDamage;
    [SerializeField] private int dashDamage;
    [SerializeField] private int spinDamage;
    [SerializeField] private LayerMask invincibilityMask;

    [Header("References")]
    [SerializeField] protected PlatformCollisionTracker ceilingChecker;
    [SerializeField] protected PlatformCollisionTracker groundChecker;
    [SerializeField] protected PlatformCollisionTracker leftWallChecker;
    [SerializeField] protected PlatformCollisionTracker rightWallChecker;
    
    [SerializeField] private ObjectPool shockwavePool;
    
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
    [SerializeField] [ReadOnly]
    private float lastDashTime;

    // Stops movement if using ability
    [SerializeField] [ReadOnly]
    private bool isSpinning;
    [SerializeField] [ReadOnly]
    private bool isDead;
    [SerializeField] [ReadOnly]
    private bool isGemGrab;
    [SerializeField] [ReadOnly]
    private bool isInvincibleDamage;
    
    #endregion
    
    #region Private Variables

    private InputManager _input;
    private Rigidbody2D _rb;
    private PlayerAppearance _appearance;
    
    // Only change when there's input
    private Vector2 _facingDirection;
    private bool _isLastFacingRight; // For when _facingDirection.x == 0 && _facingDirection.y == (1 or 0)
    
    private Coroutine _invincibilityCoroutine;
    private WaitForSeconds _invincibilityWait;
    
    #endregion
    
    #region Public Methods
    public void Init(InputManager input)
    {
        _input = input;
        
        ResetPlayer();
    }

    public bool IsGrounded()
    {
        return !isInAir;
    }
    
    public void Dash()
    {
        if (Time.time - lastDashTime < stats.DashCooldown)
            return;
        
        lastDashTime = Time.time;
        dashTimeLeft = stats.DashTime;

        isSpinning = false;
    }
    
    public void Spin()
    {
        if (isSpinning)
            return;
        
        dashTimeLeft = -1f;
        
        isSpinning = true;
        bool ifGoRight;
        if (_facingDirection.x != 0)
            ifGoRight = _facingDirection.x > 0;
        else
            ifGoRight = _isLastFacingRight;
        velocity = new Vector2(stats.SpinHorizontalSpeed * (ifGoRight ? 1 : -1), 0f);
    }

    public void OnGemGrab()
    {
        DisableInput();
        isGemGrab = true;
    }

    public void OnDeath()
    {
        DisableInput();
        isDead = true;
    }
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _appearance = GetComponent<PlayerAppearance>();

        shockwavePool.transform.parent = null;
        
        _invincibilityWait = new WaitForSeconds(stats.InvincibilityDuration);
    }

    private void Update()
    {
        if (IsDisabledMovement)
            return;
        
        float realtime = Time.realtimeSinceStartup;
        
        if (_input.JumpPressedThisFrame) 
            lastJumpPressed = realtime;
        else
            ifReleaseJumpAfterJumping = true;

        if (_input.MoveDir.x != 0f)
            _isLastFacingRight = _input.MoveDir.x > 0f;
        
        if (_input.MoveDir != Vector2.zero)
            _facingDirection = _input.MoveDir;
    }

    private void FixedUpdate()
    {
        // If dead, just apply friction on gravity
        if (IsDisabledMovement)
            return;
        
        // Updates the velocity for horizontal and vertical movement
        HorizontalMovement();
        VerticalMovement();

        _rb.linearVelocity = Vector2.zero;
        
        Vector2 platformMoveAmt = groundChecker.GetPlatformMoveAmt();
        Vector2 totalMoveAmt = platformMoveAmt + velocity * Time.deltaTime;
        
        _rb.MovePosition(_rb.position + totalMoveAmt);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // print("Contact point: " + other.contacts.Length + " | " + other.contacts[0].normal);
        // if (other.contacts.Length > 1)
        //     print("Contact point: " + other.contacts.Length + " | " + other.contacts[1].normal);
        
        EntityBase entity = other.gameObject.GetComponent<EntityBase>();
        if (entity != null)
        {
            ContactPoint2D contactPoint = other.contacts[0];

            int damage;
            if (isSpinning)
            {
                AudioManager.PlaySFX(AudioManager.SFX.PlayerAttack_Spin);
                damage = spinDamage;
            } 
            else if (dashTimeLeft > 0f)
            {
                AudioManager.PlaySFX(AudioManager.SFX.PlayerAttack_Dash);
                damage = dashDamage;
            }
            else
            {
                AudioManager.PlaySFX(AudioManager.SFX.PlayerAttack_Normal);
                damage = normalDamage;
            }
            
            entity.TakeDamage(damage, contactPoint.point);

            if (isSpinning)
            {
                GameObject shockwave = shockwavePool.Get(contactPoint.point);
                shockwave.GetComponent<Shockwave>().Init(shockwavePool);
            }
            
            ApplyKnockback(contactPoint.normal, entity.KnockbackSpeed);
            _appearance.OnHitAttack();
            // ActivateInvincibility();
        }
        
        Bomb bomb = other.gameObject.GetComponent<Bomb>();
        if (bomb)
        {
            // Only damage player and apply knockback 
            if (bomb.transform.position.y < transform.position.y)
                return;
            
            ContactPoint2D contactPoint = other.contacts[0];
            ApplyKnockback(contactPoint.normal, bomb.PlayerKnockbackSpeed);
            if (!isInvincibleDamage)
                OnHit?.Invoke(bomb.Damage, contactPoint.point);
            
            ActivateInvincibility();
            return;
        }
        
        Dagger dagger = other.gameObject.GetComponent<Dagger>();
        if (dagger)
        {
            // Only damage player and apply knockback 
            if (dagger.transform.position.y < transform.position.y)
                return;
            
            ContactPoint2D contactPoint = other.contacts[0];
            ApplyKnockback(contactPoint.normal, dagger.PlayerKnockbackSpeed);
            if (!isInvincibleDamage)
                OnHit?.Invoke(dagger.Damage, contactPoint.point);
            
            ActivateInvincibility();
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageTrigger damageTrigger = other.gameObject.GetComponent<DamageTrigger>();
        if (damageTrigger)
        {
            Vector2 dir = Vector2.one;
            if (transform.position.x < other.transform.position.x)
                dir.x *= -1f;
            ApplyKnockback(dir, damageTrigger.KnockbackSpeed);
            
            if (!isInvincibleDamage)
                OnHit?.Invoke(damageTrigger.Damage, transform.position);
            
            ActivateInvincibility();
        }
    }

    #endregion
    
    #region Private Methods

    // Updates horizontal velocity
    private void HorizontalMovement()
    {
        if (isSpinning)
        {
            // If hit either left or right wall and the wall isn't a one way
            bool ifHitWall = (leftWallChecker.IsColliding && !leftWallChecker.GetCollidingPlatformWithType(PlatformBase.Type.OneWay)) ||
                             (rightWallChecker.IsColliding && !rightWallChecker.GetCollidingPlatformWithType(PlatformBase.Type.OneWay));
                
            // Stop and apply knockback if collided
            if (ifHitWall)
            {
                velocity = stats.SpinHitVelocity;
                if (rightWallChecker.IsColliding)
                    velocity.x *= -1; // Flip horizontal velocity

                StopSpinning();
            }
            else
            {
                velocity.x = stats.SpinHorizontalSpeed * (FacingDirection.x > 0 ? 1 : -1);
            }
            
            return;
        }
        
        if (dashTimeLeft > 0f)
        {
            dashTimeLeft -= Time.deltaTime;
            
            // On finish dash
            if (dashTimeLeft < 0f)
            {
                dashTimeLeft = -1f;
                return;
            }
            
            // Negate 1f - percentage so that the curve goes from left to right.
            Vector2 dir = _facingDirection;
            if (dir.x == 0f)
                dir.x = _isLastFacingRight ? 1f : -1f;
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
        
        // if (!isCharging)
        HandleJump();
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

    private void StopSpinning()
    {
        isSpinning = false;
    }

    // contactDirection - Direction from contact point to player
    private void ApplyKnockback(Vector2 contactDirection, Vector2 speed)
    {
        StopSpinning();
        dashTimeLeft = -1f;
        
        velocity = speed * contactDirection.normalized;
        Debug.DrawRay(transform.position, velocity, Color.red, 1f);

        if (Mathf.Abs(velocity.y) < stats.MinKnockbackVerticalSpeed)
        {
            velocity.y = stats.MinKnockbackVerticalSpeed;
        }

        if (velocity.y < 0f)
            velocity.y *= 0.5f; // Down knockback is too strong, partly because of increased gravity when going down
        
        // Debug.DrawRay(transform.position, velocity, Color.green, 1f);
        // print("Knockback final vel: " + velocity);
        
        Vector2 totalMoveAmt = velocity * Time.fixedDeltaTime;
        
        transform.position = (_rb.position + totalMoveAmt);
    }

    private void ActivateInvincibility()
    {
        if (_invincibilityCoroutine != null)
        {
            StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = null;
        }
        _invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
    }

    private IEnumerator InvincibilityCoroutine()
    {
        // _rb.excludeLayers = invincibilityMask;
        isInvincibleDamage = true;
        _appearance.OnInvincibilityChange(true);

        yield return _invincibilityWait;
        
        _appearance.OnInvincibilityChange(false);
        // _rb.excludeLayers = 0;
        isInvincibleDamage = false;
        
        _invincibilityCoroutine = null;
    }

    private void DisableInput()
    {
        velocity = Vector2.zero;
        dashTimeLeft = -1f;
        isSpinning = false;
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }
    
    private void ResetPlayer()
    {
        velocity = Vector2.zero;
        
        lastJumpPressed = float.MinValue;
        lastGroundedTime = float.MinValue;
        lastJumpTime = float.MinValue;

        ifReleaseJumpAfterJumping = true;

        isInAir = false;
        isSpinning = false;
    }
    #endregion
}
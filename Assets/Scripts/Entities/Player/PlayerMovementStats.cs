using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Player Movement Stats", menuName = "ScriptableObjects/Player Movement Stats")]
public class PlayerMovementStats : ScriptableObject
{
    // ===== Movement Settings =====
    [Header("Horizontal Movement")]

    public float MaxSpeed;
    public float AirStrafeMaxSpeed;

    [Tooltip("Time to max speed")]
    public float MaxSpeedTime;

    [Tooltip("Time to from max speed to stop (when no input from player)")]
    public float StopTime;

    [Tooltip("Time to from max speed to max speed in the opposite direction (when input in opposite direction)")]
    public float TurnTime;

    [Tooltip("Time to from max speed to max speed in the opposite direction when in the air (when input in opposite direction)")]
    public float InAirTurnTime;

    [ReadOnly]
    public float MoveAcceleration;
    [ReadOnly]
    public float StopAcceleration;
    [ReadOnly]
    public float TurnAcceleration;
    [ReadOnly]
    public float InAirTurnAcceleration;

    public float DashCooldown;
    public float DashTime;
    public AnimationCurve DashCurve;

    public float SpinHorizontalSpeed;
    public float SpinVerticalSpeed;

    // ===== Gravity =====
    [Header("Gravity")]

    public float MaxFallVelocity;

    [ReadOnly]
    [Tooltip("Gravity when the player is moving up. Control by changing [Max Jump Height] and [Time To Max Height]")]
    public float Gravity;

    [ReadOnly]
    [Tooltip("Gravity when the player is falling. Control by changing [Max Jump Height] and [Time To Ground]")]
    public float FallingGravity;

    [Tooltip("Time to max wall slide speed (Assuming starting speed is 0)")]
    public float WallSlideMaxSpeedTime;
    [Tooltip("Max speed for wall slide")]
    public float WallSlideMaxSpeed;
    [ReadOnly]
    public float WallSlideAcceleration;

    // ===== Jumping =====
    [Header("Jumping")]

    public float MaxJumpHeight;
    public float MinJumpHeight;

    [ReadOnly]
    public float MinJumpTime;

    public float TimeToMaxHeight;

    [Tooltip("Time from max height (vertical speed = 0) to ground (y = 0)")]
    public float TimeToGround;

    [ReadOnly]
    [Tooltip("Upwards velocity when the player press the jump button")]
    public float JumpVelocity;

    [Tooltip("Increased gravity when the player is moving up and the jump button is released. Helps player drop down faster after releasing jump. DOES NOT affect when the player is falling.")]
    public float GravityMultiplierWhenRelease;

    [Tooltip("Let the player jump even if they just fell off a platform")]
    public float CoyoteTime;

    [Tooltip("Let the player queue the next jump if the player jumped in mid air.")]
    public float JumpBuffer;

    [Tooltip("Horizontal jump velocity when wall jumping (When horizontal input NOT towards wall)")]
    public float WallJumpHorizontalVelocity;

    [Tooltip("Horizontal jump velocity when wall jumping (When horizontal input towards wall)")]
    public float WallJumpHorizontalVelocityTowardsWall;
    
    public Vector2 SpinHitVelocity;
    public float MinKnockbackVerticalSpeed;

    [Header("Others")]

    public float InvincibilityDuration;

    private void OnValidate()
    {
        // Limit variables

        float minValue = 0.01f;

        MaxSpeed = Mathf.Max(minValue, MaxSpeed);
        AirStrafeMaxSpeed = Mathf.Max(minValue, AirStrafeMaxSpeed);
        MaxSpeedTime = Mathf.Max(minValue, MaxSpeedTime);
        StopTime = Mathf.Max(minValue, StopTime);
        TurnTime = Mathf.Max(minValue, TurnTime);
        InAirTurnTime = Mathf.Max(minValue, InAirTurnTime);
        MaxFallVelocity = Mathf.Min(-minValue, MaxFallVelocity);
        WallSlideMaxSpeedTime = Mathf.Max(minValue, WallSlideMaxSpeedTime);
        WallSlideMaxSpeed = Mathf.Min(-minValue, WallSlideMaxSpeed);
        MaxJumpHeight = Mathf.Max(minValue, MaxJumpHeight);
        MinJumpHeight = Mathf.Max(minValue, MinJumpHeight);
        TimeToMaxHeight = Mathf.Max(minValue, TimeToMaxHeight);
        TimeToGround = Mathf.Max(minValue, TimeToGround);
        GravityMultiplierWhenRelease = Mathf.Max(minValue, GravityMultiplierWhenRelease);
        CoyoteTime = Mathf.Max(minValue, CoyoteTime);
        JumpBuffer = Mathf.Max(minValue, JumpBuffer);

        // Calculate physics before hand, prevent runtime calculation
        // Movement

        MoveAcceleration = MaxSpeed / MaxSpeedTime;
        StopAcceleration = -MaxSpeed / StopTime;
        TurnAcceleration = 2f * -MaxSpeed / TurnTime;
        InAirTurnAcceleration = 2f * -MaxSpeed / InAirTurnTime;

        // Gravity

        Gravity = (-2 * MaxJumpHeight) / (TimeToMaxHeight * TimeToMaxHeight);
        FallingGravity = (-2 * MaxJumpHeight) / (TimeToGround * TimeToGround);

        WallSlideAcceleration = WallSlideMaxSpeed / WallSlideMaxSpeedTime;

        // Jumping

        JumpVelocity = (2f * MaxJumpHeight) / TimeToMaxHeight;
        MinJumpTime = 2 * MinJumpHeight / JumpVelocity;
    }
}
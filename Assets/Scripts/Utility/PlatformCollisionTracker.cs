using UnityEngine;

public class PlatformCollisionTracker : CollisionTracker<PlatformBase>
{
    #region Public Methods
    
    public bool IsCollidingWith(PlatformBase.Type type)
    {
        return CollidingObjs.Exists(platform => platform.HasPlatformTypeFlag(type));
    }
    public bool IsCollidingWith(PlatformBase.Type type, PlatformBase.Type excludeType)
    {
        return CollidingObjs.Exists(
            platform =>
                type.HasFlag(platform.PlatformType) && 
                !platform.HasPlatformTypeFlag(excludeType)
        );
    }

    public PlatformBase GetCollidingPlatformWithType(PlatformBase.Type type)
    {
        return CollidingObjs.Find(platform => platform.HasPlatformTypeFlag(type));
    }

    // Checks if standing on solid ground
    // Flipped One-way platforms is excluded
    public bool IsGrounded()
    {
        if (!IsColliding)
            return false;

        PlatformBase oneWayPlatform = GetCollidingPlatformWithType(PlatformBase.Type.OneWay);
        
        // If it isn't a one-way platform, it is a solid platform
        if (!oneWayPlatform)
            return true;

        OneWayPlatformModifier oneWayPlatformModifier = oneWayPlatform.GetPlatformModifier(PlatformBase.Type.OneWay) as OneWayPlatformModifier;
        Debug.Assert(oneWayPlatformModifier, "oneWayModifier is null even when platform has 'OneWay' flag'", oneWayPlatform);
        return !oneWayPlatformModifier.IsFlipped;
    }

    public Vector2 GetPlatformMoveAmt()
    {
        PlatformBase movingPlatform = GetCollidingPlatformWithType(PlatformBase.Type.Moving);
        
        // If can't find any moving platforms, return 0, no movement from platforms
        if (!movingPlatform) 
            return Vector2.zero;
        
        MovingPlatformModifier movingPlatformModifier = movingPlatform.GetPlatformModifier(PlatformBase.Type.Moving) as MovingPlatformModifier;
        Debug.Assert(movingPlatformModifier, "movingPlatformModifier is null even when platform has 'Moving' flag'", movingPlatform);
        return movingPlatformModifier.MoveAmt;
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}
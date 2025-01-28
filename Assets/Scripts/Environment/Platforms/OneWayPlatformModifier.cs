using UnityEngine;

[RequireComponent(typeof(PlatformEffector2D))]
public class OneWayPlatformModifier : PlatformModifierBase
{
    public bool IsFlipped { get; private set; }

    private PlatformEffector2D _effector2D;

    public void SetFlipped(bool value)
    {
        if (value)
        {
            IsFlipped = true;
            _effector2D.rotationalOffset = 180f;
        }
        else
        {
            IsFlipped = false;
            _effector2D.rotationalOffset = 0f;
        }
    }

    private void Awake()
    {
        _effector2D = GetComponent<PlatformEffector2D>();
        Debug.Assert(GetComponent<PlatformBase>().HasPlatformTypeFlag(PlatformBase.Type.OneWay), "Platform type is not one way");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Player.PlayerTag))
            SetFlipped(false);
    }

    private void OnValidate()
    {
        Type = PlatformBase.Type.OneWay;
    }
}
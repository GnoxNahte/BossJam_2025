using UnityEngine;
using VInspector;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatformModifier : PlatformModifierBase
{
    [SerializeField] [ReadOnly]
    Vector2 startPos;

    [SerializeField] Vector2 scale;
    [SerializeField] protected float speed;
    [SerializeField] protected float moveDownOffset;
    [field: SerializeField] public Vector2 MoveAmt { get; protected set; }

    private void Start()
    {
        startPos = transform.position;

        Debug.Assert(GetComponent<Rigidbody2D>()?.bodyType == RigidbodyType2D.Kinematic, "Rigidbody isn't Kinematic");
    }

    private void FixedUpdate()
    {
        Vector2 oldPos = transform.position;
        transform.position = startPos + new Vector2(Mathf.Sin(Time.timeSinceLevelLoad * speed), Mathf.Cos(Time.timeSinceLevelLoad * speed)) * scale;    
        MoveAmt = (Vector2)transform.position - oldPos;

        if (MoveAmt.y < 0)
            MoveAmt += Vector2.down * moveDownOffset;
    }

    private void OnValidate()
    {
        Type = PlatformBase.Type.Moving;
    }
}
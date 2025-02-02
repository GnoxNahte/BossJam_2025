using UnityEngine;

public class Dummy : EntityBase
{
    private Animator _animator;
    
    private static readonly int OnHit = Animator.StringToHash("OnHit");
    private static readonly int OnHitTop = Animator.StringToHash("OnHitTop");

    protected override void Awake()
    {
        base.Awake();
        
        _animator = GetComponent<Animator>();
    }

    public override void TakeDamage(int damage, Vector2 position)
    {
        base.TakeDamage(damage, position);
        
        if (position.y < transform.position.y)
            _animator.SetTrigger(OnHit);
        else
            _animator.SetTrigger(OnHitTop);
    }
}

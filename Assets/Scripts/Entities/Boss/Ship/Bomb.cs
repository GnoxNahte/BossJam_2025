using System;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private ObjectPool _objectPool;
    private static readonly int AnimId_OnExplode = Animator.StringToHash("OnExplode");
    private static readonly int AnimId_Reset = Animator.StringToHash("Reset");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _animator.SetTrigger(AnimId_Reset);
        _rb.simulated = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _animator.SetTrigger(AnimId_OnExplode);
        _rb.linearVelocity = Vector2.zero;
        _rb.simulated = false;
    }

    public void OnAnimationFinish()
    {
        _objectPool.Release(gameObject);
    }

    public void Init(Vector2 initalVelocity, ObjectPool objectPool)
    {
        _rb.linearVelocity = initalVelocity;
        _objectPool = objectPool;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : MonoBehaviour
{
    [field: SerializeField] public Vector2 PlayerKnockbackSpeed { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    
    [SerializeField] private Shockwave shockwave;

    private Animator _animator;
    private Rigidbody2D _rb;
    private ObjectPool _objectPool;

    private Transform _followTarget;
    
    private WaitForSeconds _shockwaveWait;

    public void ShootToTarget(Vector2 target)
    {
        _followTarget = null;
        
        _rb.linearVelocity = target - (Vector2)transform.position;
        
        transform.parent = null;
    }

    public void SetFollowTarget(Transform target, ObjectPool pool)
    {
        _followTarget = target;
        _objectPool = pool;
    }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        
        _shockwaveWait = new WaitForSeconds(shockwave.ShockwaveTime);
    }

    private void Update()
    {
        if (_followTarget)
            transform.right = _followTarget.position - transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        shockwave.gameObject.SetActive(true);
        _rb.linearVelocity = Vector2.zero;
        StartCoroutine(WaitShockwave());
    }

    private IEnumerator WaitShockwave()
    {
        yield return _shockwaveWait;
        
        transform.parent = _objectPool?.transform;
        _objectPool.Release(gameObject);
    }
}

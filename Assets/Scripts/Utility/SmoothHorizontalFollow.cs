using System;
using UnityEngine;

public class SmoothHorizontalFollow : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector2 horizontalClamp = new Vector2(-float.MinValue, float.MaxValue);
    #endregion
    
    #region Private Variables
	private Transform _target;
    
    private float _velocity;
    #endregion
    
    #region Public Methods
    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetTargetAndPosition(Transform target)
    {
        SetTarget(target);
        transform.position = _target.position;
    }
    #endregion
    
    #region Unity Methods

    private void Update()
    {
        float targetPosX = Mathf.Clamp(_target.position.x, horizontalClamp.x, horizontalClamp.y);
        transform.position = new Vector3(
            Mathf.SmoothDamp(transform.position.x, targetPosX, ref _velocity, smoothTime),
            _target.position.y,
            _target.position.z
        );
    }

    #endregion
}

using System;
using UnityEngine;

public class SmoothVerticalFollow : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private float smoothTime;
    [SerializeField] private float maxHeight = float.MaxValue;
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
        float targetHeight = Mathf.Min(_target.position.y, maxHeight);
        transform.position = new Vector3(
            _target.position.x,
            Mathf.SmoothDamp(transform.position.y, targetHeight, ref _velocity, smoothTime),
            _target.position.z
        );
    }

    #endregion
    
    #region Private Methods
    
    #endregion
}

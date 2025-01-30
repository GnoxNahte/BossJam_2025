using System;
using UnityEngine;
using VInspector;

public class ParallaxBackground : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] private float yParallaxScale = 1f;
    [SerializeField] private float xScrollSpeed = 0f;
    #endregion
    
    #region Private Variables
    private float _imageWidth;
    private float _parallaxAmt;
    private float _yStartHeight;

    private float _xScrollOffset;

    private Camera _cam;
    private SpriteRenderer _sr;
    #endregion

    #region Public Methods

    [Button]
    public void ResetSpriteSize()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.size = _sr.sprite.bounds.size;
    }

    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();

        _cam = Camera.main;
        //if (cam.transform.position.z - transform.position.z < cam.nearClipPlane)
        //{
        //    Debug.LogError("Background is lesser than camera's near clip plane");
        //}
        //else if (cam.transform.position.z - transform.position.z > cam.farClipPlane)
        //{
        //    Debug.LogError("Background is further than camera's far clip plane");
        //}
    }

    private void Start()
    {
        _imageWidth = _sr.size.x;
        // TODO: Calculate width using camera orthographic size 
        // Consider the distance between the background and far clip plane
        // Make sure it's an odd number to make sure a full image is in the middle.
        _sr.size = new Vector2(_imageWidth * 7, _sr.size.y);

        // Uses far clip plane here but might just use a serialized variable next time if need to extend the far clip plane in the future.
        _parallaxAmt = Mathf.Clamp01(transform.position.z / (_cam.farClipPlane + _cam.transform.position.z));

        _yStartHeight = transform.position.y;
    }

    private void Update()
    {
        _xScrollOffset = (_xScrollOffset + xScrollSpeed * Time.deltaTime) % _imageWidth;
        
        Vector2 camPos = _cam.transform.position;
        int repeatCount = (int)(camPos.x / _imageWidth);
        transform.position = new Vector2(
            (camPos.x * _parallaxAmt) % _imageWidth + _xScrollOffset + repeatCount * _imageWidth,
            camPos.y * _parallaxAmt * yParallaxScale + _yStartHeight
        );
    }

    #endregion
}

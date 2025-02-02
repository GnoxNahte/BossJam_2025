using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using VInspector;

public class Shockwave : MonoBehaviour
{
    public float ShockwaveTime => shockwaveTime;
    [SerializeField] private float shockwaveTime;
    [SerializeField] private Vector2 strengthRange; // Not using slider because might be reversed (X bigger than Y)
    [SerializeField] [MinMaxSlider(0f, 1f)] 
    private Vector2 distRange;
    [SerializeField] private float impulseForce;
    
    private SpriteRenderer _sr;
    private Material _mat;
    private CinemachineImpulseSource _impulseSource;
    
    private Coroutine _shockwaveCoroutine = null;
    private ObjectPool _objectPool;

    private static readonly int ShaderID_ShockwaveStrength = Shader.PropertyToID("_ShockwaveStrength");
    private static readonly int ShaderID_DistFromCenter = Shader.PropertyToID("_DistFromCenter");

    public void Init(ObjectPool objectPool)
    {
        _objectPool = objectPool;
    }
    
    private void OnEnable()
    {
        Debug.Assert(_shockwaveCoroutine == null);
        _shockwaveCoroutine = StartCoroutine(ShockwaveCoroutine());
    }

    private void OnDisable()
    {
        if (_shockwaveCoroutine != null)
        {
            StopCoroutine(_shockwaveCoroutine);
            _shockwaveCoroutine = null;
        }
    }

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _mat = _sr.material;
        
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private IEnumerator ShockwaveCoroutine()
    {
        _impulseSource.GenerateImpulse(impulseForce);
        
        _mat.SetFloat(ShaderID_DistFromCenter, 0f);
        float timeLeft = shockwaveTime;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            
            float percentage = 1 - timeLeft / shockwaveTime;
            _mat.SetFloat(ShaderID_DistFromCenter, Utility.Remap(percentage, Vector2.up, distRange));
            _mat.SetFloat(ShaderID_ShockwaveStrength, Utility.Remap(percentage, Vector2.up, strengthRange));
            yield return null;
        }

        _shockwaveCoroutine = null;
        
        gameObject.SetActive(false);
        
        _objectPool?.Release(gameObject);
    }
}

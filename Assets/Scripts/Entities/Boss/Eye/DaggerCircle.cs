using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[RequireComponent(typeof(ObjectPool))]
public class DaggerCircle : MonoBehaviour
{
    public float TotalDaggerSpawnTime => daggerCount * timeBetweenDaggerSpawn.Evaluate(1) + moveTiming * 2f; 
    public int DaggerCount => daggerCount;
    
    [Serializable]
    class DaggerInfo
    {
        public Dagger Dagger;
        
        // Angle
        public float CurrAngle;
        public float TargetAngle;
        public float CurrAngleSpeed;
        
        // Dist
        public float CurrDistFromCenter;
        public float CurrDistSpeed;

        public DaggerInfo(Dagger dagger)
        {
            Dagger = dagger;
            CurrAngle = TargetAngle = 0;
            CurrAngleSpeed = 0;
        }
    }
    
    #region Serialized Variables
    
    [SerializeField] private int daggerCount;
    [SerializeField] private float radiusSpeed;
    [SerializeField] private float moveTiming;
    [SerializeField] private float daggerDistFromCenter;
    [SerializeField] private AnimationCurve timeBetweenDaggerSpawn;
    [SerializeField] [ReadOnly] private List<DaggerInfo> currDaggers;
    #endregion
    
    #region Private Variables

    private ObjectPool _daggerPool;
    private Player _player;
    private BossEye _bossEye;
    
    private WaitForSeconds _daggerMoveWait;
    private Coroutine _spawnCoroutine;

    private float _currCircleAngle;
	
    #endregion
    
    #region Public Methods

    public void Init(Player player, BossEye bossEye)
    {
        _player = player;
        _bossEye = bossEye;
    }

    public void SpawnDaggers()
    {
        // currDaggers.Clear();
        if (_spawnCoroutine != null)
        {
            Debug.LogWarning("Dagger spawn called while spawning daggers.");
            StopCoroutine(_spawnCoroutine);
        }
        _spawnCoroutine = StartCoroutine(SpawnDaggerCoroutine());
    }

    public void ShootDagger(Transform target)
    {
        if (currDaggers.Count == 0)
            return;
        
        int lastIndex = currDaggers.Count - 1;
        var dagger = currDaggers[lastIndex];
        currDaggers.RemoveAt(lastIndex);
        
        dagger.Dagger.ShootToTarget(target.position);
        
        if (currDaggers.Count == 0)
            _bossEye.OnShootingDaggersDone();
    }
    
    public void OnDaggerHit(Dagger dagger, bool ifShot)
    {
        if (!ifShot)
        {
            int index = currDaggers.FindIndex(d => d.Dagger == dagger);
            currDaggers.RemoveAt(index);
        }
        dagger.transform.SetParent(transform);
        dagger.transform.localPosition = Vector3.zero;

        UpdateDaggerAngles();
    }

    public void ReleaseDagger(Dagger dagger)
    {
        _daggerPool.Release(dagger.gameObject);
    }
    #endregion
    
    #region Unity Methods

    private void Awake()
    {
        _daggerPool = GetComponent<ObjectPool>();
        
        _daggerMoveWait = new WaitForSeconds(moveTiming);
    }

    private void Update()
    {
        foreach (var dagger in currDaggers)
        {
            Transform t = dagger.Dagger.transform;
            dagger.CurrDistFromCenter = Mathf.SmoothDamp(dagger.CurrDistFromCenter, daggerDistFromCenter, ref dagger.CurrDistSpeed, moveTiming);
            dagger.CurrAngle = Mathf.SmoothDampAngle(dagger.CurrAngle, dagger.TargetAngle, ref dagger.CurrAngleSpeed, moveTiming);
            float angleInRadian = dagger.CurrAngle * Mathf.Deg2Rad;
            t.localPosition = new Vector2(Mathf.Cos(angleInRadian), Mathf.Sin(angleInRadian)) * dagger.CurrDistFromCenter;
            // t.localPosition = Vector2.SmoothDamp(t.localPosition,
            //                                     dagger.TargetLocalPosition, 
            //                                     ref dagger.CurrVelocity, 
            //                                     moveTiming);
        }
        
        _currCircleAngle += Time.deltaTime * radiusSpeed;
        transform.rotation = Quaternion.Euler(0, 0, _currCircleAngle);
    }
    #endregion
    
    #region Private Methods

    private IEnumerator SpawnDaggerCoroutine()
    {
        while (currDaggers.Count < daggerCount)
        {
            AddDagger();
            yield return new WaitForSeconds(timeBetweenDaggerSpawn.Evaluate(currDaggers.Count));
        }

        yield return _daggerMoveWait;
        yield return _daggerMoveWait;
        yield return _daggerMoveWait;
        
        _bossEye.OnSpawnDaggersDone();
        _spawnCoroutine = null;
    }

    [Button]
    private void AddDagger()
    {
        GameObject newDagger = _daggerPool.Get(transform.position);
        Dagger dagger = newDagger.GetComponent<Dagger>();
        dagger.SetFollowTarget(_player.transform, this);
        currDaggers.Add(new DaggerInfo(dagger));

        UpdateDaggerAngles();
    }

    private void UpdateDaggerAngles()
    {
        // Calculate target angle
        for (int i = 0; i < currDaggers.Count; i++)
        {
            float angle = i * 360f / currDaggers.Count;
            // currDaggers[i].TargetLocalPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * daggerDistFromCenter;
            currDaggers[i].TargetAngle = angle;
        }
    }
    #endregion
}

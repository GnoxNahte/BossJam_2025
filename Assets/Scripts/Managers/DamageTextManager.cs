using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class DamageTextManager : MonoBehaviour
{
    private ObjectPool _objectPool;

    // Singleton
    public static DamageTextManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogError("More than 1 DamageTextManager. Destroying this. Name: " + name);
            return;
        }
        
        _objectPool = GetComponent<ObjectPool>();
    }

    public static void OnDamage(int damage, Vector2 position, bool isPlayer)
    {
        GameObject damageTextGO = Instance._objectPool.Get(position);
        damageTextGO.GetComponent<DamageText>().SetDamageText(damage, isPlayer, Instance._objectPool);
        damageTextGO.transform.position = new Vector3(position.x, position.y, -0.1f);
    }
}

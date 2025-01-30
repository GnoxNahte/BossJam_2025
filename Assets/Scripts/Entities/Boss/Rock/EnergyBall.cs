using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    #region Serialized Variables
    [field: SerializeField] public Vector2 PlayerKnockbackSpeed { get; private set; }
    [field: SerializeField] public int Damage { get; private set; }
    #endregion
    
    #region Private Variables
	
    private Animator _animator;
    private Rigidbody2D _rb;
    private ObjectPool _objectPool;
    private static readonly int AnimId_OnExplode = Animator.StringToHash("OnExplode");
    private static readonly int AnimId_Reset = Animator.StringToHash("Reset");
    #endregion
    
    #region Public Methods
    public void Init(Vector2 initalVelocity)
    {
        
    }
    #endregion
    
    #region Unity Methods
    
    #endregion
    
    #region Private Methods
    
    #endregion
}

using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] private SpriteRenderer sr;
    #endregion
    
    #region Private Variables
	private Animator _animator;
    
    // Anim IDs
    private readonly int _attackAnimID = Animator.StringToHash("OnAttack");
    private readonly int _dashAnimID = Animator.StringToHash("OnDash");
    
    #endregion
    
    #region Public Methods
    // ReSharper triggers for Debug.LogError
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnActivateAbility(PlayerAbilitySystem.Type type)
    {
        switch (type)
        {
            case PlayerAbilitySystem.Type.Attack: _animator.SetTrigger(_attackAnimID); break;
            case PlayerAbilitySystem.Type.Dash: _animator.SetTrigger(_dashAnimID); break;
            default: Debug.LogError("Player Animator: Activating unknown ability - " + type); break;
        }
    }

    public void SetColor(Color color)
    {
        sr.color = color;
    }
    
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

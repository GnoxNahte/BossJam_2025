using UnityEngine;
using VInspector;

public class AbilitySystem : MonoBehaviour
{
    #region Serialized Variables
    [field: SerializeField] 
    public SerializedDictionary<PlayerAbilitySystem.Type, PlayerAbilityBase> Abilities { get; protected set; }
    #endregion
}

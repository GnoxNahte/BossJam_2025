using UnityEngine;
using VInspector;

public class AbilitySystem<T> : MonoBehaviour where T : System.Enum
{
    #region Serialized Variables
    [field: SerializeField] 
    public SerializedDictionary<T, AbilityBase<T>> Abilities { get; protected set; }
    #endregion
}

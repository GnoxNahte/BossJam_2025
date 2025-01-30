using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Values")]
    [field: SerializeField] public Color SkyColor { get; private set; }
    [field: SerializeField] public string CurrSceneName { get; private set; }
    [field: SerializeField] public string NextSceneName { get; private set; }
    
    [Header("References")]
    [field: SerializeField] public Collider2D CameraConfier { get; private set; }
    [field: SerializeField] public GameObject LeftBorder { get; private set; }
    [field: SerializeField] public GameObject RightBorder { get; private set; }
    [field: SerializeField] public Transform PlayerStart { get; private set; }
    [field: SerializeField] public Transform LevelCenter { get; private set; }
}

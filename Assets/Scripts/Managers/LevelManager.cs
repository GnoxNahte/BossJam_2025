using UnityEngine;
using VInspector;

public class LevelManager : MonoBehaviour
{
    [Header("Values")]
    [field: SerializeField] public Color SkyColor { get; private set; }
    [field: SerializeField] public string CurrSceneName { get; private set; }
    [field: SerializeField] public string NextSceneName { get; private set; }
    [field: SerializeField] public bool IfStaticCamera { get; private set; }
    [Header("References")]
    
    [field: ShowIf("IfStaticCamera")]
    [field: SerializeField] public Transform StaticCameraFollow { get; private set; }
    [field: HideIf("IfStaticCamera")]
    [field: SerializeField] public Collider2D CameraConfier { get; private set; }
    [field: EndIf]
    [field: SerializeField] public GameObject LeftBorder { get; private set; }
    [field: SerializeField] public GameObject RightBorder { get; private set; }
    [field: SerializeField] public Transform PlayerStart { get; private set; }
    [field: SerializeField] public Transform LevelCenter { get; private set; }
}

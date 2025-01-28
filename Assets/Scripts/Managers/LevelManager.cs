using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [field: SerializeField] public Collider2D CameraConfier { get; private set; }
    [field: SerializeField] public GameObject LeftBorder { get; private set; }
    [field: SerializeField] public GameObject RightBorder { get; private set; }
    [field: SerializeField] public Transform PlayerStart { get; private set; }
}

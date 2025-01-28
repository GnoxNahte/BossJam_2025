using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] private CinemachineCamera gameCamera;
    // [SerializeField] private CinemachineCamera overviewCamera;
    
    #endregion
    
    #region Public Methods
    public void Init(Player player, Collider2D cameraConfiner)
    {
        gameCamera.Follow = player.transform;
        gameCamera.GetComponent<CinemachineConfiner2D>().BoundingShape2D = cameraConfiner;
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

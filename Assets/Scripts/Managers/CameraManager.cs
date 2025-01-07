using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] private CinemachineCamera gameCamera;
    // [SerializeField] private CinemachineCamera overviewCamera;
    
    #endregion
    
    #region Public Methods
    public void Init(Player player)
    {
        gameCamera.Follow = player.transform;
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

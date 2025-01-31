using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Serialized Variables

    [SerializeField] private CinemachineCamera gameCamera;
    // [SerializeField] private CinemachineCamera overviewCamera;
    
    #endregion
    
    #region Public Methods
    public void Init(Player player, LevelManager levelManager)
    {
        gameCamera.Follow = player.transform;

        if (levelManager.IfStaticCamera)
            gameCamera.Follow = levelManager.StaticCameraFollow;
        else
            gameCamera.GetComponent<CinemachineConfiner2D>().BoundingShape2D = levelManager.CameraConfier;
        if (Camera.main)
            Camera.main.backgroundColor = levelManager.SkyColor;
    }
    #endregion
    
    #region Private Methods
    
    #endregion
}

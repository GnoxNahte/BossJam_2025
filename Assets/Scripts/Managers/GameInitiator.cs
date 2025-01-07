using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitiator : MonoBehaviour
{
    #region Public Variables

    [Header("Prefabs")]
    [Header("Managers")]
    public GameObject InputManagerPrefab;
    public GameObject CameraManagerPrefab;
    
    [Header("Game Objects")]
    public GameObject Player;

    [Header("Temp")] 
    [SerializeField] private Transform playerStart;
    
    #endregion
    
    #region Unity Methods
    private async void Start()
    {
        await LoadGame();
    }
    #endregion
    
    #region Private Methods
    private async Awaitable LoadGame()
    {
        await InstantiatePrefabs();
        
        // await SceneManager.LoadSceneAsync("Boss 1", LoadSceneMode.Additive);
        await SceneManager.LoadSceneAsync("TestLevel", LoadSceneMode.Additive);
    }

    private async Awaitable InstantiatePrefabs()
    {
        // === Instantiate Objects ===
        GameObject managerParent = new GameObject("Managers");
        // GameObject inputManagerGO = (await InstantiateAsync(InputManagerPrefab, managerParent.transform))[0];
        GameObject inputManagerGO = Instantiate(InputManagerPrefab, managerParent.transform); 
        GameObject cameraManagerGO = Instantiate(CameraManagerPrefab, managerParent.transform); 
        GameObject playerGO = Instantiate(Player, playerStart.position, Quaternion.identity);
        
        // === Get relevant components ===
        InputManager inputManager = inputManagerGO.GetComponent<InputManager>();
        CameraManager cameraManager = cameraManagerGO.GetComponent<CameraManager>();
        Player player = playerGO.GetComponent<Player>();
        
        // === Init Objects ===
        player.Init(inputManager);
        cameraManager.Init(player);
    }
    #endregion
}

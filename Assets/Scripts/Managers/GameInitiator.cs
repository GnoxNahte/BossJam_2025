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
    [Header("UI")] 
    public Transform Canvas;
    public GameObject GameUIManagerPrefab;
    
    [Header("Game Objects")]
    public GameObject Player;
    public GameObject BorderTriggersPrefab;

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
        // await SceneManager.LoadSceneAsync("Boss 1", LoadSceneMode.Additive);
        if (!SceneManager.GetSceneByName("TestLevel").isLoaded && !SceneManager.GetSceneByName("MainMenuLevel").isLoaded)
            await SceneManager.LoadSceneAsync("MainMenuLevel", LoadSceneMode.Additive);
            
        await InstantiatePrefabs();
    }

    private async Awaitable InstantiatePrefabs()
    {
        print("Init prefabs");
        // === Instantiate Objects ===
        GameObject managerParent = new GameObject("Managers");
        // GameObject inputManagerGO = (await InstantiateAsync(InputManagerPrefab, managerParent.transform))[0];
        GameObject inputManagerGO = Instantiate(InputManagerPrefab, managerParent.transform); 
        GameObject cameraManagerGO = Instantiate(CameraManagerPrefab, managerParent.transform); 
        GameObject gameUIManagerGO = Instantiate(GameUIManagerPrefab, Canvas.transform); 
        GameObject playerGO = Instantiate(Player, playerStart.position, Quaternion.identity);
        
        // === Get relevant components ===
        InputManager inputManager = inputManagerGO.GetComponent<InputManager>();
        CameraManager cameraManager = cameraManagerGO.GetComponent<CameraManager>();
        GameUIManager gameUIManager = gameUIManagerGO.GetComponent<GameUIManager>();
        Player player = playerGO.GetComponent<Player>();
        
        // === Init Objects ===
        player.Init(inputManager);
        cameraManager.Init(player);
        gameUIManager.Init(player.PlayerAbilitySystem);
        inputManager.Init(player.PlayerAbilitySystem);
        
        // === Init other objects ===
        Camera mainCamera = Camera.main;
        Debug.Assert(mainCamera);
        
        Vector3 leftBorderPos = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
        GameObject leftBorder = Instantiate(BorderTriggersPrefab, leftBorderPos, Quaternion.identity, mainCamera.transform);
        
        Vector3 rightBorderPos = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0));
        GameObject rightBorder = Instantiate(BorderTriggersPrefab, rightBorderPos, Quaternion.identity, mainCamera.transform);

        BossShip boss = FindAnyObjectByType<BossShip>();
        if (boss)
            boss.Init(player, leftBorder, rightBorder);
        else 
            print("Can't find boss ship");
    }
    #endregion
}

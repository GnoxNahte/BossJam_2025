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
    public GameObject DamageManagerPrefab;
    [Header("UI")] 
    public Transform Canvas;
    public GameObject GameUIManagerPrefab;
    
    [Header("Game Objects")]
    public GameObject Player;
    
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
        bool isMainMenuLoaded = SceneManager.GetSceneByName("MainMenuLevel").isLoaded;
        
        if (SceneManager.loadedSceneCount <= 1 && !isMainMenuLoaded)
        {
            await SceneManager.LoadSceneAsync("MainMenuLevel", LoadSceneMode.Additive);
            isMainMenuLoaded = true;
        } 
        
        await InstantiatePrefabs(isMainMenuLoaded);
    }

    private async Awaitable InstantiatePrefabs(bool isMainMenuLoaded)
    {
        print("Init prefabs");
        // === Instantiate Objects ===
        GameObject managerParent = new GameObject("Managers");
        // GameObject inputManagerGO = (await InstantiateAsync(InputManagerPrefab, managerParent.transform))[0];
        GameObject inputManagerGO = Instantiate(InputManagerPrefab, managerParent.transform); 
        GameObject cameraManagerGO = Instantiate(CameraManagerPrefab, managerParent.transform); 
        GameObject damageTextManageGO = Instantiate(DamageManagerPrefab, managerParent.transform);
        GameObject gameUIManagerGO = Instantiate(GameUIManagerPrefab, Canvas.transform);
        GameObject playerGO = Instantiate(Player);
        
        // === Get relevant components ===
        InputManager inputManager = inputManagerGO.GetComponent<InputManager>();
        CameraManager cameraManager = cameraManagerGO.GetComponent<CameraManager>();
        GameUIManager gameUIManager = gameUIManagerGO.GetComponent<GameUIManager>();
        Player player = playerGO.GetComponent<Player>();
        
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        
        // === Init Objects ===
        player.Init(inputManager, gameUIManager);
        playerGO.transform.position = levelManager.PlayerStart.position;
        cameraManager.Init(player, levelManager.CameraConfier);
        gameUIManager.Init(isMainMenuLoaded);
        inputManager.Init(player.PlayerMovement);
        
        // === Init other objects ===
        BossShip boss = FindAnyObjectByType<BossShip>();
        if (boss)
            boss.Init(player, levelManager.LeftBorder, levelManager.RightBorder, gameUIManager.BossHealthUI);
        else 
            print("Can't find boss ship");
    }
    #endregion
}

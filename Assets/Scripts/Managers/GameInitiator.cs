using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;

public class GameInitiator : MonoBehaviour
{
    #region Public Variables

    [Header("Prefabs")]
    [Header("Managers")]
    public GameObject InputManagerPrefab;
    public GameObject CameraManagerPrefab;
    public GameObject DamageManagerPrefab;
    public GameObject AudioManagerPrefab;
    [Header("UI")] 
    public Transform Canvas;
    public GameObject GameUIManagerPrefab;
    
    [Header("Game Objects")]
    public GameObject Player;

    [ShowInInspector]
    public static bool IsHardMode;
    [ShowInInspector]
    public static bool IsGameCleared;
    public static bool IsGameCleared_Hard;
    
    #endregion

    public static void OnGameCleared(bool onHard)
    {
        GameInitiator.IsGameCleared = true;
        PlayerPrefs.SetInt("IsGameCleared", 1);
        PlayerPrefs.SetInt("IsGameCleared_Hard", onHard ? 1 : 0);
    }
    
    #region Unity Methods

    private void Awake()
    {
        IsGameCleared = PlayerPrefs.GetInt("IsGameCleared", 0) == 1;
        IsGameCleared_Hard = PlayerPrefs.GetInt("IsGameCleared_Hard", 0) == 1;
    }

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
        
        if (AudioManager.Instance == null)
            Instantiate(AudioManagerPrefab, managerParent.transform);
        
        // === Get relevant components ===
        InputManager inputManager = inputManagerGO.GetComponent<InputManager>();
        CameraManager cameraManager = cameraManagerGO.GetComponent<CameraManager>();
        GameUIManager gameUIManager = gameUIManagerGO.GetComponent<GameUIManager>();
        Player player = playerGO.GetComponent<Player>();
        
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        Cutscenes cutscenes = FindFirstObjectByType<Cutscenes>(FindObjectsInactive.Include);
        
        // === Init Objects ===
        print("Init objs");
        player.Init(inputManager, gameUIManager);
        playerGO.transform.position = levelManager.PlayerStart.position;
        cameraManager.Init(player, levelManager);
        gameUIManager.Init(isMainMenuLoaded, levelManager);
        inputManager.Init(player.PlayerMovement, cutscenes);
        
        // === Init other objects ===
        print("Init other objs");
        BossShip bossShip = FindAnyObjectByType<BossShip>();
        if (bossShip)
            bossShip.Init(player, levelManager.LeftBorder, levelManager.RightBorder, gameUIManager.BossHealthUI);
        
        BossRock bossRock = FindAnyObjectByType<BossRock>();
        if (bossRock)
            bossRock.Init(player, levelManager.LevelCenter, gameUIManager.BossHealthUI);
        
        BossEye bossEye = FindAnyObjectByType<BossEye>();
        if (bossEye)
            bossEye.Init(player, gameUIManager.BossHealthUI);
    }
    #endregion
}

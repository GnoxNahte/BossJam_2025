using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TransitionToGame : MonoBehaviour
{
    [SerializeField] private string sceneLevelName;
    [SerializeField] private bool ifAllowSubstituteLevel = false;
    #region Unity Methods

    public async void TriggerTransition()
    {
        await LoadNextScene();
    }

    public void SetTransitionSceneName(string sceneName)
    {
        sceneLevelName = sceneName;
    }

    private async void OnTriggerEnter2D(Collider2D other)
    {
        await LoadNextScene();
    }

    private async Awaitable LoadNextScene()
    {
        if (ifAllowSubstituteLevel)
        {
            // Get keyboard
            var devices = InputSystem.devices;
            Keyboard keyboard = null;
            for (int i = 0; i < devices.Count; i++)
            {
                InputDevice device = devices[i];
                if (device is Keyboard tmpKeyboard)
                    keyboard = tmpKeyboard;
            }

            if (keyboard != null)
            {
                if (keyboard.digit1Key.isPressed)
                    sceneLevelName = "BossShip";
                else if (keyboard.digit2Key.isPressed)
                    sceneLevelName = "BossRock";
                else if (keyboard.digit3Key.isPressed)
                    sceneLevelName = "BossFinal";
                print("Sub scene loaded: " + sceneLevelName + " | " + keyboard.digit2Key.wasPressedThisFrame + " | " + keyboard.digit2Key.isPressed);
            }
            else
            {
                Debug.LogWarning("Can't find keyboard");
            }
        }
        int sceneCount = SceneManager.loadedSceneCount;
        for (int i = 0; i < sceneCount - 1; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            SceneManager.UnloadSceneAsync(scene);
            print("Unload scene:" + scene.name);
        }
        print("Load Game");
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        print("Load Scene: " + sceneLevelName);
        SceneManager.LoadScene(sceneLevelName, LoadSceneMode.Additive);
        // await SceneManager.LoadSceneAsync("TestLevel", LoadSceneMode.Additive);
        print("Done");
    }

    #endregion
}

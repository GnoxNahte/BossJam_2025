using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionToGame : MonoBehaviour
{
    #region Unity Methods

    private async void OnTriggerEnter2D(Collider2D other)
    {
        await LoadNextScene();
    }

    private async Awaitable LoadNextScene()
    {
        int sceneCount = SceneManager.loadedSceneCount;
        for (int i = 0; i < sceneCount - 1; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            SceneManager.UnloadSceneAsync(scene);
            print("Unload scene:" + scene.name);
        }
        print("Load Game");
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        print("Load Scene");
        SceneManager.LoadScene("TestLevel", LoadSceneMode.Additive);
        // await SceneManager.LoadSceneAsync("TestLevel", LoadSceneMode.Additive);
        print("Done");
    }

    #endregion
}

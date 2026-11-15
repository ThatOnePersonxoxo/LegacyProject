using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleporterB : MonoBehaviour
{
    public string currentScene = "Moon Scene";
    public string destinationScene = "Mars Scene";

    public void Teleport()
    {
        StartCoroutine(LoadDestination());
    }

    IEnumerator LoadDestination()
    {
        // Load Mars first
        AsyncOperation loadOperation =
            SceneManager.LoadSceneAsync(destinationScene, LoadSceneMode.Additive);

        yield return loadOperation;

        // Set Mars as the active scene
        SceneManager.SetActiveScene(
            SceneManager.GetSceneByName(destinationScene)
        );

        Physics.SyncTransforms();

        // Unload Moon
        yield return SceneManager.UnloadSceneAsync(currentScene);
    }
}

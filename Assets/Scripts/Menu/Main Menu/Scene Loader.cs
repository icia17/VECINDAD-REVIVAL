using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation asyncOperation;
    private string currentSceneName;

    public void PreloadScene(string sceneName)
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {

            if (asyncOperation.progress >= 0.9f)
            {
                yield break;
            }

            yield return null;
        }
    }

    public void ActivateScene()
    {
        if (asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
            StartCoroutine(UnloadCurrentScene());
        }
    }

    private IEnumerator UnloadCurrentScene()
    {

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentSceneName);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnable : MonoBehaviour
{
    public string loadScene;
    public bool additive=false; //not done yet.
    public string tagName = "Default";

    public bool resetGame;
    public bool resetLevel;
   
    [Header("Advance")]
    public bool unloadScene;
    private void OnEnable()
    {
        if(unloadScene)
        {
            DoUnloadScene();
        }
        else
        {
            DoLoadScene();
        }
    }
    void DoUnloadScene()
    {
        if (GameManager.instance == null)
        {
            StartCoroutine(UnloadAScene(loadScene));
        }
        else
        {
            GameManager.instance.StartCoroutine(UnloadAScene(loadScene));
        }
    }
    void DoLoadScene()
    {
        if (GameManager.instance == null)
        {
            LoadWithoutGameManager();
        }
        else
        {
            LoadViaGameManager();
        }
    }
    void LoadViaGameManager()
    {
        if (resetGame)
        {
            GameManager.instance.RebootGame();
            return;
        }
        if (resetLevel)
        {
            GameManager.instance.RestartLevel();
            return;
        }
        GameManager.instance.LoadScene(loadScene, tagName, additive);
    }
    void LoadWithoutGameManager()
    {
        StartCoroutine(LoadAScene(loadScene,additive));
    }
    IEnumerator LoadAScene(string sceneName, bool additive)
    {

        float waitTime = 0.5f;
        CameraFade.instance.StartFade(Color.black, waitTime, false);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (additive == false)
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(currentSceneIndex);
            while (!unload.isDone)
            {
                yield return null;
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
        waitTime = 0.5f;
        CameraFade.instance.StartFade(Color.black, waitTime, true);
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    IEnumerator UnloadAScene(string sceneName)
    {

        float waitTime = 0.5f;
        CameraFade.instance.StartFade(Color.black, waitTime, false);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
   
        waitTime = 0.5f;
        CameraFade.instance.StartFade(Color.black, waitTime, true);
        while (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}

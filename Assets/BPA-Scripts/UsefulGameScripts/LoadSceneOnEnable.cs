using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnable : MonoBehaviour
{
    public string loadScene;
    public bool additive=false; //not done yet.
    public bool resetGame;
    public bool resetLevel;
    public string tagName = "Default";
    private void OnEnable()
    {
        if(GameManager.instance==null)
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

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*
     * Add any sort of "gloabal" system object references here
     */
    public static GameManager instance;
    public GameObject Player;
    public PlayerMovementBPA PlayerMovement;
    public bool pauseMovementOnReadDialogue=true;

    private System.Random rand = new System.Random();

    public System.Random RandomMain()
    {
        return rand;
    }

    private void Awake()
    {
        instance = this;
    }
    void OnDialogueRead()
    {
        if (!pauseMovementOnReadDialogue) return;
       if(PlayerMovement!=null) PlayerMovement.enabled = false;
    }
    void OnDialogueEnd()
    {
        if (PlayerMovement != null) PlayerMovement.enabled = transform;
    }
    public void ShowPlayer(bool playerActive)
    {
        //Disable everything you wouldn't need in a scene that doesn't have the player. use for things like title screen and
        //custom mini games
        Player.gameObject.SetActive(playerActive);
    }
    public void SetPlayerLocation(Vector3 location)
    {
        Player.transform.position = location;
    }
    public void SetPlayerRotation(Quaternion rotation)
    {
        Player.transform.rotation = rotation;
    }
    Action OnRead;
    Action OnReadEnd;
    // Start is called before the first frame update
    void Start()
    {
        OnRead = () => OnDialogueRead();
        OnReadEnd = () => OnDialogueEnd();
        if (ScreenplayReader.instance!=null)
        {
            ScreenplayReader.instance.SubscribeOnDialogeRead(OnRead);
            ScreenplayReader.instance.SubscribeOnDialogeEnd(OnReadEnd);
        }

    }

    public void LoadScene(string sceneName, bool additive)
    {
        LoadScene(sceneName, string.Empty, additive);
    }
    public void LoadScene(string sceneName, string tagName, bool additive)
    {
        StartCoroutine(LoadAScene(sceneName, tagName, additive));
    }
    public string sceneTagLocationName = string.Empty;
    IEnumerator LoadAScene(string sceneName, string tagName, bool additive)
    {

        float waitTime = 0.5f;
        sceneTagLocationName = tagName;
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

    public void RestartLevel()
    {
        CameraFade.instance.StartFade(Color.black, 1f, false);
        StartCoroutine(RestartScene(1));
    }
    public void RebootGame()
    {
        CameraFade.instance.StartFade(Color.white, 0.3f, false);
        StartCoroutine(RestartScene(0.3f));
    }
    IEnumerator RestartScene(float wait)
    {
        while (wait > 0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
        //todo: erease data here if desired...
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //i'd suggest not using this and capture the starting state of your level instead and loading that data, but this works as well...
        yield break;
    }
    IEnumerator RebootGame(float wait)
    {
        while (wait > 0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
        //go back to 0 scene and do boot proccess again.
        SceneManager.LoadScene(0);
        yield break;
    }


}

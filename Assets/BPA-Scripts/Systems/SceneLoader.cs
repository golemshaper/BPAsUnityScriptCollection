using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BPAScene.Util.Scenes;
public class SceneLoader : MonoBehaviour
{
    //TODO: Change class name!
    //This is more of a bootstrapper. Use game manager for normal scene loading.

    public Camera disableAfterLoad;
    const string coreGameEngineName = "_1CoreEngine"; //IE: Core game objects and game manager
    const string bootToSceneOnceEngineLoaded = "TitleScreen"; //IE: Title Screen
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BootGame());
    }
    IEnumerator BootGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(coreGameEngineName, LoadSceneMode.Additive);
        CameraFade.instance.BlackScreen();
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        //boot starting level...
        Scene setActiveScene;
        if (LevelMemory.levelName == string.Empty)
        {
            LevelMemory.levelName = bootToSceneOnceEngineLoaded;
        }
       
        AsyncOperation asyncLoad2 = SceneManager.LoadSceneAsync(LevelMemory.levelName, LoadSceneMode.Additive);
        setActiveScene = SceneManager.GetSceneByName(LevelMemory.levelName);
        GameManager.instance.ShowPlayer(LevelMemory.includeGameplayObjects);
        disableAfterLoad.gameObject.SetActive(false);
        while (!asyncLoad2.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(setActiveScene);

       /* If you have things in the start up scene you want to disable I'd suggest something like this here:
        * foreach (GameObject g in GameManager.instance.disableIfSceneLoaded)
        {
            if (g != null) g.SetActive(false);
        }*/


        CameraFade.instance.StartFade(Color.black,1f,true);
        yield break;
    }
    
}
namespace BPAScene.Util.Scenes
{
    public static class LevelMemory
    {
        public static string levelName=string.Empty;
        public static bool includeGameplayObjects = true;
    }
}
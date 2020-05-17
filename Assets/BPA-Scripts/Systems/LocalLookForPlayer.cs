using System.Collections;
using System.Collections.Generic;
using BPAScene.Util.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalLookForPlayer : MonoBehaviour
{
    /*
     * I tend to use a version of this class in games for deciding to load the player or not.
     * you can expand on this or not use it all together if you have a better way of working.
     * If you want to do it my way, just place this in your scene and it will load everything you need and then re-load this scene.
     */
    // Start is called before the first frame update
    [Tooltip("Show the Player character and game UI")]
    public bool requestPlayer = true;
    [SerializeField] TagLocation[] allTaggedLocations;
    void Awake()
    {
        if(GameManager.instance==null)
        {
            LevelMemory.levelName = SceneManager.GetActiveScene().name;
            LevelMemory.includeGameplayObjects = requestPlayer;
            SceneManager.LoadScene(0);
            return;
        }
        //IF HAS GAME MANAGER:
        GameManager.instance.SetPlayerLocation(
                    GetTagLocation(GameManager.instance.sceneTagLocationName)
                    );
    }
    Vector3 GetTagLocation(string tagName)
    {
        foreach (TagLocation tl in allTaggedLocations)
        {
            if (tl.locationTagName == tagName)
            {
                tl.OnTagEntered();
                return tl.transform.position;
            }
        }
        return Vector3.zero;
    }
}

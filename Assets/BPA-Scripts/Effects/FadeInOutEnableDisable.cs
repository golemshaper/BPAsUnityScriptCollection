using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOutEnableDisable: MonoBehaviour
{
    public float fadeInTime=1f;
    public float waitTime = 0.1f;
    public float fadeOutime=1f;
    public Color color = Color.white;
    public GameObject[] enable;
    public GameObject[] disable;

    // Start is called before the first frame update
    void OnEnable()
    {
        if(GameManager.instance==null)
        {
            StartCoroutine(FadeInOut());
            return;
        }
        GameManager.instance.StartCoroutine(FadeInOut());
    }
    IEnumerator FadeInOut()
    {
        float wait = fadeInTime;
        //fade in
        CameraFade.instance.StartFade(color, fadeInTime, false);
        while(wait>0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
        wait = waitTime;
        while (wait > 0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
        //activate/deactivate
        foreach (GameObject g in enable)
        {
            if (g != null) g.SetActive(true);
        }
        foreach (GameObject g in disable)
        {
            if (g != null) g.SetActive(false);
        }
        //transition out
        CameraFade.instance.StartFade(Color.white, fadeOutime, true);
        wait = fadeOutime;
        while (wait > 0f)
        {
            wait -= Time.deltaTime;
            yield return null;
        }
       
  
        yield break;
    }
}

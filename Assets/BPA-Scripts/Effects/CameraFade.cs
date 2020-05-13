using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFade : MonoBehaviour
{
    private static CameraFade mInstance = null;
   
    public GUIStyle backgroundStyle = new GUIStyle();                     
    public Texture2D fadeTexture;                                       
    public Color currentScreenOverlayColor = new Color(0, 0, 0, 0);
    public int fadeGUIDepth = -1000;
    //TODO: Make this work
    public List<Event> onFadeInEndEvent= new List<Event>();
    public static CameraFade instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(CameraFade)) as CameraFade;
                if (mInstance == null)
                {
                    mInstance = new GameObject("CameraFade").AddComponent<CameraFade>();
                }
            }
            return mInstance;
        }
    }
    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this as CameraFade;
            instance.Initialize();
        }
    }
    public void Initialize()
    {
        instance.fadeTexture = new Texture2D(1, 1);
        instance.backgroundStyle.normal.background = instance.fadeTexture;
    }
    public float currentOpacity;
    bool fadeInMode = true;
    float curentFadeTime;
    float maxWaitTime;
    float fadeTimeMappedToOne = 0f;

    public bool _testFade;
    public float _testFadeTIme;
    public bool _testFadeMode;

  
    public void StartFade(Color fadeColor, float fadeTime,bool doFadeIn)
    {
        currentScreenOverlayColor = fadeColor;
         fadeInMode = doFadeIn;
        maxWaitTime = fadeTime;
        curentFadeTime = 0;
        holdScreen = false;
    }
    bool holdScreen = false;
    void OnGUI()
    {
        if(holdScreen)
        {
            GUI.depth = instance.fadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), instance.fadeTexture, instance.backgroundStyle);
            return;
        }
        if(_testFade)
        {
            _testFade = false;
            StartFade(Color.black, _testFadeTIme, _testFadeMode);
        }

        if(!fadeInMode)
        {
            curentFadeTime += Time.deltaTime;
            if (curentFadeTime > maxWaitTime) curentFadeTime = maxWaitTime;
            fadeTimeMappedToOne = (curentFadeTime * 100f / maxWaitTime) * 0.01f;

            currentOpacity = fadeTimeMappedToOne;
        }
        else
        {
            curentFadeTime += Time.deltaTime;
            if (curentFadeTime > maxWaitTime) curentFadeTime = maxWaitTime;
            fadeTimeMappedToOne = (curentFadeTime * 100f / maxWaitTime) * 0.01f;

            currentOpacity = Mathf.Lerp(1f,0f, fadeTimeMappedToOne);
        }
        // Only draw the texture when the alpha value is greater than 0:
        instance.currentScreenOverlayColor.a = currentOpacity;
        SetScreenOverlayColor(instance.currentScreenOverlayColor);
       // SetScreenOverlayColor(new Color(0,0,0,currentOpacity));
        if (currentScreenOverlayColor.a > 0)
        {
            
            GUI.depth = instance.fadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), instance.fadeTexture, instance.backgroundStyle);
        }
    }
    private static void SetScreenOverlayColor(Color newScreenOverlayColor)
    {
        instance.currentScreenOverlayColor = newScreenOverlayColor;
        instance.fadeTexture.SetPixel(0, 0, instance.currentScreenOverlayColor);
        instance.fadeTexture.Apply();
    }
    public void Clear()
    {
        SetScreenOverlayColor(new Color(0, 0, 0, 0));
        holdScreen = false;
    }
    public void BlackScreen()
    {
        SetScreenOverlayColor(new Color(0, 0, 0, 1));
        holdScreen = true;

    }
}

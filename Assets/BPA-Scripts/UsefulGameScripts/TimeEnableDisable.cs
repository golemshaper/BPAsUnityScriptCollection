using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEnableDisable : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        limitOnce = false;
        curTime = 0f;
    }
    bool limitOnce;
    public float Timer=1;
    float curTime;
    public GameObject[] enable;
    public GameObject[] disable;
    public bool invert;
    // Update is called once per frame
    void Update()
    {
        if (limitOnce) return;
        if(curTime<Timer)
        {
            curTime += Time.deltaTime;
        }
        if (curTime >= Timer)
        {
            limitOnce = true;
            curTime = 0f;
            foreach (GameObject g2 in enable)
            {
                if (g2 != null) g2.SetActive(!invert);
            }
            foreach (GameObject g2 in disable)
            {
                if (g2 != null) g2.SetActive(invert);
            }
        }
    }
}

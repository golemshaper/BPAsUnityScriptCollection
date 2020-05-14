using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDisableEnableDisable : MonoBehaviour
{
    public GameObject[] enable;
    public GameObject[] disable;
    public bool invert;
    // Update is called once per frame
    void OnDisable()
    {
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
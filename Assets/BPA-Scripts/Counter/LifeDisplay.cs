using BPA.Gamedata;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDisplay : MonoBehaviour
{
    //a generic array is used for lives. if you want a zelda style, 
    //set up objects for hearts side by side if you want zelda or issac style.
    public Counter myCounter;
    public GameObject[] LifeObjects;
    Action myAction;
    int val = 0;
    // Start is called before the first frame update
    void Start()
    {
        myAction = ()=> UpdateDisplay();
        myCounter.SubscribeToValueChange(myAction);
    }
    void UpdateDisplay()
    {
        val = myCounter.Value;
        for(int i=0; i<LifeObjects.Length;i++)
        {
            if(i<=val)
            {
                LifeObjects[i].SetActive(true);
            }
            else
            {
                LifeObjects[i].SetActive(false);
            }
        }
    }
   
}

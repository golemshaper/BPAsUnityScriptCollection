using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagLocation : MonoBehaviour
{
    //Data:
    public string locationTagName;
    public bool setPlayerRotation = true;
    public bool TestGotoLocation;
    public bool ignoreSafety;
    public GameObject[] onEnteredEnable;
    public GameObject[] onEnteredDisable;
    public void OnTagEntered()
    {
        foreach(GameObject g in onEnteredEnable)
        {
            if(g!=null)
            {
                g.SetActive(true);
            }
        }
        foreach (GameObject g in onEnteredDisable)
        {
            if (g != null)
            {
                g.SetActive(false);
            }
        }
    }
    private void Awake()
    {
        //safety
        if(ignoreSafety==false)
            TestGotoLocation = false;
    }
    private void Update()
    {
        if(TestGotoLocation)
        {
            TestGotoLocation = false;
            GameManager.instance.SetPlayerLocation(this.transform.position);
            if(setPlayerRotation)
            {
                GameManager.instance.SetPlayerRotation(this.transform.rotation);
            }
            OnTagEntered();
        }
    }
}

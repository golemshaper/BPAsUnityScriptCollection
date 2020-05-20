using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTest : MonoBehaviour
{
    /*
     * How to use multiple actions
     */

    public Action events;

    // Start is called before the first frame update
    void Start()
    {
        events += DoThing;
        events += DoThing2;
    }
    void DoThing()
    {
        Debug.Log("MOT");
    }
    void DoThing2()
    {
        Debug.Log("1111");
    }
    // Update is called once per frame
    void Update()
    {
        events();
    }
}

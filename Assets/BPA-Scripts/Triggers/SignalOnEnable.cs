using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SignalOnEnable : MonoBehaviour
{
   
    public List<Action> actions = new List<Action>();
    public void SubscribeToOnEnable(Action a)
    {
        actions.Add(a);
    }
    public void UnsubscribeToOnEnable(Action a)
    {
        actions.Remove(a);
    }
    public void OnEnable()
    {
        foreach(Action a in actions)
        {
            a();
        }
    }
}
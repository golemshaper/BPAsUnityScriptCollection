using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentEnableDisable : MonoBehaviour
{
    public MonoBehaviour[] components;
    public bool setComponentState = false;

    private void OnEnable()
    {
        foreach(var c in components)
        {
            c.enabled = setComponentState;
        }
    }
}

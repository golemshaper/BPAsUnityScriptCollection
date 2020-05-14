using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentEnableDisable : MonoBehaviour
{
    public MonoBehaviour[] components;
    public bool setComponentState = false;
    public bool autoDisable;
    private void OnEnable()
    {
        foreach(var c in components)
        {
            c.enabled = setComponentState;
        }
        if(autoDisable)
        {
            this.gameObject.SetActive(false);
        }
    }
}

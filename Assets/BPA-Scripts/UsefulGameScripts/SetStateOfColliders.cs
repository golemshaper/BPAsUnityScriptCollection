using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStateOfColliders : MonoBehaviour
{
    public Collider[] colliders;
    public bool setComponentState = false;
    public bool autoDisable;
    public bool triggerOnEnable = true;
    public bool triggerOnDisable = false;
    private void OnEnable()
    {
        if (!triggerOnEnable) return;
        foreach (var c in colliders)
        {
            c.enabled = setComponentState;
        }
        if (autoDisable)
        {
            this.gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if (!triggerOnDisable) return;
        foreach (var c in colliders)
        {
            c.enabled = setComponentState;
        }
    }
}

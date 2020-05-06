using BPA.Gamedata;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadCounterOnEnable : MonoBehaviour
{
    public Counter counter;
    public bool autoDisable;
    private void OnEnable()
    {
        counter.Load();
        counter.ValueChanged();
        if(autoDisable)
        {
            this.gameObject.SetActive(false);
        }
    }
}

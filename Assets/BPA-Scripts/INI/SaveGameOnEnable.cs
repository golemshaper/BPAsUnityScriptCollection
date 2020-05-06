using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameOnEnable : MonoBehaviour
{
    public bool autoDisable = true;
    private void OnEnable()
    {
        IniGameMemory.instance.SaveINIFile();
        if(autoDisable)
        {
            this.gameObject.SetActive(false);
        }
    }
}

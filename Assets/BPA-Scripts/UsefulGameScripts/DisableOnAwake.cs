using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnAwake : MonoBehaviour
{
    public bool disableRenderer=true;
    public bool disableGameObject;
    public GameObject overrideObj;
    // Start is called before the first frame update
    void Awake()
    {
        if (overrideObj == null) overrideObj = this.gameObject;
        if (disableRenderer) overrideObj.GetComponent<Renderer>().enabled = false;
        if (disableGameObject) overrideObj.SetActive(false);
    }

}

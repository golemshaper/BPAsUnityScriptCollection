using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubscribeToSignal : MonoBehaviour
{
    public SignalOnEnable signal;
    public bool unsbubscribeOnDisable = false;
    public GameObject[] enable;
    public GameObject[] disable;
    System.Action actionLambda;
    // Start is called before the first frame update
    void Start()
    {
        actionLambda = () => DoAction();
        signal.SubscribeToOnEnable(actionLambda);
    }
    void DoAction()
    {
        foreach(GameObject g in enable)
        {
            if (g != null) g.SetActive(true);
        }
        foreach (GameObject g in disable)
        {
            if (g != null) g.SetActive(false);
        }
    }
    private void OnDisable()
    {
        if(unsbubscribeOnDisable)
        {
            signal.UnsubscribeToOnEnable(actionLambda);
        }
    }
    private void OnEnable()
    {
        if(unsbubscribeOnDisable)
        {
            signal.SubscribeToOnEnable(actionLambda);
        }
    }
    private void OnDestroy()
    {
        signal.UnsubscribeToOnEnable(actionLambda);
    }
}

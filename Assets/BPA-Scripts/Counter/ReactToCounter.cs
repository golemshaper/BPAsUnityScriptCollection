using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactToCounter : MonoBehaviour
{
    public Counter watchCounter;
    public int value;
    public Counter getValueFromCounter;
    public enum CompareType {Equals,LessThen,GreaterThen};
    public CompareType compareType = CompareType.Equals;
    public GameObject[] enableIfMatch;
    public GameObject[] disableIfMatch;

    // Start is called before the first frame update
    void Start()
    {
        watchCounter.SubscribeToValueChange(()=> OnValueChanged());
    }
    void OnValueChanged()
    {
        if(getValueFromCounter!=null) value = getValueFromCounter.Value;
        switch (compareType)
        {
            case CompareType.Equals:
                if (watchCounter.Value == value) DoAction();
                break;
            case CompareType.LessThen:
                if (watchCounter.Value < value) DoAction();
                break;
            case CompareType.GreaterThen:
                if (watchCounter.Value > value) DoAction();
                break;
        }

    }
    void DoAction()
    {
        foreach(GameObject g in enableIfMatch)
        {
            if (g != null) g.SetActive(true);
        }
        foreach (GameObject g in disableIfMatch)
        {
            if (g != null) g.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        watchCounter.UnsubscribeToValueChange(()=> OnValueChanged());
    }
}

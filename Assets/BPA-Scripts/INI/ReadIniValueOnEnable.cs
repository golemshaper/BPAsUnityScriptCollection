using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadIniValueOnEnable : MonoBehaviour
{
    public string group, key;
    public int expectedValue;
    public int defaultValue=-1;
    public enum Evaluation {Equals,LessThen,GreaterThen,Not};
    public Evaluation evaluation;

    public GameObject[] enable;
    public GameObject[] disable;

    [Header("Advanced")]
    public string copyGroup;
    public string copyKey;
    public int copyDefaultValue=0;
    public bool getExpectedFromCopyGroup;
    void OnEnable()
    {
        if(getExpectedFromCopyGroup)
        {
            expectedValue = IniGameMemory.instance.GetDataValue(copyGroup, copyKey, copyDefaultValue);
        }
        int memValue = IniGameMemory.instance.GetDataValue(group, key, defaultValue);
        switch (evaluation)
        {
            case Evaluation.Equals:
                if (memValue == expectedValue) DoAction();
                break;
            case Evaluation.LessThen:
                if (memValue < expectedValue) DoAction();
                break;
            case Evaluation.GreaterThen:
                if (memValue > expectedValue) DoAction();
                break;
            case Evaluation.Not:
                if (memValue != expectedValue) DoAction();
                break;
        }
    }
    void DoAction()
    {
        foreach(GameObject g in enable)
        {
            if(g!=null)g.SetActive(true);
        }
        foreach (GameObject g in disable)
        {
            if (g != null) g.SetActive(false);
        }
    }

}
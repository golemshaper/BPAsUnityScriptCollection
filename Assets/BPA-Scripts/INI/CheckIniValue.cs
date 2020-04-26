using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIniValue : MonoBehaviour
{
    public string group,key;
    public int expectedValue;
    public enum EvalType {greaterOrEqual};
    public EvalType evalType;
    public int subtractValue=1;
    public bool doSubtract = false;

    public GameObject[] enableIfTrue;
    public GameObject[] disableIfTrue;
    public bool reverseOnStart = true;
    private void Start()
    {
        if (!reverseOnStart) return;

        ActivateGO(enableIfTrue, false);
        ActivateGO(disableIfTrue, true);
    }
    void ActivateGO(GameObject[] ary,bool state)
    {
        foreach(GameObject g in ary)
        {
            if (g != null) g.SetActive(state);
        }
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        if (IniGameMemory.instance == null) return;
        switch (evalType)
        {
            case EvalType.greaterOrEqual:
                if (IniGameMemory.instance.GetDataValue(group, key) >= expectedValue)
                {
                    ActivateGO(enableIfTrue, true);
                    ActivateGO(disableIfTrue, false);
                    if(doSubtract)
                    {
                        IniGameMemory.instance.IncrementKeyValue(group,key,-subtractValue,-1);
                    }
                }
                break;
        }
    }

}

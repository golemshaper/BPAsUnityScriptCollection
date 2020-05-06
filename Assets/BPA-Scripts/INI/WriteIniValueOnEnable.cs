using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriteIniValueOnEnable : MonoBehaviour
{
    public string group, key;
    public int value=1;
    public int defaultValue = 0;
    public enum WriteType { Set,Increment,Copy};
    public WriteType writeType;
    [Header("Copy From (COPY ONLY):")]
    public string copyGroup; 
    public string copyKey;
    public int copyDefaultValue=0;
    private void OnEnable()
    {
        IniGameMemory.instance.WriteData(group, key, value);
        switch (writeType)
        {
            case WriteType.Set:
                IniGameMemory.instance.WriteData(group, key, value);
                break;
            case WriteType.Increment:
                IniGameMemory.instance.IncrementKeyValue(group, key, value, defaultValue);
                break;
            case WriteType.Copy:
                int copyFromVal = IniGameMemory.instance.GetDataValue(copyGroup, copyKey, copyDefaultValue);
                IniGameMemory.instance.WriteData(group, key, copyFromVal);
                break;
        }

    }
}

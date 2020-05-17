using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCSVDataHolder : MonoBehaviour
{
    public static SystemCSVDataHolder instance;
    public bool makeLocal=false;
    public TextAsset csvDataFile;
    public CommaSeperatedValueParser csv;
    // Start is called before the first frame update
    void Awake()
    {
       if(!makeLocal) instance = this;
        csv.Parse(csvDataFile.ToString());
    }
    public string GetCSVData(string id)
    {
        return csv.GetCSVData(id);
    }
}
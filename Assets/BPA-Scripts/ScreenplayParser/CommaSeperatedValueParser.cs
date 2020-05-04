using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class CommaSeperatedValueParser
{
    [UnityEngine.Multiline(8)]public string SourceCSV;
    public Dictionary<string, string> csvStrings = new Dictionary<string, string>();
    public void Parse()
    {
        Parse(SourceCSV);
    }
    public void Parse(string SourceStr)
    {
        SourceCSV = SourceStr;
        string[] splitByLine = SourceCSV.Split("\n".ToCharArray());
        foreach(string s in splitByLine)
        {
            string[] subStr = s.Split(",".ToCharArray());
            if (subStr.Length < 1) continue;
            csvStrings.Add(subStr[0].Trim(), subStr[1].Trim());
        }
    }
    public string GetCSVData(string id)
    {
        return csvStrings[id.Trim()].Trim();
    }
}
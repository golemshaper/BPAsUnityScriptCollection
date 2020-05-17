using System.Collections.Generic;
public class CommaSeperatedValueParser
{
    public Dictionary<string, string> csvStrings = new Dictionary<string, string>();
    public void Parse(string SourceStr)
    {
        string[] splitByLine = SourceStr.Split("\n".ToCharArray());
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
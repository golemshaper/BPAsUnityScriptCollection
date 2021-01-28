using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FindUniqueCharactersInDocument : MonoBehaviour
{
    //TODO: Convert to text asset array!
    public TextAsset doc;
    public TextAsset[] multi_doc;
    StringBuilder sb = new StringBuilder();
    public int itemsPerLine = 60;
    Dictionary<char, int> dictionary= new Dictionary<char, int>();
    [Multiline(8)]
    public string output;
    private void OnEnable()
    {
        if(multi_doc!=null)
        {
            StringBuilder sb2 = new StringBuilder();
            foreach(TextAsset ta in multi_doc)
            {
                sb2.Append(ta.text);
            }
            output = PullUniqueCharacters(sb2.ToString());
            return;
        }
        output = PullUniqueCharacters(doc.text);
    }
    string PullUniqueCharacters(string input)
    {
        dictionary.Clear();
        input = input.Replace("\n", "");
        input = input.Replace("\t", "");
        input = input.Replace(";", "");
        input = input.Replace("/", "");
        input = input.Replace("\\", "");
        input = input.Replace("<", "");
        input = input.Replace(">", "");
       
        input = input.Replace(" ", "");
        sb.Clear();
        int i = 0;
        int y = 0;
        foreach(char c in input)
        {
            if(dictionary.ContainsKey(c)==false)
            {
                i++;
                y++;
                dictionary.Add(c,i);
                sb.Append(c);
                if(y==itemsPerLine)
                {
                    y = 0;
                    sb.Append("\n");
                }
            }
        }
        return sb.ToString();
    }
}

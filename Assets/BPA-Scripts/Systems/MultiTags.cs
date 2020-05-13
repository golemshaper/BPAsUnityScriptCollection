using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTags : MonoBehaviour
{
    public string[] Tags;
    public int[] TagsHashed;
    private void Awake()
    {
        TagsHashed=GetHashedTagsArray(Tags);
    }
    public static int[] GetHashedTagsArray(string[] inputTags)
    {
        List<int> resultList = new List<int>();
        foreach(string s in inputTags)
        {
            resultList.Add(Animator.StringToHash(s));
        }
        return resultList.ToArray();
    }
}

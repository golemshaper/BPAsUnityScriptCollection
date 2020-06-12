using Ackk.Text.Parsing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenplayVisualNovelReader : MonoBehaviour
{
    private List<SentenceGroup> sentenceGroups = new List<SentenceGroup>();

    public ScreenplayParser parserReference;
    int group=0;
    int line=0;

    const string saveGroup = "VisualNovelChapter";
    const string saveChapterKey = "BookmarkChapter";
    const string saveBookmarKey = "BookmarkLine";
    void Save()
    {
        //TODO
        IniGameMemory.instance.WriteData(saveGroup, saveChapterKey, group);
        IniGameMemory.instance.WriteData(saveGroup, saveBookmarKey, line);
        IniGameMemory.instance.SaveINIFile();

    }
    void Load()
    {
        IniGameMemory.instance.LoadINIFile();
        group = IniGameMemory.instance.GetDataValue(saveGroup,saveChapterKey,0);
        line = IniGameMemory.instance.GetDataValue(saveGroup, saveBookmarKey, 0);


    }
    void Start()
    {
        sentenceGroups = parserReference.GetScreenplay();
        Load();
        BeginReading();
    }
    void BeginReading()
    {
        enableUpdate = true;
    }
    bool next = false;
    void Next()
    {
        next = true;
    }
    bool enableUpdate;
    bool limitOnce = false;
    private void Update()
    {
        if(next)
        {
            limitOnce = false;
            next = false;
            line++;
        }
        if(limitOnce==false)
        {
            limitOnce = true;
            DisplayCurrent();
        }
    }
    void DisplayCurrent()
    {

    }
    void SentenceDataCheck(SpecialCommand spc)
    {

    }
}

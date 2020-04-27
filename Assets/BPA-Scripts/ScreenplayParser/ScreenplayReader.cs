using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ackk.Text.Parsing;
public class ScreenplayReader : MonoBehaviour 
{
	public static ScreenplayReader instance;
	//Screenplay data
    [Header("Screenplay Data:")]
	public ScreenplayParser parserReference;
 	private List<SentenceGroup> Sentences= new List<SentenceGroup>(); 
    //For saved data...
    [Header("Save Game Viusal Novel Style:")]
    public bool loadOnStart;
    public bool autoSaveData;
    public string defaultDialogueGroupIfLoadFails="MY.FIRST.SCRIPT.HELLO.WORLD";
    const string group = "Dialogue";
    const string key = "DialogueGroupKey";
    const string lineKey="CurrentLineNumber";
	//Display objects
    [Header("Display Objects:")]
	public StringWriter nameDisplay;
	public StringWriter SentenceDisplay;
    public GameObject UI;
	//Menu Prompt
    [Header("Menu Prompt:")]
	public ScreenplayMenuPromptSelector menuPrompt;
    [Header("Config:")]
    public bool autoNextAtEndIfPromptIsRequested=true;
	//Special commands............................................
	public const string _gotoCommand="goto";
	//Special commands............................................
    //------------------------------------------
    //Special Save keys:
    const string menuGroupName = "Menu";
    const string choiceKeyRootName = "Choice";
    //------------------------------------------
	// Use this for initialization
	void Awake()
	{
		if (instance == null)instance = this;
        nameDisplay.ClearText();
        SentenceDisplay.ClearText();


    }
    void Start () 
	{
		Sentences = parserReference.GetScreenplay ();
        if (loadOnStart)
        {
            StartCoroutine(SafeLoad());
        }
	}
    bool GetFastforwardButton()
    {
        //todo: replace with a button reading from playerinput instead.
        return Input.GetKey(KeyCode.BackQuote);
    }
    IEnumerator SafeLoad()
    {
        //Wait until save data is fetched.
        //TODO: Actually wait don't rely on this timing bullshit.
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        //This option should be reserved for visual novels. an RPG wouldn't need it.
        string loadDialogueGroup = IniGameMemory.instance.GetDataString(group, key, defaultDialogueGroupIfLoadFails);
        int currentLine = IniGameMemory.instance.GetDataValue(group, lineKey, 0);
        BeginReadingFromGroup(loadDialogueGroup,currentLine);
        yield break;
    }
	public void Write(string dataToWrite)
	{
		SentenceDisplay.SetText (dataToWrite,true);
	}
	bool IsPrinting()
	{
		return SentenceDisplay.IsPrinting();
	}
	public void SetNameToWrite(string name)
	{
		nameDisplay.SetText (name,false);
	}
	public void BeginReadingFromGroup(string groupToRead)
	{
        BeginReadingFromGroup(groupToRead, -1);
	}
    public void BeginReadingFromGroup(string groupToRead,int beginOnLine)
    {
        startReadingFromLine = beginOnLine;
        if (autoSaveData)
        {
            IniGameMemory.instance.WriteData(group, key, groupToRead);
            IniGameMemory.instance.WriteData(group, lineKey, beginOnLine);
        }
        StartCoroutine (DoReading (groupToRead));
    }
	void HideDialogueBox()
	{
		nameDisplay.ClearText ();
		SentenceDisplay.ClearText ();
        if (UI != null)
        {
            UI.SetActive(false);
        }
	}
    void ShowDialogueBox()
    {
        if (UI != null)
        {
            UI.SetActive(true);
        }
    }

	public bool CheckIfIsReading()
	{
		return isReading;
	}
	bool isReading=false;
	IEnumerator DoReading(string groupToRead)
	{
		if (isReading) {
			BREAK_LOOP = true;
			while(isReading)
			{
				yield return true;
			}
		}
		isReading = true;
		
		yield return true;
		yield return StartCoroutine(ReadGroup (groupToRead));
		isReading = false;
		yield break;
	}
	bool BREAK_LOOP=false;
	public bool next;
	int sentenceVal =-1;
	/// <summary>
	/// Gets the current line value.
	/// (How far along the current group you are.)
	/// </summary>
	/// <returns>The current line value.</returns>
	public int GetCurrentLineValue()
	{
		//how far along the current group you are.
		return curSentenceValue;
	}
	public SentenceUnit GetCurrentSentenceUnitData()
	{
		return curSentenceUnit;

	}
	SentenceUnit curSentenceUnit=null;
	public int curSentenceValue = -1;
    int startReadingFromLine=-1;
    //pause time is a special value that can be used to pause the text before the next line. use an embeded script to change this value.
    public float pauseTime=0;
    public void PausePrompt(float pauseForTime)
    {
        pauseTime = pauseForTime;
    }
    public SentenceGroup GetSentenceGroupData(string group)
    {
        int g = ObtainSentenceGroupValue(group);
        return Sentences[g];
    }
    public IEnumerator ReadGroup(string groupToRead)
	{
		yield return true;
		BREAK_LOOP = false;
		sentenceVal = ObtainSentenceGroupValue (groupToRead);
		if (sentenceVal == -1)
			yield break;
		//When a group must a new number is selected, and the foreach is evaluated again.
		//Bite me. This is a great use for goto ...you snob.
		READ_FROM_GROUP:
       
		curSentenceValue = -1;
        //Use this to set the starting sentences. fro save/load system if you save in the middle of dialogue.
        int intialLine = 0;
        if (startReadingFromLine > -1)
        {
            intialLine = startReadingFromLine;
            //reset the start of line reading to -1 so that goto' don't break.
            startReadingFromLine = -1;
        }
        if (autoSaveData)
        {
            IniGameMemory.instance.WriteData(group, key, groupToRead); //this gets updated during goto's 
            IniGameMemory.instance.WriteData(group, lineKey, 0); //if you ever add a way of specifying a line, then save it here.
        }
        //---------------------------------------------------------------------------------------------------
        for (int i = intialLine; i < Sentences[sentenceVal].sentences.Count; i++)
        {
            SentenceUnit s = Sentences[sentenceVal].sentences[i];
            curSentenceUnit = s;
            curSentenceValue++;
            next = false;
            if (autoSaveData)
            {
                IniGameMemory.instance.WriteData(group, lineKey, i);
            }
            while (pauseTime > 0)
            {
                HideDialogueBox();
                pauseTime -= Time.deltaTime;
                yield return null;
            }
            ShowDialogueBox();

            Write(s.dialogue);
            SetNameToWrite(s.speakerName);
            yield return true;
            while (IsPrinting())
            {
                if (BREAK_LOOP)
                {
                    BREAK_LOOP = false;
                    isReading = false;
                    yield break;
                }
                if (PlayerInput.instance != null)
                {
                    next = PlayerInput.instance.GetFire1_B();
                }
                if (next)
                {
                    //Make text typeout all at once.
                    SentenceDisplay.EndTypewriterEffect();
                    next = false;
                }
                if (GetFastforwardButton())
                {
                  //  Time.timeScale = 50f;
                    SentenceDisplay.EndTypewriterEffect();
                }
              
                yield return true;
            }
            //...now look for skip button input.
            yield return true;
          
            while (!next  && GetFastforwardButton()==false)
            {
                if (BREAK_LOOP)
                {
                    BREAK_LOOP = false;
                    isReading = false;
                    yield break;
                }
                if (PlayerInput.instance != null)
                {
                    next = PlayerInput.instance.GetFire1_B();
                }
                if (autoNextAtEndIfPromptIsRequested)
                {
                    if (SentenceDisplay.IsPrinting() == false)
                    {
                        if (s.dealWithSpecialCommands == SentenceUnit.DealWithSpecailCommands.UseInPromptMenu)
                        {
                            yield return new WaitForSeconds(0.5f);
                            next = true;
                        }
                    }
                }
                yield return true;
            }
            next = false;
            //The multichoice prompt is evaluated before each specail command is requested.
            //all special commands become a choice in the prompt
            if (s.dealWithSpecialCommands == SentenceUnit.DealWithSpecailCommands.UseInPromptMenu)
            {
                menuPrompt.ClearChoicesList();
                List<SpecialCommand> modifiedChoiceList = new List<SpecialCommand>();
                int checkCommandNumber = 0;
                foreach (SpecialCommand command in s.specialCommands)
                {
                    //Remove choice if memory says to remove it.
                    if(IniGameMemory.instance.GetDataValue(menuGroupName, choiceKeyRootName + checkCommandNumber, -1)==1)
                    {
                        //skip this command choice. this works, but I need to find a way to have menu choice  link to new index number...
                        checkCommandNumber++;
                        continue;
                    }
                    menuPrompt.AddChoice(command.requestName);
                    //The choice list can be modfied, so make a copy of it and build it up from valid options.
                    modifiedChoiceList.Add(command);
                    checkCommandNumber++;

                }
                yield return true;
                menuPrompt.OpenPrompt();
                while (menuPrompt.isPromptOpen())
                {
                    if (BREAK_LOOP)
                    {
                        BREAK_LOOP = false;
                        isReading = false;
                        menuPrompt.ClearChoicesList();
                        menuPrompt.HidePromptDisplay();
                        yield break;
                    }
                    yield return true;
                }
                sentenceVal = ObtainSentenceGroupValue(modifiedChoiceList[menuPrompt.GetChoice()].commandLine);
                if (autoSaveData)
                {
                    groupToRead = s.specialCommands[menuPrompt.GetChoice()].commandLine;
                }
                if (sentenceVal == -1)
                {
                    Debug.LogError("ScreenplayReader Error 01: Group '" + s.specialCommands[menuPrompt.GetChoice()].commandLine + "' does not exist!");
                }
                goto READ_FROM_GROUP;
                //Bite me. This is a great use for goto ...you snob.
            }
            //Parse special commands.....................
            foreach (SpecialCommand command in s.specialCommands)
            {
                switch (s.dealWithSpecialCommands)
                {
                    case SentenceUnit.DealWithSpecailCommands.Ignore:
                        break;
                    case SentenceUnit.DealWithSpecailCommands.NotifySystem:
                        break;
                    case SentenceUnit.DealWithSpecailCommands.UseInPromptMenu:
                        //... not evaluated here... was delt with by the if statement above. on line starting @ if(s.dealWithSpecialCommands==SentenceUnit.DealWithSpecailCommands.UseInPromptMenu)
                        break;
                    case SentenceUnit.DealWithSpecailCommands.AutoGoto:
                        print("GOTO:" + command.commandLine);
                        //pick a new sentence group and goto that group...
                        sentenceVal = ObtainSentenceGroupValue(command.commandLine);
                        if (autoSaveData)
                        {
                            groupToRead = command.commandLine;
                        }
                      
                        if (sentenceVal == -1)
                        {
                            Debug.LogError("ScreenplayReader Error 01: Group '" + command.commandLine + "' does not exist!");
                        }
                        goto READ_FROM_GROUP;
                        //Bite me. This is a great use for goto ...you snob.
#pragma warning disable CS0162 // Unreachable code detected
                        break;
#pragma warning restore CS0162 // Unreachable code detected

                }
            }
            //.........................................................................................
        }
		sentenceVal =-1;
		print ("End of conversation");
		HideDialogueBox ();
		yield break;
	}
	//Get name of the current group
	public string GetCurrentGroupName()
	{
		if (isReading) {
			return Sentences[sentenceVal].groupName;
		}
		//not reading, so no group is found...
		return "-1";
	}
	//Check if a name matches the current group
	public bool CheckIfGroupNameIs(string nameCheck)
	{
		if (nameCheck == GetCurrentGroupName ()) {
			return true;
		} else {
			return false;
		}
	}
	// Get group index by name
	public int ObtainSentenceGroupValue(string group)
	{
		for(int i=0; i< Sentences.Count; i++)
		{
			if(Sentences[i].groupName.Trim()==group)
			{
				return i;
			}
		}
		return -1;
	}
}
using UnityEngine;
using System.Collections;
using Ackk.Text.Parsing;
using System.Collections.Generic;
using System.Text;


public class ScreenplayParser : MonoBehaviour 
{
    /*EXAMPLE DATA:
     * 
     * 
     * 
HELLO.WORLD
				@EnPeeSea
	Hi, I'm EnPeeSee. I can say things!;
	And I can do it with multiple lines!!;
	...;
	.......;
	Bye!;
	Actually wait, do you have the time?;
					PROMPT
	Yes, I do. =HELLO.WORLD.YES
	No I don't =HELLO.WORLD.NO
HELLO.WORLD.YES
                 @SYSTEM
    Make sure you didn't end the prompt choice with a ;

     */
    public TextAsset Screenplay;
    public TextAsset TSVTextData;
	public List<SentenceGroup> Sentences= new List<SentenceGroup>();
	public bool ConsolidateMultilinesToSingleLine=true;
	// Use this for initialization
	void Awake () 
	{
	
		if(Screenplay!=null) Sentences=ParseScreenplay (Screenplay.text);
        if(TSVTextData!=null) Sentences.AddRange (ParseFromSpreadsheet (TSVTextData.text));
	}
	public List<SentenceGroup> GetScreenplay()
	{
		if (Sentences == null) {
			Sentences=ParseScreenplay (Screenplay.text);
		}
		return Sentences;
	}
	//Parser variables...
	enum ProcessType
	{
		Unknown,BuildGroup,BuildName,BuildSentence,AddScriptingToLine,BuildPrescriptActions
	};
	/*
	 * When you write the reader class:
	 * Have it read the dialogue,but when you press the Next button, have it eval the commands.
	 * if the commands are a goto, then auto goto the group requested.
	 * If they are a menu, then have it pop the menu up when the usere hits the next button, but do not erease the text until after the prompt has been resolved.
	 * otherwise, throw the commands onto the command list, and allow the user to deal with them.
	 * 
	 * Follow this advice and the prompt will be smooth and trouble free.
	 * (considder having an EVAL_IMMEDIATLY option, but I would rather design around it)
	 * 
	 * 
	 * Note you need it to count the next line as part of the first prev line unless it has a ; at the end...
	 */
	ProcessType processType;
	const string specialCharCountAsUpper = ("-.!_()\n`'|@©");

	//Special reserved names...
	public const string SystemEvalString="EVAL";
	public const string SystemGotoString="GOTO";
	public const string SystemPromptString="PROMPT";
	public const string SystemHiddenName="SYSTEM";
	
	public SentenceUnit.DealWithSpecailCommands specialCommandAction;
	//Parse screenplay from text
	bool addToPreviousSentenceSentence;

    public string ConsolidateLines(string data)
	{
		/*
		 * EXAMPLE.SCREENPLAY
		 * 					DOC
				No... just the new package, I 
				guess...
		 * 
		 * Becomes:
		 * EXAMPLE.SCREENPLAY
		 * 					DOC
				No... just the new package, I guess...
		 * 
		 * */
        StringBuilder reformatBeforeRemoveWordWrapBuilder= new StringBuilder();
       

		//string reformatTextRemovingWordWrap = string.Empty;
		string[] RawData=data.Split ("\n".ToCharArray (),System.StringSplitOptions.RemoveEmptyEntries);
	
		string addALine = string.Empty;
		foreach (string s in RawData) 
		{

			//if(string.IsNullOrEmpty(trimLine))continue;
			if(CheckIfStringIsUppercase(s) || CheckIfLineIsAName(s) || LookForSpecialScript(s) || IsLineAPrescriptAction(s))
			{
				if(addALine!=string.Empty)
				{
                    //old
				//	reformatTextRemovingWordWrap+="\n\t"+addALine+"\n";
                    //new
                    reformatBeforeRemoveWordWrapBuilder.Append("\n\t");
                    reformatBeforeRemoveWordWrapBuilder.Append(addALine);
                    reformatBeforeRemoveWordWrapBuilder.Append("\n");

					addALine=string.Empty;
				}
                //old
			//	reformatTextRemovingWordWrap+=s+"\n";
                //new
                reformatBeforeRemoveWordWrapBuilder.Append(s);
                reformatBeforeRemoveWordWrapBuilder.Append("\n");

			}
			else if(char.IsWhiteSpace(s[0]))
			{
				string trimLine=s.Trim();
				//print("trimLineLen:="+trimLine.Length+":"+trimLine);
				if(trimLine.Length<=0)continue;
				if(trimLine[trimLine.Length-1]==';')
				{
					addALine+=" "+trimLine+"\n";
					
				}
				else
				{
					addALine+=" "+trimLine;
				}
				
				continue;
			}


		}
		if(addALine!=string.Empty)
		{
            //old
			//reformatTextRemovingWordWrap+="\n\t"+addALine+"\n";
            //new
            reformatBeforeRemoveWordWrapBuilder.Append("\n\t");
            reformatBeforeRemoveWordWrapBuilder.Append(addALine);
            reformatBeforeRemoveWordWrapBuilder.Append("\n");
			addALine=string.Empty;
		}
       
        return reformatBeforeRemoveWordWrapBuilder.ToString();
	}
	public List<SentenceGroup> ParseScreenplay(string screenplayText)
	{
        Debug.Log(screenplayText);
        //not working as a soultion, but the ’ makes it unable to parse... screenplayText = screenplayText.Replace("’", "'");

        List<SentenceGroup> createScreenplayData = new List<SentenceGroup> ();
		//merge lines next line if the current line has no semicolon;.
		if(ConsolidateMultilinesToSingleLine)screenplayText = ConsolidateLines (screenplayText);

		string[] dataByLine = screenplayText.Split ("\n".ToCharArray (),System.StringSplitOptions.RemoveEmptyEntries);
	
		string lastNameParsed = "";
	
		foreach (string line in dataByLine) 
		{
		
			processType=ProcessType.Unknown;
			if(CheckIfLineIsAGroup(line))
			{
				processType=ProcessType.BuildGroup;
			}
			//Speaking Character Name
			else if(CheckIfLineIsAName(line))
			{
				processType=ProcessType.BuildName;

			}
			else if (IsLineAPrescriptAction(line))
			{
				processType=ProcessType.BuildPrescriptActions;
			}
			//Dialoge Sentence Data
			else
			{
				if(char.IsWhiteSpace(line[0]))
				{
					processType=ProcessType.BuildSentence;
				}
				else
				{
					//the line is a comment because it is not allcaps, and it has no leading spaces...
					processType=ProcessType.Unknown;
				}
			}
			if(LookForSpecialScript(line))
			{
				processType=ProcessType.AddScriptingToLine;
			}
			switch (processType) 
			{
			case ProcessType.BuildGroup:

				createScreenplayData.Add(new SentenceGroup(line));
				break;
			case ProcessType.BuildName:
				if(line.Trim()==SystemEvalString)
				{
					//Eval redirect...
					specialCommandAction=SentenceUnit.DealWithSpecailCommands.NotifySystem;
				}
				else if(line.Trim()==SystemGotoString)
				{
					specialCommandAction=SentenceUnit.DealWithSpecailCommands.AutoGoto;
					//Goto redirect...
				}
				else if(line.Trim()==SystemPromptString)
				{
					specialCommandAction=SentenceUnit.DealWithSpecailCommands.UseInPromptMenu;
					//Prompt redirect...
				}
				else if(line.Trim()==SystemHiddenName)
				{
					lastNameParsed=string.Empty;
				}
				else
				{
					specialCommandAction=SentenceUnit.DealWithSpecailCommands.AutoGoto;
					//Build a name...
					lastNameParsed=line.Trim();
				}
				break;
			case ProcessType.BuildSentence:
				//createScreenplayData[createScreenplayData.Count-1].sentences.Add(new SentenceUnit(lastNameParsed,line));
			
				string[] splitLineBySemiColon=line.Trim().Split(";".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);
				foreach(string sentence in splitLineBySemiColon)
				{
					createScreenplayData[createScreenplayData.Count-1].sentences.Add(new SentenceUnit(lastNameParsed,sentence));
				}

			
				break;
			case ProcessType.AddScriptingToLine:
			
				int senCount=createScreenplayData[createScreenplayData.Count-1].sentences.Count;
				if(senCount>=1)
					createScreenplayData[createScreenplayData.Count-1].sentences[senCount-1].LoadSpecialCommands(line,specialCommandAction);
				
				break;
			case ProcessType.BuildPrescriptActions:
				/*Example of a pre-dialogueAction:
				 *<Actor/0/Brian>
				 *which will be parsed for finding:
				 * Actor(the action to take)
				 * 0 the image the actor should use.
				 * Brian the name of the actor to select the image from.
				 * 
				 */
				int senCount2=createScreenplayData[createScreenplayData.Count-1].sentences.Count;
				if(senCount2>=1)
				{
					PrescriptAction action= new PrescriptAction(line);
					createScreenplayData[createScreenplayData.Count-1].sentences[senCount2-1].prescriptActions.Add(action);
				}
				break;
			}
		}
		return createScreenplayData;
	}

    public List<SentenceGroup> ParseFromSpreadsheet(string tsv)
    {
        //String	Tag	Where	What	English	JP	

        string[] columns = tsv.Split("\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
        string lastTagName = "";

        //THE DATA TAGS. For localization, make these not constant, and grab values from that languages constants (which don't exist yet.)
        //numbers are the row to grab data from!
        const int TagID = 0;
        const int englishName = 1;
        const int englishDialgoue = 2;
        //const int ignore_comments = 3;


        int languageData = englishDialgoue;
        List<SentenceGroup> dataCreated = new List<SentenceGroup>();

        dataCreated.Add(new SentenceGroup());
        dataCreated[dataCreated.Count - 1].groupName = "BBB";

        string nameOfSpeaker = "";
        foreach (string row in columns)
        {
            //Data layout: TAG(Group)	English Name	English String	Comments	Special Code

            string[] rowData = row.Split("\t".ToCharArray());

            if (rowData.Length == -1) continue;
            if (rowData.Length < TagID) continue;

  
            //empty is part of the last dialogue...
            if (rowData[TagID] != lastTagName && rowData[TagID] != string.Empty)
            {
                //tag name
                lastTagName = rowData[TagID];
                nameOfSpeaker = ""; //assume no name by default
                dataCreated.Add(new SentenceGroup(lastTagName));
            }
            if (rowData[englishName] != string.Empty) nameOfSpeaker = rowData[englishName]; //USE LAST NAME FOUND
            if (rowData[englishName] == "SYSTEM") nameOfSpeaker = ""; //set to system to clear name
            if (rowData[englishName] == "CLEAR") nameOfSpeaker = ""; //set to CLEAR to clear name


            if (rowData.Length < languageData) continue;

            //no names in the system for now...
            if (dataCreated.Count <= 0) continue;
            dataCreated[dataCreated.Count - 1].sentences.Add(new SentenceUnit(nameOfSpeaker, rowData[languageData]));

        }
        return dataCreated;
    }


    bool CheckIfLineIsAGroup(string line)
	{
		if (line == string.Empty) {
			return false;
		}

		if (char.IsWhiteSpace(line [0])) {
			return false;
		}

		bool isGroup = CheckIfStringIsUppercase (line);
	
		return isGroup;
	}
	bool LookForSpecialScript(string s)
	{
		foreach (char c in s) {
            //ignore if enclosed in <>
            if(c=='{')return false;
            if(c=='}')return false;
			if(c=='=')return true;
		}
		return false;
	}
	bool LookForScriptBegin(string s)
	{
		foreach (char c in s) {
			if(c=='{')return true;
		}
		return false;
	}
	bool LookForScriptEnd(string s)
	{
		foreach (char c in s) {
			if(c=='}')return true;
		}
		return false;
	}
	bool CheckIfLineIsAName(string line)
	{
	
		if (line == string.Empty) {
			return false;
		}
		if (line.Trim () != string.Empty) {
			if (line.Trim () [0] == '@') {
				//the @ symbol can be used to define a name of any format.
				return true;
			}
		}
		if (!char.IsWhiteSpace(line [0])) {
			return false;
		}
		bool isName = CheckIfStringIsUppercase (line);
		if (line [line.Length - 1] == ';') {
			isName = false;
		}
		return isName;
	}
	public bool IsLineAPrescriptAction(string str)
	{
        //ExampleData: {Actor/0/Alex}
		str = str.Trim ();
		if (str == string.Empty)
			return false;
        if (str [0] == '{') {
			return true;
		}
		return false;
	}
	public bool CheckIfStringIsUppercase(string str)
	{
		bool isUppercase = true;
		str = str.Trim ();
		bool anotherCharacterWasNotANumber = false;
		foreach (char c in str) 
		{
			if(char.IsNumber(c)==false)
		    {
				anotherCharacterWasNotANumber=true;
			}
			if(char.IsUpper(c))
			{
				//isUppercase = true;
				continue;
			}
			else if(char.IsNumber(c))
			{
			//	isUppercase = true;
				continue;
			}
			else if(IsSpecialChar(c))
			{
			//	isUppercase=true;
				continue;
			}
			else
			{
				//print ("NotUpper:"+c);
				isUppercase = false;
				return isUppercase;
			}
		
		}
		if (isUppercase) 
		{
			if(anotherCharacterWasNotANumber==false)
			{
				isUppercase=false;
			}
		}
		return isUppercase;
	}
	public bool IsSpecialChar(char c)
	{
		bool isSpecialCharacter=false;
		foreach(char s in specialCharCountAsUpper)
		{
			if(c==s)
			{
				isSpecialCharacter=true;
				return isSpecialCharacter;

			}
		}
		return isSpecialCharacter;
	}

}
namespace Ackk.Text.Parsing
{
	[System.Serializable]
	public class SentenceGroup
	{
		public SentenceGroup()
		{
			//empty constructor
		}
		public SentenceGroup(string nGroupName)
		{
			groupName = nGroupName;
		}
		public string groupName;
		public List<SentenceUnit> sentences = new List<SentenceUnit>();
	}
	[System.Serializable]
	public class SentenceUnit
	{
		public SentenceUnit()
		{
			//empty constructor
		}
		public SentenceUnit(string actorName,string sentence)
		{
			speakerName = FormatSpeakerName(actorName);
			dialogue = sentence.Trim();
		}
		string FormatSpeakerName(string n)
		{
			if (n == string.Empty) {
				return n;
			}
			if (n [0] == '@') {
				string[] splitName=n.Split("@".ToCharArray());
				return splitName[1];
			}
			//Fix capitalization.
			string buildNewName = string.Empty;
			n = n.ToLower ();
			for (int i=0; i<n.Length; i++) 
			{
				if(i==0)
					buildNewName+=n[i].ToString().ToUpper();
				else
					buildNewName+=n[i];
			}
			return buildNewName;
		}
		public void LoadSpecialCommands(string commandString,DealWithSpecailCommands actionToTake)
		{
			string[] parseCommands = commandString.Split ("\n".ToCharArray (),System.StringSplitOptions.RemoveEmptyEntries);
			foreach(string cmnd in parseCommands)
			{
				specialCommands.Add(new SpecialCommand(cmnd.Trim()));
			}
			dealWithSpecialCommands = actionToTake;
		}
		public enum DealWithSpecailCommands
		{
			Ignore,NotifySystem,UseInPromptMenu,AutoGoto
		};
		[Multiline(3)]
		public string dialogue;
		public string speakerName;
		public DealWithSpecailCommands dealWithSpecialCommands;
		public List<PrescriptAction> prescriptActions= new List<PrescriptAction>();
		public List<SpecialCommand> specialCommands= new List<SpecialCommand>();

	}
    [System.Serializable]
	public class SpecialCommand
	{
		public SpecialCommand()
		{	
			//empty constructor
		}
		public SpecialCommand(string s)
		{
			string[] dataSplit = s.Split ("=".ToCharArray ());
			requestName = dataSplit [0];
			commandLine = dataSplit [1];
		}
		public string requestName;
		public string commandLine;
		
	}
	[System.Serializable]
	/// <summary>
	/// Prescript action.
	/// Actions called like <ActorName/Action/Data>
	/// </summary>
	public class PrescriptAction
	{
		public PrescriptAction()
		{	
			//empty constructor
		}
		public PrescriptAction(string s)
		{
			s = s.Trim ();
            dataPack= s.Split ("{/}".ToCharArray (),System.StringSplitOptions.RemoveEmptyEntries);
		}
		[Multiline(3)]
		public string[] dataPack;
		
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ackk.Text.Parsing;

public class CommonSystemScriptReactions : MonoBehaviour,IUpdateSlave {
    public PrescriptAction[] curPrescriptActions;

    public PrescriptAction[] lastAction;



    // Use this for initialization
    void OnEnable () 
    {
        UpdateMaster.Register(this as IUpdateSlave);
	}
    void OnDisable () 
    {
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
    void OnDestroy()
    {
        UpdateMaster.Deregister(this as IUpdateSlave);
    }

    #region IUpdateSlave implementation
    public int lastValue=-2;
    public void DoUpdate()
    {
        if (ScreenplayReader.instance == null)
            return;

        if (ScreenplayReader.instance.GetCurrentLineValue () != lastValue) 
        {
            //Check to see if it has evaluated this line yet. If it hasn't it will read it this frame, but ignore it next frame.
            //It does this so it doesn't have to do extra work evaluating the commands.
            lastValue = ScreenplayReader.instance.GetCurrentLineValue ();
        } 
        else 
        {
            //already evaluated this line, so don't do it again.
            return;
        }


        SentenceUnit sentenceUnit=  ScreenplayReader.instance.GetCurrentSentenceUnitData ();
        if (sentenceUnit == null)
            return;
        curPrescriptActions=sentenceUnit.prescriptActions.ToArray();
        if (curPrescriptActions != null) {
            foreach(PrescriptAction s in curPrescriptActions)
            {
                //Pause.......................................................................
                if(s.dataPack[0].ToLower()=="system")
                {
                    if(s.dataPack[1].ToLower()=="pause")
                    {
                        ScreenplayReader.instance.PausePrompt(float.Parse(s.dataPack[2]));
                    }
                }
                //Pause.......................................................................
                //Memory......................................................................
                if(s.dataPack[0].ToLower()=="memory")
                {
                    EventManagerBPA.instance.EvalString(s.dataPack[1]);
                }
                //Memory......................................................................
                //IF statements...............................................................
                if (s.dataPack[0].ToLower() == "if")
                {
                    //for now, if can only link to a goto. it cannot write data.
                    //DoIfEvaluation(s.dataPack[1]);
                    StartCoroutine(WaitForDialogueToEndAndThenEvalIfStatement(s.dataPack[1]));
                }
                //IF statements...............................................................

                //TODO: A goto with a goto line option.
            }
        }
        //don't repeat these actions next frame.
        if(curPrescriptActions!=null) lastAction=curPrescriptActions;
        curPrescriptActions = null;
    }

    #endregion
    IEnumerator WaitForDialogueToEndAndThenEvalIfStatement(string checkIfStatement)
    {
        while (ScreenplayReader.instance.CheckIfIsReading())
        {
            yield return null;
        }
        DoIfStatementEvaluation(checkIfStatement);
    }
    void DoIfStatementEvaluation(string mainExpression)
    {
       /*
        PROBLEM! You cannot use a < or > sign because that is what the script tags currently use!!
        * */
       // mainExpression= mainExpression.Replace(';', ' ');
        //Eval an if statement:
        const char unknown='^';
        const char equalsSign = '=';
        const char LessThen = '<';
        const char GreaterThen = '>';
        const char LessThenOrEquals = '&';
        const char GreaterThenOrEquals = '%';
        char[] conditionArray = {equalsSign,LessThen,GreaterThen,LessThenOrEquals,GreaterThenOrEquals};
        //double equal is for habit, but it's not needed.
        mainExpression=  mainExpression.Replace("==", equalsSign.ToString());
        //replace all compound checks with a single character for easier parsing.
        mainExpression= mainExpression.Replace("<=", LessThenOrEquals.ToString());
        mainExpression=mainExpression.Replace(">=", GreaterThenOrEquals.ToString());
        print("Expression to check:"+mainExpression);
        /*
         * input data looks like this:
         * 
         * {IF/BriansGroup:Item==1 ? MARIA.EVENT : MY.FIRST.SCRIPT.HELLO.WORLD}
         * 
         * but buy the time this function gets it...
         * it should be this:
         * 
         * BriansGroup:Item==1 ? MARIA.EVENT : MY.FIRST.SCRIPT.HELLO.WORLD
         * 
         * 
         * 
        */
        //subEvets result= [0]BriansGroup:Item==1 
        //subEvets result= [1]MARIA.EVENT : MY.FIRST.SCRIPT.HELLO.WORLD
        string[] subEvents = mainExpression.Split("?".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);
        string condition = subEvents[0];
       
        //Condition parts result:
        //[0]BriansGroup
        //[1]Item==1 
        string[] conditionParts = condition.Split(":".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);
        string group = conditionParts[0].Trim();
        //find the condition type:
        char typeOfCondition = unknown;
        foreach (char s in conditionParts[1])
        {
            if (s == equalsSign)
                typeOfCondition = s;
            if (s == LessThen)
                typeOfCondition = s;
            if (s == GreaterThen)
                typeOfCondition = s;
            if (s == LessThenOrEquals)
                typeOfCondition = s;
            if (s == GreaterThenOrEquals)
                typeOfCondition = s;
        }
        //keyAndRequestedValue results:
        //[0]Item
        //[1]1
     
        string[] keyAndRequestedValue=conditionParts[1].Split(conditionArray,System.StringSplitOptions.RemoveEmptyEntries);
        string key = keyAndRequestedValue[0].Trim();
        string compareValue = keyAndRequestedValue[1];
        int compareValueInt=-1;
        bool isAnInt = int.TryParse(compareValue, out compareValueInt);
        //This will do expression [0] if true, and expression [1] if false.
        bool Results = false;
        if (isAnInt)
        {
         
            //If the data is an int:
            int dataValueFromMemory = IniGameMemory.instance.GetDataValue(group, key, -1);
            print("Is an int: "+dataValueFromMemory+" and "+compareValueInt );
            if (typeOfCondition == equalsSign)
            {
                if (dataValueFromMemory == compareValueInt)
                {
                    Results = true;
                }
            }
            if (typeOfCondition == LessThen)
            {
                if (dataValueFromMemory < compareValueInt)
                {
                    Results = true;
                }
            }
            if (typeOfCondition == LessThenOrEquals)
            {
                if (dataValueFromMemory <= compareValueInt)
                {
                    Results = true;
                }
            }
            if (typeOfCondition == GreaterThen)
            {
                if (dataValueFromMemory > compareValueInt)
                {
                    Results = true;
                }
            }
            if (typeOfCondition == GreaterThenOrEquals)
            {
                if (dataValueFromMemory >= compareValueInt)
                {
                    Results = true;
                }
            }
        }
        else if (typeOfCondition == equalsSign)
        {
            //if the data is a string:
            string dataValueFromMemory = IniGameMemory.instance.GetDataString(group, key, "???");
            print("Is NOT an int: "+dataValueFromMemory+" and "+compareValue );
            if (typeOfCondition == equalsSign)
            {
                if (dataValueFromMemory == compareValue)
                {
                    if (compareValue.Trim() == dataValueFromMemory.Trim())
                    {
                        Results = true;
                    }
                }
            }
        }

        //expressions result= [0]MARIA.EVENT 
        //expressions result= [1]MY.FIRST.SCRIPT.HELLO.WORLD
        string[] expressions = subEvents[1].Split(":".ToCharArray());
        //For now, just do a goto:
        if (Results)
        {
            ScreenplayReader.instance.BeginReadingFromGroup(expressions[0].Trim(),-1);
        }
        else
        {
            if (expressions[1].ToLower() == "null")
            {
                //you can also use a null if you want to eval more then one expression for true only.
                return;
            }
            ScreenplayReader.instance.BeginReadingFromGroup(expressions[1].Trim(),-1);
        }
    }
	
}

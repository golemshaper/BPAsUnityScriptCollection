using UnityEngine;
using System.Collections;
using Ackk.EventManagerSpace;
using System.Text;

public class IniMathFunctionEventsReciever : MonoBehaviour,IEventReceiver {
    void Start () 
    {
        //Must register here:
        EventManagerBPA.instance.RegisterEventListener(this as IEventReceiver, "ADD");
        EventManagerBPA.instance.RegisterEventListener(this as IEventReceiver, "SUB");
        EventManagerBPA.instance.RegisterEventListener(this as IEventReceiver, "SET");
        EventManagerBPA.instance.RegisterEventListener(this as IEventReceiver, "MULT");
        EventManagerBPA.instance.RegisterEventListener(this as IEventReceiver, "DIV");
        EventManagerBPA.instance.RegisterEventListener(this as IEventReceiver, "CLEAR");

    }
    #region IEventReceiver implementation
    public void TriggerEvent(EventBPA eventRecieved)
    {
        string objectDataString = (string)eventRecieved.eventData as string;
        string[] dataInArray = objectDataString.Split(":,".ToCharArray());
        int addValue = 0;
       
        int defaultValue = 0;

        if (dataInArray.Length < 1)
            return;
        if (dataInArray.Length > 2)
        {
            TryParseOrGetFromKey(dataInArray[2], out addValue, -1);
        }
        if (dataInArray.Length > 3)
        {
            TryParseOrGetFromKey(dataInArray[3], out defaultValue, -1);
        }
        //CLEAR GROUP FUNCTION (NOT MATH)
        if (eventRecieved.eventName.ToUpper() == "CLEAR")
        {
            //add 1 value to a key, and set the default value.
            IniGameMemory.instance.ClearGroup(dataInArray[0]);
            Debug.Log("Attempt to clear:" + dataInArray[0]);
        }
        //ADD FUNCTION
        if (eventRecieved.eventName.ToUpper() == "ADD")
        {
            //add 1 value to a key, and set the default value.
            IniGameMemory.instance.IncrementKeyValue(dataInArray[0], dataInArray[1], addValue, defaultValue);
        }
      
        //SUB FUNCTION
        if (eventRecieved.eventName.ToUpper() == "SUB")
        {
            IniGameMemory.instance.IncrementKeyValue(dataInArray[0], dataInArray[1], addValue*-1, defaultValue);
        }
        //SET FUNCTION
        if (eventRecieved.eventName.ToUpper() == "SET")
        {
          //  IniGameMemory.instance.WriteData(dataInArray[0], dataInArray[1], addValue);
            //Use a string instead of an int, so that this can also be used for non numaric data setting.
            IniGameMemory.instance.WriteData(dataInArray[0], dataInArray[1], dataInArray[2]);

        }
        //MULT
        if (eventRecieved.eventName.ToUpper() == "MULT")
        {
            IniGameMemory.instance.MultiplyKeyValue(dataInArray[0], dataInArray[1], addValue, defaultValue);
        }
        //DIV
        if (eventRecieved.eventName.ToUpper() == "DIV")
        {
            /*
             * Division example:
                SET(v,a,               32)
                SET(v,b,                2)
                DIV(output,a,     v/a,v/b)
                --------------------------
                Can also be written as:
                    SET(v,a:32)
                    SET(v,b:2)
                    DIV(output,a:v/a,v/b)
               Or like this:
                    SET(Var,a: 32)
                    SET(Var,b:  2)
                    DIV(Output,c: Var\a,Var\b)
             * */
            if (defaultValue != 0)
            {
                IniGameMemory.instance.WriteData(dataInArray[0], dataInArray[1], addValue / defaultValue);
            }
        }

    }
  
    public void TryParseOrGetFromKey(string input, out int output,int defaultValue)
    {
        int firstVal = -1;
        if (int.TryParse(input, out firstVal))
        {
            output = firstVal;

            return;
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            string group="g";
            string key="k";
            foreach (char c in input)
            {
                switch (c)
                {
                    case '/':
                        group = sb.ToString().Trim();
                        sb.Length = 0;
                        break;
                    case '\\':
                        group = sb.ToString().Trim();
                        sb.Length = 0;
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            key = sb.ToString().Trim();
            firstVal = IniGameMemory.instance.GetDataValue(group, key, defaultValue);
            output = firstVal;
        }
       
    }
    public bool GetRemoveIfEventTriggered()
    {
        return false;
    }
    #endregion
 

}

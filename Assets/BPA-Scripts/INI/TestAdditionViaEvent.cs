using UnityEngine;
using System.Collections;
using Ackk.EventManagerSpace;
using System.Text;

public class TestAdditionViaEvent : MonoBehaviour 
{
 
    public string groupName="Group";
    public string KeyName="Key";

    public bool add;
    public bool sub;
    public int addValue=1;
    public int defaultValue=0;
    [Multiline(5)]
    public string evalStrings;
    public bool doEvalString;
    public string CombineData(string a, string b,int c,int d)
    {
        StringBuilder sw =new StringBuilder();
        //format will be "Group,Key,1,defaultValue"
        sw.Append(a);
        sw.Append(",");
        sw.Append(b);
        sw.Append(",");
        sw.Append(c.ToString());
        sw.Append(",");
        sw.Append(d.ToString());
        return sw.ToString();
       
    }
    public string CombineData(string a, string b,int c)
    {
        StringBuilder sw =new StringBuilder();
        //format will be "Group,Key,1"
        sw.Append(a);
        sw.Append(",");
        sw.Append(b);
        sw.Append(",");
        sw.Append(c.ToString());
        return sw.ToString();

    }
	// Use this for initialization
    public void Update()
    {



        /*
         * 
         * IDEA:
         * if statements could be a function with the value to test pased in followed by the function to call if true. and an else funtion.
         * 
         * 
         * */
        if (EventManagerBPA.instance == null)
            return;
        if (add)
        {
            add = false;
            EventManagerBPA.instance.CallEvent("ADD",CombineData(groupName,KeyName,addValue,defaultValue));
        //    EventManagerBPA.instance.CallEvent("ADD",CombineData(groupName,"defaultOmitted",addValue));
        //    EventManagerBPA.instance.CallEvent("ADD",CombineData(groupName,KeyName+"2",addValue*2,defaultValue));
        }
        if (sub)
        {
            sub = false;
            EventManagerBPA.instance.CallEvent("ADD",CombineData(groupName,KeyName,addValue*-1,defaultValue));
            //    EventManagerBPA.instance.CallEvent("ADD",CombineData(groupName,KeyName+"2",addValue*2,defaultValue));
        }
        if (doEvalString)
        {
            doEvalString = false;
            EventManagerBPA.instance.EvalString(evalStrings);
            /*
             * EXAMPLE CODE:
                    ADD(Input,A,8)
                    ADD(Input,B,3)
                    ADD(Output,C,Input/A,Input/B)
             *
                    -Make a group called "Input" and a key called "A", and assign 8.
                   -Create a new key in group "Input" called "B", and assign 3.
                    Add Input/A   and Input/B  together and add the result to key "C" under group "Output"

             *  EXAMPLE2:
                    ADD(Input,A,8)
                    ADD(Input,B,3)
                    ADD(Output,C,Input/A,Input/B)
                    SET(Output,D,Output/C)
                    SUB(Output,E,2,Output/D,0)
                        -you can refer to variables by typing the group name followed by a '/' an then the keyname for that variable.
             *   SIMPLE DIVISION:
                    DIV(Group,Key:Group\Key,2)


             *   DIVISION:
                   SET(v,a,               32)
                   SET(v,b,                2)
                   DIV(output,a,     v/a,v/b)
                    --------------------------
                    Can also be written as:
                        SET(v,a:32)
                        SET(v,b:2)
                        DIV(output,a:v/a,v/b)

                    -create two variables nambe 'a', and 'b' and store them in 'v'
                    -assign 32 to a and 2 to b
                    -divide a and b and store the result in group 'output' under key 'a'.
                    (*Note that I made, and : interchangable so that a seperation could be logically made, but without strictly enforcing that requirement)
                    (*note that , and : are not evaluated here and are instead split in the various math functions and other functions as needed. some args may not require any of that data)
                    (*if no data required simply write like this: FunctionName() )

             *   Multiplication:
                    SET(Var,a: 32)
                    SET(Var,b:  2)
                    DIV(Output,c: Var\a,Var\b)
                    SET(Output,d:Output\c)
                    MULT(Output,d:50)
                    
            
             */

        }
    }
   
	
}

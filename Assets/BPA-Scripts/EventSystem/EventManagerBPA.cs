using UnityEngine;
using System.Collections;
using Ackk.EventManagerSpace;
using System.Collections.Generic;
using System.Text;


public class EventManagerBPA : MonoBehaviour 
{

    /*
     * NOTE: Not that this isn't cool, but I don't like string based events, so I may replace this script in the future, or hash the event names
     * ore something in the future...
     */
	public List<EventPairing> eventsList = new List<EventPairing> ();
	private static Object thislock = new Object ();
	public static EventManagerBPA instance;


	// Use this for initialization
	void Awake () 
	{
		instance = this;
		DontDestroyOnLoad (instance);
	}


	//new simple event manager test. untested as of now.
    public void CallEvent(string eventName,object eventData)
	{
     //   Debug.LogWarning("!"+eventName+":"+eventData);

        //modifys a single instance of the event so that an event can be created without the "new" keyword *(unless the objecty is null of course...)*
        if (QuickEventCall==null)QuickEventCall=new EventBPA();
        QuickEventCall.eventName = eventName;
        QuickEventCall.eventData = eventData;
        CallEvent(QuickEventCall);
	}
     EventBPA QuickEventCall= new EventBPA();
    public void CallEvent(EventBPA eventCall)
    {
        for (int i = 0; i < eventsList.Count; i++) 
        {
            if (eventsList [i].eventName == eventCall.eventName) 
            {
                for (int e = 0; e < eventsList[i].eventListeners.Count; e++) 
                {
                    eventsList [i].eventListeners [e].TriggerEvent (eventCall);
                    if (eventsList [i].eventListeners [e].GetRemoveIfEventTriggered ()) 
                    {
                        //remove event if required
                        eventsList [i].eventListeners.Remove (eventsList [i].eventListeners [e]);
                        e--;
                    }
                }
                //as found a matching event, so register and return.
                return;
            }
        }
    }
	public void RegisterEventListener(IEventReceiver newReciever,string eventName)
	{
		for (int i = 0; i < eventsList.Count; i++) 
		{
			if (eventsList [i].eventName == eventName) 
			{
				RegisterEventListenerToEvent (i,newReciever);
				//as found a matching event, so register and return.
				return;
			}
		}
		//if you have reached this point, then the event is a new event, so add it then register.
		eventsList.Add(new EventPairing(eventName,newReciever));
	}
	public void UnRegisterEventListener(IEventReceiver newReciever,string eventName)
	{
		for (int i = 0; i < eventsList.Count; i++) 
		{
			if (eventsList [i].eventName == eventName) 
			{
				RegisterEventListenerToEvent (i,newReciever);
				//as found a matching event, so register and return.
				return;
			}
		}
	}
	void RegisterEventListenerToEvent(int index,IEventReceiver newReciever)
	{
		eventsList [index].eventListeners.Add (newReciever);
	}
    /// <summary>
    /// Evals a string and sends multiple event calls!
    /// </summary>
    /// <param name="s">S.</param>
    /// 
    StringBuilder sb = new StringBuilder();
    public void EvalString(string s)
    {
       
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
        if (sb == null)
        {
            sb = new StringBuilder();
        }
     
        string functionCallName = "";
        string dataObject = "";

        foreach (char c in s)
        {

            if (c == '(')
            {
                functionCallName = sb.ToString();
                sb.Length = 0;
            }
            else if (c == ')')
            {
                dataObject = sb.ToString();
                EventManagerBPA.instance.CallEvent(functionCallName.Trim(), dataObject.Trim());
                functionCallName = string.Empty;
                dataObject = string.Empty;
                sb.Length = 0;
            }
            else if (c != '\n' && c != '<' && c != '>')
            {
                sb.Append(c); 
            }
            //force clear line... a space between function calls would work as well
            if (c == ';')
            {
                functionCallName = string.Empty;
                dataObject = string.Empty;
                sb.Length = 0;
            }
        }
    }

}
namespace Ackk.EventManagerSpace
{
	[System.Serializable]
	public class EventPairing
	{
		public EventPairing()
		{
			//empty constructor
		}
		public EventPairing(string nameOfEvent)
		{
			eventName = nameOfEvent;
		}
		public EventPairing(string nameOfEvent, IEventReceiver initialReciever)
		{
			eventName = nameOfEvent;
			eventListeners.Add (initialReciever);

		}
		public void RemoveEventListener(IEventReceiver removeThisListener)
		{
			eventListeners.Remove (removeThisListener);
		}
		public string eventName;
        
		public List<IEventReceiver> eventListeners = new List<IEventReceiver> ();

	}
    [System.Serializable]
    public class EventBPA
    {
        public string eventName;
        public object eventData;
        public EventBPA()
        {
            eventName = string.Empty;
            eventData = string.Empty;
        }
        public EventBPA(string name,object data)
        {
            eventName = name;
            eventData = data;
        }
    }

	public interface IEventReceiver
	{
        void TriggerEvent (EventBPA eventRecieved);
		bool GetRemoveIfEventTriggered();
	}
}
using UnityEngine;
using System.Collections;
using Ackk.Text.Parsing;
using Ackk.GameEngine.Helpers;


public class Screenplay_CallObjectByName : MonoBehaviour 
{
	int lastLine=-1;
	PrescriptAction[] prescriptActions;

	[Header("The Action to Respond to('Actor' by default):")]
	public string Action="Actor";

	[Header("The Name of actor:")]
	public string ActorName="NameHere";
	const string hideAction = "HideAll";
	[Header("The 'emotions' name and object pairings:")]
	public ObjectNamePair[] emotions;

	void Start()
	{
	
		HideAllObjects ();
	}
	void HideAllObjects()
	{
		foreach (ObjectNamePair p in emotions) {
			//disable all subobjects byDefault...
			GameObjectOperations.EnableDisableGameObjects(p.gObject,false);
		}
	}
	//Use format:
	//</Action/NameInObjectList/
	// Update is called once per frame
	void Update () 
	{
		if (lastLine != ScreenplayReader.instance.GetCurrentLineValue ()) 
		{
			lastLine=ScreenplayReader.instance.GetCurrentLineValue();
			//end and  hide all objects if is not reading.
			if(ScreenplayReader.instance.CheckIfIsReading()==false)
			{
				HideAllObjects ();
				return;
			}
			prescriptActions=ScreenplayReader.instance.GetCurrentSentenceUnitData().prescriptActions.ToArray();
			if(prescriptActions!=null)
			{
				foreach(PrescriptAction psAction in prescriptActions)
				{
					//not enough data, so move along...
					if(psAction.dataPack.Length<1)continue;

					//has enough data, proccess:
					if(psAction.dataPack[0].ToLower()==Action.ToLower())
					{
						if(psAction.dataPack[1].ToLower()==hideAction.ToLower())
						{
							HideAllObjects ();
							continue;
						}
						//not enough data, so move along...
						if(psAction.dataPack.Length<2)continue;

						//enable an objects based on the requested data.
						if(psAction.dataPack[1].ToLower()==ActorName.ToLower())
						{
							int parseData=-1;
							if(int.TryParse(psAction.dataPack[2],out parseData))
							{
								//enable object by emotion id
								GameObjectOperations.EnableDisableObjectNamePair(emotions,parseData,true);
							}
							else
							{
								//Enable by name of emotion
								GameObjectOperations.EnableDisableObjectNamePair(emotions,psAction.dataPack[2],true);
							}
						}
					
					}
				}
			}

		}
	}
}

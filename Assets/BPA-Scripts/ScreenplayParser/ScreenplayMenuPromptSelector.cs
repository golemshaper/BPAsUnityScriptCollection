using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ScreenplayMenuPromptSelector : MonoBehaviour 
{
	bool promptIsOpen=false;
	public List<string> choicesList= new List<string>();
	public StringWriter outputTemplate;
	public GameObject EnableIfActive;
	public void ClearChoicesList()
	{
		choicesList= new List<string>();
	}
	public void OpenPrompt()
	{
		StartCoroutine (PromptUpdate ());
	}
	public void AddChoice(string choice)
	{
		//choicesList.Add ("("+(choicesList.Count+1)+") "+choice);
		
		choicesList.Add (choice);
	}
	public bool selectPressed;
	public int selection = 0;
	public int GetChoice()
	{
		return selection;
	}
	public bool isPromptOpen()
	{
		return promptIsOpen;
	}
	void Start()
	{
		HidePromptDisplay ();
	}
	public void HidePromptDisplay()
	{
		if (EnableIfActive != null)
			EnableIfActive.SetActive (false);
		outputTemplate.ClearText ();
	}
	void RenderChoiceMenu()
	{
		//this is temporary. eventually make each choice its own object
		//with its own string writer.
		string choiceListAsSingleString = string.Empty;
		int curLine = 0;
		foreach (string choice in choicesList) 
		{
            
			string formatSel="";
			string pointingCursor=" ";
			if(curLine==selection)
			{
				formatSel="|";
				//shift to alt character set with a hand cursor pointing right defined.
				pointingCursor="^>^";
			}
			choiceListAsSingleString+=pointingCursor+formatSel+choice+formatSel+"\n";
			curLine++;
		}
		outputTemplate.SetText (choiceListAsSingleString, false);
	}
	int lastSel=-1;
	IEnumerator PromptUpdate()
	{
		promptIsOpen = true;
		if (EnableIfActive != null)
			EnableIfActive.SetActive (true);
		RenderChoiceMenu ();
		selection = 0;
		selectPressed = false;
		while (PlayerInput.instance.GetFire1_B()) {
			//do not allow it to go to the menu unless the player is not holding fire 1.
			yield return true;
		}
		yield return new WaitForSeconds (0.1f);
		while (selectPressed==false) 
        {
			//Getjoystick input.
			InputUpdate();
			if(lastSel!=selection)
			{
				lastSel=selection;
				RenderChoiceMenu ();
			}
			//Clamp selection.
			if(selection>choicesList.Count-1)selection=0;
			if(selection<0)selection=choicesList.Count-1;
			
			if(PlayerInput.instance!=null)
			{
				selectPressed=PlayerInput.instance.GetFire1_B();
			}
			//menu input update loop here

			yield return true;
		}
		selectPressed = false;
		yield return true;
		promptIsOpen = false;
		HidePromptDisplay ();
		yield break;
	}





	//Menu Control variables------------------
	public float deley=0;
	const float maxDeleySpeed=1f;
	const float quickMove=0.2f;
	bool enableQuickMove=false;
	//Menu Control variables------------------


	public void InputUpdate()
	{
		if(PlayerInput.instance.GetMovement(true).y==0)
		{
			enableQuickMove=false;
			deley=0f;
		}
		if(deley>0)
		{
			deley-=Time.deltaTime;
			return;
		}
		if(PlayerInput.instance.GetMovement(true).y<0)
		{
			selection++;
		//	AudioManager.instance.PlaySFX(GlobalPlayerManager.instance.GetInventoryManager().SoundMoveCursor);
			if(enableQuickMove)deley=quickMove;
			else deley=maxDeleySpeed;
			enableQuickMove=true;
			
			return;
		}	
		if(PlayerInput.instance.GetMovement(true).y>0)
		{
			//deley...
			
			selection--;
		//	AudioManager.instance.PlaySFX(GlobalPlayerManager.instance.GetInventoryManager().SoundMoveCursor);
			if(enableQuickMove)deley=quickMove;
			else deley=maxDeleySpeed;
			enableQuickMove=true;
			return;
		}
		
	}

}

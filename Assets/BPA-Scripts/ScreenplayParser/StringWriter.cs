using UnityEngine;
using System.Collections;
using System.Text;
using TMPro;

public class StringWriter : MonoBehaviour {

	public TextMesh DisplayAsTextmesh;
    public TextMeshProUGUI DisplayAsTextMeshPro;
	public bool typewritter;
	public float printYieldTime=0.001f;
	public bool ifNoPuntuationMarkAtEndDrawPeriod=true;
    public bool handleVariables = true;
    const string variableStorageGroup = "Variables";
    const string missingVariableName = "???";
	void Awake()
	{
		if (DisplayAsTextmesh == null)
			DisplayAsTextmesh = this.GetComponent<TextMesh> ();
        if (DisplayAsTextMeshPro == null)
            DisplayAsTextMeshPro = this.GetComponent<TextMeshProUGUI>();
    }
	public int maxCharCountPerLine = 50;
	public bool onlyBreakOnSpaces=true;
    string FilterForVariables(string filterThis)
    {
        if (handleVariables == false)
            return filterThis;
        StringBuilder sb= new StringBuilder();
        StringBuilder sb2= new StringBuilder();
        bool isInVariableMode = false;
        
        foreach (char c in filterThis)
        {
            if (c == '$')
            {
                isInVariableMode = true;
                continue;
            }
            if (isInVariableMode)
            {
                //consider replacing this with a loop.
                if (c == ' ' || c == '\n' || c == '\n' || c == '?'|| c == '!' || c == '.' || c == ',' || c== '|' || c=='©')
                {
                    //Replace variables name with contents of key.
                    isInVariableMode = false;
                    sb.Append(IniGameMemory.instance.GetDataString(variableStorageGroup, sb2.ToString().Trim(), missingVariableName));
                    sb2.Length = 0;

                }
            }
            if (isInVariableMode)
            {
               //variable text
                sb2.Append(c);
            }
            else
            {
                //main text.
                sb.Append(c);
            }

        }
        if (isInVariableMode)
        {
            //if it's still in variable mode, then the line didn't end, so close it off now and append the variable.
            isInVariableMode = false;
            sb.Append(IniGameMemory.instance.GetDataString(variableStorageGroup, sb2.ToString(), missingVariableName));
            sb2.Length = 0;
        }
        return sb.ToString();
    }
	public void SetText(string input)
	{
	
        SetText (input, true);
	}
	public void SetText(string input, bool useTypewriter)
	{
		typewritter = useTypewriter;

        if (gameObject.activeSelf)
        {
            StartCoroutine(Typewriter(FilterForVariables(input), typewritter));

        }



    }
	public bool IsPrinting()
	{
		return isTyping;
	}
	bool isTyping=false;
	public bool skip=false;
	public void EndTypewriterEffect()
	{
		skip = true;
	}
//Text mesh typwriter output.
	string lastInput=string.Empty;
	void setTextMesh(string input)
	{
		if (input == lastInput) {
			return;
		}
		lastInput = input;
		if (DisplayAsTextmesh != null) 
		{
			DisplayAsTextmesh.text = input;
		}
        if (DisplayAsTextMeshPro != null)
        {
            DisplayAsTextMeshPro.text = input;
        }

    }
	public void ClearText()
	{
		lastInput=string.Empty;
		if(DisplayAsTextmesh!=null)DisplayAsTextmesh.text=lastInput;
        if (DisplayAsTextMeshPro != null)DisplayAsTextMeshPro.text=lastInput;
        
	}
	const string punctuationMarks = "-).!,?`|©:…";

	public bool IsCharAPuntuationMark(char c)
	{
		foreach(char m in punctuationMarks)
		{
			if(c==m)return true;
		}
		return false;
	}

	IEnumerator Typewriter(string outputData,bool useTimeYield)
	{
		isTyping = true;
		string collect = string.Empty;
		int curCharCount = 0;
		setTextMesh (string.Empty);
		if(useTimeYield)yield return new WaitForSeconds (0.1f);
		if (ifNoPuntuationMarkAtEndDrawPeriod) 
		{
			if(string.IsNullOrEmpty(outputData)==false && outputData.Length>1)
			{
				if(IsCharAPuntuationMark(outputData[outputData.Length-1])==false)
				{
					//add a period if a puntuation mark was forgotten...
					outputData+=".";
				}
			}
		}
	
		foreach (char c in outputData)
        {
			float extraWaitTime = 0f;
			if (IsCharAPuntuationMark (c))
				extraWaitTime = 0.01f;
			if (curCharCount >= maxCharCountPerLine) {
				if (onlyBreakOnSpaces) {
					if (char.IsWhiteSpace (c)) {
						collect += "\n";
						curCharCount = 0;
						continue;
					}
				} else {
					collect += "\n";
					curCharCount = 0;
				}
			}
			if (char.IsWhiteSpace (c) && curCharCount == 0) {
				curCharCount++;
				continue;
			}
			collect += c;
			if (useTimeYield) {
				yield return new WaitForSeconds (extraWaitTime);
			}
			setTextMesh (collect);
			if (skip) {
				setTextMesh (outputData);
				skip = false;
				isTyping = false;
				yield break;
			}
			if (useTimeYield) {

				yield return new WaitForSeconds (printYieldTime + extraWaitTime);

			}
			curCharCount++;
		}
		isTyping = false;
		yield break;
	}
}

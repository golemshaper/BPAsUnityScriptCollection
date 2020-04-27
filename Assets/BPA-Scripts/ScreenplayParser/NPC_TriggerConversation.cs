using UnityEngine;
using System.Collections;

public class NPC_TriggerConversation : MonoBehaviour {

	public string GroupName;
    public int startingLine=0;
	public bool TestTriggerNPC;
   
    public bool LoadFromSavedData;
    const string group = "Dialogue";
    const string key = "DialogueGroupKey";
    const string lineKey="CurrentLineNumber";
    public bool autoDisable;
    public bool triggerOnEnable;
    private void OnEnable()
    {
        if (!triggerOnEnable) return;
        if(ScreenplayReader.instance.CheckIfIsReading())
        {
            if (autoDisable)
            {
                this.gameObject.SetActive(false);
            }
            return;
        }
        ScreenplayReader.instance.BeginReadingFromGroup(GroupName, startingLine);
        if (autoDisable)
        {
            this.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update () {
		if (TestTriggerNPC) {
			TestTriggerNPC=false;
			if(ScreenplayReader.instance==null)return;

            ScreenplayReader.instance.BeginReadingFromGroup(GroupName,startingLine);
          
		}
	}
}

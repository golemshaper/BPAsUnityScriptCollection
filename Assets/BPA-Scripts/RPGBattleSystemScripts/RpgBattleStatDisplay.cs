using RPG.BPA;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RpgBattleStatDisplay : MonoBehaviour
{
    /*
     * I may need a way to connect to an arbitrary actor instead...
     */
    [Header("Connect to Hero Actor:")]
    public string connectActor = "Hero";
    public int myHeroIndex;
    public bool useIndexToConnectActor;
    public TextMeshProUGUI nameDisplay;
    public RollingNumberDisplay rollingNumberDisplay;
    public HealthBar barDisplay;
    RPGActor myActor;
    RpgBattleSystemMain rpg;

    public bool useATimeDelay = false;
    // Start is called before the first frame update
    void Start()
    {
        rpg = RpgBattleSystemMain.instance;
        if (useIndexToConnectActor)
        {
            ConnectToActor(myHeroIndex);
        }
        else
        {
            ConnectToActor(connectActor);
        }

    }
    void ConnectToActor(int index)
    {
        if (rpg.heroParty == null) return;
        if(rpg.heroParty.Count-1<index)
        {
            //out of bounds, disable this UI element.
            this.gameObject.SetActive(false);
            return;
        }
        myActor = rpg.heroParty[index];
        myActor.onHpChangedEvents += HpChanged;
        nameDisplay.text = myActor.displayName;

        //setup rolling number effect:
        ConfigureRollingNumber();
        ConfigureLifeBar();
    }
    void ConfigureLifeBar()
    {
        if (barDisplay == null) return;
        barDisplay.SetMaxValue(myActor.stats.GetMaxHP());
        barDisplay.SetValue(myActor.stats.hp,false);
    }
    void ConfigureRollingNumber()
    {
        if (rollingNumberDisplay == null) return;
        rollingNumberDisplay.SetPreText(rpg.battleMessage_csv.GetCSVData("HP"));
        rollingNumberDisplay.SetPostText("/" + myActor.stats.GetMaxHP().ToString());
        rollingNumberDisplay.SetValue(myActor.stats.hp, false);
        //---
    }
    void ConnectToActor(string connectStr)
    {
        ConnectToActor(rpg.GetHeroIndex(connectStr));
    }
    void HpChanged()
    {
        if(useATimeDelay)
        {
            ActionTimerManager.GetInstance().DoActionAfterTime(0.2f, () => rollingNumberDisplay.SetValue(myActor.stats.hp, true));
            ActionTimerManager.GetInstance().DoActionAfterTime(0.2f, () => barDisplay.SetValue(myActor.stats.hp, true));
        }
        else
        {
            if(rollingNumberDisplay!=null) rollingNumberDisplay.SetValue(myActor.stats.hp, true);
            if (barDisplay != null) barDisplay.SetValue(myActor.stats.hp,true);
        }

        //TODO: If hp bar, update here...
    }
}

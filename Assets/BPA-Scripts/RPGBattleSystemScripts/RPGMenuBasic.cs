using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.BPA;
using RPG.BPA.MENU;

public class RPGMenuBasic : MonoBehaviour
{
    //Write as many menus as you like.
    //use this method of communicating
    //this will make it easier change the game style (I hope)

    //if you don't want to use the name, do a foreach ofer hero party and create one for each index. (this is the real way to do it I'd say)
    public string MyActor = "Hero"; 
    public RPGMenuCommunication menuInterface = new RPGMenuCommunication();
    public List<string> skillStrings = new List<string>();

    public bool debugAwaitingInputMSG = true;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: INITIALIZE ON BATTLEMODE START INSTEAD!! 
        menuInterface.Initialize(MyActor);
    }
    bool limitOnce=false;
    public bool TEST;
    // Update is called once per frame
    void Update()
    {
        if (menuInterface.GetMenuIsActive() == false)
        {
            limitOnce = false;
            return;
        }
        if(!limitOnce)
        {
            limitOnce = true;
            skillStrings = menuInterface.GetSkillListAsStringList();
            if(debugAwaitingInputMSG)
            {
                RpgBattleSystemMain.instance.WriteToPrompt("Awaiting input...");
            }
        }
      
        //MENU CODE:
       // if(PlayerInput.instance.GetFire1_B())
       if(TEST)
        {
            TEST = false;
            //attack all with default skill for now:
            menuInterface.SetSkill(menuInterface.GetSkillList()[0]);
            menuInterface.SetTargets(menuInterface.enemyTargets);


        }
    }
}

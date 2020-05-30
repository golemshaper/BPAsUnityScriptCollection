using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.BPA;
using RPG.BPA.MENU;
using TMPro;

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

    public SimpleMenu simpleMenuDisplay;
    public SimpleMenuSlot slotTemplate;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: INITIALIZE ON BATTLEMODE START INSTEAD!! 
        menuInterface.Initialize(MyActor);

    }
    bool limitOnce=false;
    public bool TEST;
    /*
     * 
     * TODO: Draw list of skills in skill draw mode. then draw list of targets in target draw mode.
     * Have item draw mode that draws items as skills.
     * 
     * either that or make it more generic like the YIIK battle menu. not sure yet. pros and cons to each idea.
     * 
     * 
     * TODO: Select skill and then target and then execute.
     * 
     * Target aqquisition could be based on the enemy list and a max number of targets, or it could be based on collision.
     */
    // Update is called once per frame
    void Update()
    {
        if (menuInterface.GetMenuIsActive() == false)
        {
            //menu not active, ignore update loop...
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
            //menu pops open...

            //test skill draw for now...
            DrawSkillsMenu();
            //TODO: Make a state machine or something...
        }
      
        //MENU CODE:
       // if(PlayerInput.instance.GetFire1_B())
       if(TEST)
        {
            TEST = false;
            //select using cursor index of simple menu
            menuInterface.SetSkill(menuInterface.GetSkillList()[simpleMenuDisplay.cursorIndex]);
            menuInterface.SetTargets(menuInterface.enemyTargets);


        }
    }
    
    void DrawSkillsMenu()
    {
        //return all to the pool
        oldSlots.AddRange(simpleMenuDisplay.menuSlots);
        foreach(var s in oldSlots)
        {
            s.gameObject.SetActive(false);
        }
        simpleMenuDisplay.menuSlots.Clear();
       
        Vector3 firstSlotPos = slotTemplate.transform.position;
        Vector3 offsetByAmount = new Vector3(0, -75f, 0f);
        List<Skill> skillsList = menuInterface.GetSkillList();

        for (int i = 0; i < skillsList.Count; i++)
        {
            SimpleMenuSlot slotGFX = CreateOrRecycleSlot();
            slotGFX.transform.position = firstSlotPos+(offsetByAmount * i);
           
            simpleMenuDisplay.menuSlots.Add(slotGFX);
            slotGFX.GetComponent<TextMeshProUGUI>().SetText(skillsList[i].skillName);
            slotGFX.gameObject.SetActive(true);
        }
    }
    //old slot becomes like an object pool
    List<SimpleMenuSlot> oldSlots= new List<SimpleMenuSlot>();
    SimpleMenuSlot CreateOrRecycleSlot()
    {
        SimpleMenuSlot nSlot=null;
        if(oldSlots.Count>0)
        {
            nSlot = oldSlots[0];
            oldSlots.Remove(nSlot);
        }
      
        if (oldSlots == null)
        {
            nSlot= Instantiate(slotTemplate) as SimpleMenuSlot;
        }
        return nSlot;
    }
}

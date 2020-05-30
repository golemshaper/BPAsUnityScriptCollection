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
    [Header("Connect to Actor:")]
    public string MyActor = "Hero"; 
    public RPGMenuCommunication menuInterface = new RPGMenuCommunication();
    public List<string> skillsLoaded_DebugView = new List<string>();

    public bool debugAwaitingInputMSG = true;
    [Header("Simple Menu interface")]
    public SimpleMenu simpleMenuDisplay;
    public SimpleMenuSlot slotTemplate;
    //consider driving the menu with the state machine...
    enum MenuState {inactive,selectSkill,selectTarget};
    public AckkStateMachine sm = new AckkStateMachine();
    public int debugState = -1;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: INITIALIZE ON BATTLEMODE START INSTEAD!! 
        menuInterface.Initialize(MyActor);
        simpleMenuDisplay.neverUseUpdate = true; //we'll control the update in this class...
        simpleMenuDisplay.gameObject.SetActive(false);
        //-----BIND ACTIONS---
        sm.LinkStates(MenuState.selectSkill,()=>UpdateSkillsMenu(),()=>OnEnterSkillMenu());
        sm.LinkOnEnterState(MenuState.inactive,()=> OnEnterInactiveState());
        sm.SetState(MenuState.inactive);
    }
    void OnEnterInactiveState()
    {
        simpleMenuDisplay.gameObject.SetActive(false);
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
        debugState = sm.GetState();
        if (menuInterface.GetMenuIsActive() == false)
        {
            //menu not active, ignore update loop...
            limitOnce = false;
            return;
        }
        if (!limitOnce)
        {
            limitOnce = true;
            //Make sure that you always open in the skill state! when you enter all of the actions and close the menu, be sure to go back to inactive...
            sm.SetState(MenuState.selectSkill);
        }
        sm.UpdateTick();
    }

    void OnEnterSkillMenu()
    {
        skillsLoaded_DebugView = menuInterface.GetSkillListAsStringList();
        if (debugAwaitingInputMSG)
        {
            RpgBattleSystemMain.instance.WriteToPrompt("Awaiting input...");
        }
     
         //menu pops open...
        delayInput = 0.2f;
        DrawSkillsMenu();

        //TODO: Make a state machine or something...
    }
    void UpdateSkillsMenu()
    {
        //Ignore input for time...
        if (delayInput > 0f)
        {
            delayInput -= Time.deltaTime;
            return;
        }
        //Call menu update:
        simpleMenuDisplay.DoUpdate();

        //UPDATE:
        if (PlayerInput.instance.GetFire2_A())
        {
            //select using cursor index of simple menu
            menuInterface.SetSkill(menuInterface.GetSkillList()[simpleMenuDisplay.cursorIndex]);
            menuInterface.SetTargets(menuInterface.enemyTargets);
            //TODO: Actually, go to the target select menu instead of the inactive state, but I'll do this for now...
            sm.SetState(MenuState.inactive);
        }

    }

    float delayInput = 0f;
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
        /*
         * TODO: Make this more generic so you can re-use for the enemy targets list.
         * also, scale the backing window to fit the number of SlotNodes and nothing more
         */
        for (int i = 0; i < skillsList.Count; i++)
        {
            SimpleMenuSlot slotGFX = CreateOrRecycleSlot();
            slotGFX.transform.position = firstSlotPos+(offsetByAmount * i);
           
            simpleMenuDisplay.menuSlots.Add(slotGFX);
            slotGFX.GetComponent<TextMeshProUGUI>().SetText(skillsList[i].skillName);
            slotGFX.gameObject.SetActive(true);
        }
        simpleMenuDisplay.gameObject.SetActive(true);
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

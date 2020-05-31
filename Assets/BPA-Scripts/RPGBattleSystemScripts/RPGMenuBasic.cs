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
    enum MenuState {inactive,selectSkill,selectTarget,selectAllTargets};
    public AckkStateMachine sm = new AckkStateMachine();
    public int debugState = -1;

    int skillCursorMemory = 0;
    int targetCursorMemory= 0;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: INITIALIZE ON BATTLEMODE START INSTEAD!! 
        menuInterface.Initialize(MyActor);
        simpleMenuDisplay.neverUseUpdate = true; //we'll control the update in this class...
        simpleMenuDisplay.gameObject.SetActive(false);
        //-----BIND ACTIONS---
        sm.LinkStates(MenuState.selectSkill,()=>UpdateSkillsMenu(),()=>OnEnterSkillMenu());
        sm.LinkStates(MenuState.selectTarget,()=>TargetSelectUpdate(),()=>OnEnterTargetSelect());
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
    //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 
    //__SKILL MENU__
    void OnEnterSkillMenu()
    {
        skillsLoaded_DebugView = menuInterface.GetSkillListAsStringList();
        if (debugAwaitingInputMSG)
        {
            RpgBattleSystemMain.instance.WriteToPrompt("Awaiting input...");
        }

        //menu pops open...
        delayInput = 0.2f;
        //DrawSkillsMenu();
        DrawMenuGeneral(menuInterface.GetSkillListAsStringList());
        //remember in current menu!
        skillCursorMemory = ClampToMenuSize(skillCursorMemory);
        simpleMenuDisplay.cursorIndex = skillCursorMemory;
        //remember in current menu!

    }
    void UpdateSkillsMenu()
    {
        if (sm.TimeInState > 0.2f)
        {
            if (simpleMenuDisplay.gameObject.activeSelf == false)
            {
                simpleMenuDisplay.gameObject.SetActive(true);
                //return early, but still counts as part of the delay.
                delayInput -= Time.deltaTime;
                return;
            }
           
        }

        //Ignore input for time...
        if (delayInput > 0f)
        {
            delayInput -= Time.deltaTime;
            return;
        }
        //Call menu update:
        simpleMenuDisplay.DoUpdate();
        skillCursorMemory = simpleMenuDisplay.cursorIndex;
        //UPDATE:
        if (PlayerInput.instance.GetFire2_A())
        {
            //select using cursor index of simple menu
            menuInterface.SetSkill(menuInterface.GetSkillList()[simpleMenuDisplay.cursorIndex]);
            //TODO: Actually, go to the target select menu instead of the inactive state, but I'll do this for now...
            //target select mode should be dictated by skill selected!
            //define in skill definitions file!
            sm.SetState(MenuState.selectTarget);
            simpleMenuDisplay.gameObject.SetActive(false);
            //NOTE: THE TYPE OF TARGET THAT YOU CAN SELECT SHOULD BE DEFINED PER SKILL!
            //HEALING SPELLS SHOULD TARGET HERO PARTY, BUT IF YOU WANT TO MAKE A GAME WHERE 
            //YOU CAN HURT OR HEAL ANYTHING, YOU SHOULD BE ABLE TO WITHOUT MODDING THE ENGINE!
        }

    }
    //__SKILL MENU__
    //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 
    //__TARGET MENU__
    //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 

    void OnEnterTargetSelect()
    {
        skillsLoaded_DebugView = menuInterface.GetSkillListAsStringList();
        if (debugAwaitingInputMSG)
        {
            RpgBattleSystemMain.instance.WriteToPrompt("Awaiting input...");
        }



        //delay for a bit
        delayInput = 0.2f;
        //menu pops open...
        DrawMenuGeneral(menuInterface.GetTargetListAsStringList(menuInterface.skillToPerform));
        //remember in current menu!
        targetCursorMemory = ClampToMenuSize(targetCursorMemory);
        simpleMenuDisplay.cursorIndex = targetCursorMemory;
        //remember in current menu!
    }
    void TargetSelectUpdate()
    {
        if(sm.TimeInState>0.2f)
        {
            if (simpleMenuDisplay.gameObject.activeSelf==false)
            {
                simpleMenuDisplay.gameObject.SetActive(true);
                //return early, but still counts as part of the delay.
                delayInput -= Time.deltaTime;
                return;
            }
        }
        //Ignore input for time...
        if (delayInput > 0f)
        {
            delayInput -= Time.deltaTime;
            return;
        }
        //Call menu update:
        simpleMenuDisplay.DoUpdate();
        targetCursorMemory = simpleMenuDisplay.cursorIndex;
        //TODO: Add a back button!

        //UPDATE:
        if (PlayerInput.instance.GetFire2_A())
        {
            //select using cursor index of simple menu
            menuInterface.SetTargets(menuInterface.enemyTargets);
            //TODO: Actually, go to the target select menu instead of the inactive state, but I'll do this for now...
            sm.SetState(MenuState.inactive);
            //NOTE: THE TYPE OF TARGET THAT YOU CAN SELECT SHOULD BE DEFINED PER SKILL!
            //HEALING SPELLS SHOULD TARGET HERO PARTY, BUT IF YOU WANT TO MAKE A GAME WHERE 
            //YOU CAN HURT OR HEAL ANYTHING, YOU SHOULD BE ABLE TO WITHOUT MODDING THE ENGINE!
        }
    }
    //__TARGET MENU__
    //. . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . 

    float delayInput = 0f;



    int ClampToMenuSize(int cursorIndex)
    {
        if (cursorIndex > simpleMenuDisplay.menuSlots.Count - 1) cursorIndex = 0;
        if (cursorIndex < 0) cursorIndex = simpleMenuDisplay.menuSlots.Count - 1;
        return cursorIndex;
    }
    void DrawMenuGeneral(List<string> listToDraw)
    {
        //return all to the pool
        oldSlots.AddRange(simpleMenuDisplay.menuSlots);
        foreach (var s in oldSlots)
        {
            s.gameObject.SetActive(false);
        }
        simpleMenuDisplay.menuSlots.Clear();

        Vector3 firstSlotPos = slotTemplate.transform.position;
        Vector3 offsetByAmount = new Vector3(0, -75f, 0f);
        slotTemplate.gameObject.SetActive(false); //MAKE SURE SLOT TEMPLATE IS NOT PART OF THE MENU SLOTS< OR THE INITIAL POSITION WILL JUMP!
        for (int i = 0; i < listToDraw.Count; i++)
        {
            SimpleMenuSlot slotGFX = CreateOrRecycleSlot();
            slotGFX.transform.parent = slotTemplate.transform.parent;
            slotGFX.transform.position = firstSlotPos + (offsetByAmount * i);

            simpleMenuDisplay.menuSlots.Add(slotGFX);
            slotGFX.GetComponent<TextMeshProUGUI>().SetText(listToDraw[i]);
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
            return nSlot;
        }
        
        nSlot= Instantiate(slotTemplate) as SimpleMenuSlot;
        return nSlot;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.BPA;
using RPG.BPA.MENU;
using TMPro;

public class RPGMenuBasic : MonoBehaviour
{
    private const string skillMenuHeader = "SkillsMenuHeader";

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
    public float spacing = -75f;
    public SimpleMenu simpleMenuDisplay;
    public SimpleMenuSlot slotTemplate;
    public TextMeshProUGUI menuHeaderText;
    //consider driving the menu with the state machine...
    enum MenuState {inactive,selectSkill,selectTarget,selectAllTargets};
    public AckkStateMachine sm = new AckkStateMachine();
    public int debugState = -1;

    int skillCursorMemory = 0;
    int targetCursorMemory= 0;
    public bool GetConfirm()
    {
        return PlayerInput.instance.GetFire2_A();
    }
    public bool GetBack()
    {
        return PlayerInput.instance.GetFire1_B();
    }
    // Start is called before the first frame update
    void Start()
    {
        //TODO: INITIALIZE ON BATTLEMODE START INSTEAD!! 
        menuInterface.Initialize(MyActor);
        simpleMenuDisplay.neverUseUpdate = true; //we'll control the update in this class...
        simpleMenuDisplay.gameObject.SetActive(false);
        menuInterface.onTerminate += OnTerminateMenuEarly;
        //-----BIND ACTIONS---
        sm.LinkStates(MenuState.selectSkill,()=>UpdateSkillsMenu(),()=>OnEnterSkillMenu());
        sm.LinkStates(MenuState.selectTarget,()=>TargetSelectUpdate(),()=>OnEnterTargetSelect());
        sm.LinkOnEnterState(MenuState.inactive,()=> OnEnterInactiveState());
        sm.SetState(MenuState.inactive);
    }
    void OnTerminateMenuEarly()
    {
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
    void DrawSkillMenuHeaderTitle()
    {
        if (menuHeaderText != null) menuHeaderText.text = string.Format(RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData(skillMenuHeader),
       menuInterface.GetMyActor().displayName, "Skills");
        //TODO: Replace the string "Skills" with the currently equipped+selected weapon name. multiple weapons can be equipped and skills will be filtered by weapon type (not implemented yet)
        skillsLoaded_DebugView = menuInterface.GetSkillListAsStringList();
    }

    //__SKILL MENU__
    void OnEnterSkillMenu()
    {
        DrawSkillMenuHeaderTitle();
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
        //------------------
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
        //------------------
        //UPDATE:


        //TODO: Make it so you can press left/right to select a different weapon, causing new skills to be drawn. 
        //(reset cursor memory if you do that, or make a new memory int for each weapon type
        if(GetBack())
        {
            //TODO!
            // now I need an option to go back if you want to cancel all of the commands!
            //you'll need to add the previous menu to RpgBattleSystemMain.instance.menusInputQueueList!!
            bool success=  RpgBattleSystemMain.instance.GoBackToPreviousMenu();
            if(success==false)
            {
                //TODO: Play error sound.
            }
        }
        if (GetConfirm())
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
    void DrawTargetMenuHeaderTitle()
    {
        // 'Target' is a string that says Target in english. you can make it something else in another language...

        if (menuHeaderText != null) menuHeaderText.text = string.Format(RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData(skillMenuHeader),
        menuInterface.skillToPerform.skillName,RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("Target"));
        skillsLoaded_DebugView = menuInterface.GetSkillListAsStringList();
    }
    void OnEnterTargetSelect()
    {
        skillsLoaded_DebugView = menuInterface.GetSkillListAsStringList();
        if (debugAwaitingInputMSG)
        {
            RpgBattleSystemMain.instance.WriteToPrompt("Awaiting input...");
        }
        DrawTargetMenuHeaderTitle();


        //delay for a bit
        delayInput = 0.2f;
        //menu pops open...
        DrawMenuGeneral(menuInterface.GetAttackTargetListAsStringList(menuInterface.skillToPerform));
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
        if(GetBack())
        {
            //back button pressed
            sm.SetState(MenuState.selectSkill);
            //turn off menu so it uses the open effect again...
            simpleMenuDisplay.gameObject.SetActive(false);
            return;
        }
        if (GetConfirm())
        {
            //select using cursor index of simple menu
            menuInterface.SetTargets(menuInterface.availibleTargets[simpleMenuDisplay.cursorIndex]);
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
        Vector3 offsetByAmount = new Vector3(0, spacing, 0f);
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

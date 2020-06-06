using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ackk.INI.Helpers;
using RPG.BPA.MENU;
using RPG.Elements;

namespace RPG.BPA
{
    public class RpgBattleSystemMain : MonoBehaviour
    {
        /*
         * What this will be:
         * A generic RPG battle mode with optional ATB.
         * Myabe allow for FFX style speed as well. We'll see.
         * 
         * It must be able to have enemies added at runtime and battles should be able to be stopped and started from a simple function call.
         * 
         * I want this running in its own scene, but I never want to unload that scene so that battles don't have any load time to start.
         * 
         * Items and skills will be a thing in this game.
         * Enemies and players should have AI programable in the editor using a system similar to FFXII Gambits.
         * 
         * As much data as possible should be programable from files not found in the unity editor.
         * Graphics, animations, enemy stats, and enemy AI should be decoupled!
         * 
         * Enemy stats should be defined in an ini like file:
         * -------------------
         * [Slime]
         *      GFX=Slime
         *      Hp=12
         *      Def=1
         *      Atk=1
         *      Spd=1
         * -------------------
         * Preferably, no enemy prefabs will be used. instead, enemy prefabs will only contain the graphics data.
         * Graphics will be loaded from the GFX tag in the ini file. These should be unity Assets maybe.
         * 
         * A battle trigger from the overworld or dungeon scenes will ask for a list of enemies, and you will load them from the ini group name.
         * 
         * Skills should be programmed, but the effects should be bindable to skills or items defined in an ini file.
         * Not sure what this will look like yet.
         * 
         * I want this to be really easy to use for large games. based on what I learned from my work on previous projects.
         * 
         * Keep localization in mind when working on this battle sytem!
         * 
         * Graphics will be able to be set up in a way that it auto-creates movement animation like squash stretch, and arc to jump to an enemy position.
         * I want this to be super easy to set up in a 2D or 3D game. 
         * 
         * 
         * 
         * 
         * Enemies should be no different from player characters.
         * The skill menu should be able to read skills from enemies as well.
         * This will enable things like monster collecting
         * 
         */

        [Header("TESTS")]
        public bool Test1;
        public bool Test2;
        public int test2Data = 100;
        public bool RESET_BATTLE_TEST;
        [Header("Setup")]
        public StringWriter promptDisplay;
        public StringWriter previousPromptDisplay;
        [Header("Game Data")]
        public TextAsset EnemyStatDefinitions;
        public IniGeneralUse actorStatsData;
        public TextAsset battleMessages;
        public TextAsset skillDefinitions;
        public IniGeneralUse skillData;
        public RPGSkillParser skillsParserRealSkills = new RPGSkillParser();
        public CommaSeperatedValueParser battleMessage_csv= new CommaSeperatedValueParser();

        public static RpgBattleSystemMain instance;
        public enum BattleType { TurnBased, ATB, MultiTurnSpeedBased };
        [Header("Settings")]
        public BattleType battleType = BattleType.TurnBased;
        //...
        //Use this to require all player input before turns play out!
        public bool decideActionsFirst = false;
        List<RPGMenuCommunication> menusInputQueueList = new List<RPGMenuCommunication>();
        List<RPGMenuCommunication> menuHistoryQueue = new List<RPGMenuCommunication>();
        //...
        [Header("Current Data")]
        public int curTurn = 0;

        public List<RPGActor> heroParty = new List<RPGActor>();
        public List<RPGActor> enemyParty = new List<RPGActor>();
        public List<RPGActor> AllActors = new List<RPGActor>();
        public float battleSpeed = 1f;


        public List<string> battleLog= new List<string>();
        string battlePrompt=string.Empty;
        //ACTIONS TO PERFORM
        public List<ActionNode> actionQueue = new List<ActionNode>();
        //maybe re-use actions so you don't need to use the New keyword!
        public List<ActionNode> actionPool = new List<ActionNode>();

        private void Awake()
        {
            instance = this;
            //parse text data:
            actorStatsData.MemoryFromIniString(EnemyStatDefinitions.text);
            battleMessage_csv.Parse(battleMessages.text);
            //--skill data:
            skillData.MemoryFromIniString(skillDefinitions.text);
            skillsParserRealSkills.ParseSkillsFromData(skillDefinitions.text);
            //--

            //initialize default hero party. will be overriden by the party menu probably.
            //this is really only being calleed here right now as a test. remove later:
            MockData();
            //^-mock data only
        }
        public int GetHeroIndex(string name)
        {
            for (int i = 0; i < heroParty.Count; i++)
            {
                if (heroParty[i].name.ToLower()==name.ToLower())
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// WARNING: This will only return the first instance! This isn't really useful except for some work in progress type code.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetEnemyIndex(string name)
        {
            for (int i = 0; i < heroParty.Count; i++)
            {
                if (enemyParty[i].name.ToLower() == name.ToLower())
                {
                    return i;
                }
            }
            return -1;
        }
        void MockData()
        {
            //initialize default hero party. will be overriden by the party menu probably.
            //this is really only being calleed here right now as a test. remove later:
            //AUTO LEVEL EXAMPLE

        
            //-------------------------------------------------------------------------------------------
            //load the hero stats so that you can set the enemy to a matching level for this mock data:
            heroParty[0].LoadStatsFromSaveFile(actorStatsData);
            enemyParty[0].stats.lvl = 1;
            //-------------------------------------------------------------------------------------------
            //-------------------------------------------------------
            //Set hero stats with level formula for now

            //-------------------------------------------------------
            //-------------------------------------------------------------------------------------------

            //FORCE ADD PARTY:
            SetHeroPartyList(heroParty);
            SetEnemyParty(enemyParty);

            StartBattle();
        }
        //Call when you create a new party layout.
        public void SetHeroPartyList(List<RPGActor> setHeroParty)
        {
            heroParty = setHeroParty;
            foreach (var a in heroParty)
            {
                a.LoadStatsFromSaveFile(actorStatsData);
                a.LoadSkills(actorStatsData);
            }
        }
        //Call before start battle!
        public void SetEnemyParty(List<RPGActor> setEnemyParty)
        {
            enemyParty = setEnemyParty;
            foreach(var a in enemyParty)
            {
                a.useAI=true; //enemies use AI.
                a.LoadStatsFromStatsFile(actorStatsData);
                a.LoadSkills(actorStatsData);
            }
        }
        void FillMenuInputQueue()
        {

            menuHistoryQueue.Clear();
            menusInputQueueList.Clear();
            foreach (RPGActor a in heroParty)
            {
                if (a.stats.hp <= 0) continue; //skip if dead...
                menusInputQueueList.Add(a.GetMenuInterface());
            }
          
            Debug.Log("Full up menu");
        }

        public void AppendAlphabetToName(string originalName)
        {
            //sorry about this maddness
            string[] modifier = "A,B,C,D,E,F,G,h,I,J,K,L,M,N,O,P,Q".Split(",".ToCharArray());
            int count = 0;
            int rememberFirstInstance = -1;
            bool pleaseModifyOriginal = false;
            for (int i = 0; i < enemyParty.Count; i++)
            {
                RPGActor a = enemyParty[i];
                if (a.name != originalName) continue;
                if(count>0)
                {
                    a.alphabetPlacementAppend = " " + modifier[count];
                    pleaseModifyOriginal = true;
                }
                else
                {
                    rememberFirstInstance = i;
                }

                count++;
            }
            if(pleaseModifyOriginal)
            {
                enemyParty[rememberFirstInstance].alphabetPlacementAppend = " " + modifier[0];
            }
        }
        public void StartBattle()
        {
            //clear last battle log.
            battleLog.Clear();
            curTurn = 0;
            AllActors.Clear();
            
            //setup default AI targets...
            foreach (RPGActor a in heroParty)
            {
                a.LoadDiplayName();
                a.ai.SetTargets(enemyParty);
            }
            foreach (RPGActor a in enemyParty)
            {
                a.ai.SetTargets(heroParty);
                AppendAlphabetToName(a.name);
                a.LoadDiplayName();
                WriteToPrompt(String.Format(battleMessage_csv.GetCSVData("EnemyAppears"), a.displayName));
            }
            //add all to the acting actors list:

            //Add hero's that are in party to the list:
            foreach (RPGActor a in heroParty)
            {
                if (a.isInParty) AllActors.Add(a);
            }
            //Add the enemy list
            AllActors.AddRange(enemyParty);
            //SORT SPEED:
            //LINQ version: AllActors = AllActors.OrderBy(o => o.stats.spd).ToList();
            //Non-Linq sort by speed:
            AllActors.Sort((y,x) => x.stats.spd.CompareTo(y.stats.spd));

            //FILL UP MENU QUEUE FIRST!
            if (decideActionsFirst)
            {
                CreateAction(string.Empty,0f,() => FillMenuInputQueue(),null);
               
            }
            allowUpdate = true;
        }
        public void EndBattle()
        {
            //battle clean up here...
            
            allowUpdate = false;
        }
        void SetUpBattleActors()
        {
            //load in the graphics for players and enemis and place them on the field.
            //populate target menus
            //TODO: 
        }
        public void CreateAction(string message)
        {
            CreateAction(message,  1f);
        }
        public void CreateAction(string message, float forTime)
        {
            CreateAction(message, forTime, null, null);
        }
        public void CreateAction(string message, float delay, Action nAction,Action endAction)
        {
            if (actionPool.Count > 0)
            {
                //use pool
                actionPool[0].Setup(message, delay, nAction, endAction);
                actionQueue.Add(actionPool[0]);
                actionPool.Remove(actionPool[0]);
                return;
            }
            //if not enough in pool, expand pool size!
            actionQueue.Add(new ActionNode(message, delay, nAction, endAction));
        }
        public void EndTurn()
        {
            curTurn++;
            if (curTurn >= AllActors.Count)
            {
                if (decideActionsFirst)
                {
                    //require all player input again. start of a new battle phase!
                    //if you want to track battle phases, do it here!
                    FillMenuInputQueue();
                }

                curTurn = 0;
            }

            actorTurnLimitOnce = false;
        }
        public void ConsumeAction()
        {
            actionPool.Add(actionQueue[0]);
            actionQueue.Remove(actionQueue[0]);
        }
        bool allowUpdate = false;
        private void Update()
        {
            if (!allowUpdate) return;
            if(Test1)
            {
                Test1 = false;
                CreateAction("Test #1:" + curTurn, 1f, null,null);
            }
            if(Test2)
            {
                Test2 = false;
                enemyParty[0].stats=enemyParty[0].LerpStats(actorStatsData, enemyParty[0].name, (enemyParty[0].name + "*High"), test2Data);
            }
            if(RESET_BATTLE_TEST)
            {
                RESET_BATTLE_TEST = false;
              
                MockData();
                return;
            }
            switch (battleType)
            {
                case BattleType.TurnBased:
                    TurnBasedUpdate();
                    break;
                case BattleType.ATB:
                    break;
                case BattleType.MultiTurnSpeedBased:
                    break;
            }
        }
        public bool CheckForVictoryOrDefeat()
        {
            Debug.Log("Checking victory...");
            bool heroPartyDead = true;
            foreach(RPGActor a in heroParty)
            {
                if (a.stats.hp > 0)
                {
                    heroPartyDead = false;
                    break;
                }
            }
            if(heroPartyDead)
            {
                CreateAction(battleMessage_csv.GetCSVData("EnemyPartyWins"), 0.5f, null, () => EndBattle());
                //Defeat!
                return true;
            }
            bool enemyPartyDead = true;
            foreach (RPGActor a in enemyParty)
            {
                if (a.stats.hp > 0)
                {
                    enemyPartyDead = false;
                    break;
                }
            }
            if(enemyPartyDead)
            {
                CreateAction(battleMessage_csv.GetCSVData("HeroPartyWins"),0.5f,null, ()=>EndBattle());
                
                //Victory!
                return true;
            }
            return false;
        }
        //write a message that the player can see. Either YIIK style (Do this by writing damage as an action node), or Breath of Fire IV style (The Default).
        private void SetDisplayString(string input)
        {
            if (input == string.Empty) return;//not allowed to send empty message.


            //move old prompt to the old message display if availible
            if (previousPromptDisplay != null) previousPromptDisplay.SetText(battlePrompt);

            battlePrompt = input;
           if(promptDisplay!=null) promptDisplay.SetText(input);
            //a log of all commands. clear at start of battle
            battleLog.Add(input);
        }
        //create a new action that displays txt to the player
        public void WriteToPrompt(string input)
        {
            CreateAction(input, 1f, null,null);
        }
        bool actorTurnLimitOnce=false;


        //============================================================================
        //MENU INPUT:
        //. . . . . . 
       // int limitOnceMenuInputCheck = -1;
        public int debug_MENU_SIZE;
        bool limitOncePerMenu = false;
        bool nnnnnnnDebug = false;
        bool HasMenusThatNeedInputPlusUpdate()
        {
            debug_MENU_SIZE = menusInputQueueList.Count;
            if (requestGoBackToPreviousMenu)
            {
                DoGoBackToPreviousMenu();
                requestGoBackToPreviousMenu = false;
                limitOncePerMenu = false;

                //skip frame:
                return false;
            }

            //Sorry that this function also changes state... maybe I'll split it in to two, but it's fine for now, just keep it in mind
            //GET ALL INPUT AT ONCE!
            if (menusInputQueueList.Count > 0)
            {
               if(limitOncePerMenu==false)
                {
                    if(menusInputQueueList[0]==null)
                    {
                        //probably uses AI, remove it and move on...
                        menusInputQueueList.Remove(menusInputQueueList[0]);
                        return false;  
                    }
                    limitOncePerMenu = true;
                    menusInputQueueList[0].SetMenuIsActive(true);
                }
                if(menusInputQueueList[0].IsReadyToUseSkill())
                {
                    //successful input given, remove it from the list and get ready for the next one
                    menuHistoryQueue.Add(menusInputQueueList[0]);
                    menusInputQueueList.Remove(menusInputQueueList[0]);
                    limitOncePerMenu = false;
                    return false;
                }

                return false;
            }
            //only return true when you have no input left to give
            return true;
        }
        public bool GoBackToPreviousMenu()
        {
            if(decideActionsFirst==false)
            {
                //can't go back if you do actions per turn
                return false;
            }
            if(menuHistoryQueue.Count<=0)
            {
                //no history, play error sound in the menu code...
                return false;
            }
            //will be dealt with next tick
            requestGoBackToPreviousMenu = true;
            return true;
        }
        private void DoGoBackToPreviousMenu()
        {
            menusInputQueueList[0].Terminate(); // erease input and call terminate event to talk to graphical menu representation.
          
            menusInputQueueList.Insert(0,menuHistoryQueue[menuHistoryQueue.Count - 1]);
            menuHistoryQueue[menuHistoryQueue.Count - 1].Terminate();
            menuHistoryQueue.Remove(menuHistoryQueue[menuHistoryQueue.Count - 1]);

        }
        private bool requestGoBackToPreviousMenu = false;
        
        //...
        //==================================================================================================================


        void TurnBasedUpdate()
        {


            //---------------------------------------------------------------------------------------------------------
            //MENU INPUT
            //. . . . . finish action queue before starting this phase
            if(decideActionsFirst && actionQueue.Count==0)
            {
                if (HasMenusThatNeedInputPlusUpdate() == false)
                {
                    //returns true if no input left to give
                  
                  return;
                }
            }

            //---------------------------------------------------------------------------------------------------------
            /*
             * So what I want to do is do all of the player input first, and then go through all of the players turns, 
             * and then when you loop back to the begnning, get all input again.
             * 
             * maybe use a state machine for the turn based version.
             * 
             * or if it's in turn based mode just return until you have no more characters to give input to.
             * in atb mode, just keep the battle update running and have the menu pop open whenever the atb is ready.
             * 
             */
            //TODO: Define a battle update loop here.
            if (actionQueue.Count > 0)
            {
                //has actions to resolve:
                ActionAndMessageUpdate();
            }
            else
            {
                //TODO: Turn this in to a state machine!
                //Start of turn:
                if (actorTurnLimitOnce == false)
                {
                    //set false on end of turn...
                    actorTurnLimitOnce = true;
                    //only once at start of turn
                    AllActors[curTurn].StartOfTurn();
                   
                }
                //Constant Update
                else
                {
                    /*
                    * I don't like this. no need for an update, just call an function or something.
                    */
                    AllActors[curTurn].ActorUpdate();
                }
            }
        }
        void ActionAndMessageUpdate()
        {
       
            if (actionQueue[0].limitOnce == false)
            {
                //AT START OF TURN, DO THINGS
                SetDisplayString(actionQueue[0].message);
            }
            actionQueue[0].ActionUpdate();
            
        }
    }
    [System.Serializable]
    public class ActionNode
    {
        public float delay = 1f;
        public string message = "...";
        public Action doAction;
        public Action doActionOnEnd;
        public bool limitOnce = false;
        public ActionNode()
        {
            //default constructor
        }
        public ActionNode(string nMessage, float nDelay, Action nAction, Action endAction)
        {
            Setup(nMessage, nDelay, nAction,endAction);
        }
        public void Setup(string nMessage,float nDelay, Action nAction,Action endAction)
        {
            delay = nDelay;
            message = nMessage;
            doAction = nAction;
            doActionOnEnd = endAction;
            limitOnce = false;
        }
        public virtual void OnStartOfAction()
        {
            if (doAction != null) doAction();
        }
        public virtual void ActionUpdate()
        {
            //Not sure if I'll use delagates or have skills derrived from ActionNode. We'll see...
            if (!limitOnce)
            {
                OnStartOfAction();
                limitOnce = true;
            }
            //by default when they delay ends, the turn is over.
            //this could be replaced with when an animation ends, or delay could just be set to the time it takes for an animation to finish.
            delay -= RpgBattleSystemMain.instance.battleSpeed *Time.deltaTime;
            if(delay<=0)
            {
                ConsumeMessage();
            }
        }
        public void ConsumeMessage()
        {
            if(doActionOnEnd!=null) doActionOnEnd();
            RpgBattleSystemMain.instance.ConsumeAction();
        }
    }
    //Define Actor 
    [System.Serializable]
    public class RPGActor
    {
        /*
         * Acotrs should be things that are subscribed to by the health UI elements, and by the characte model that
         * needs to do the actual animation. Maybe a component like RPGActorWatcher should be made to interface with the unity objects.
         * 
         * ideally this system could be used to make a game that doesn't need a separate battle screen, or one that has a separate battle screen.
         * designers choice.
         */
         /*
          * TODO: Create a list of skills that the actor will hold. 
          * (load from stats page maybe? or make a seperate skills package definition?)
          * 
          * AI should anylize skills maybe and decide which to use. either that or  it should use a gambit sytem. not sure how those
          * should work together. maybe every skill comes pared with its own gambit and the AI reads that?
          * 
          * 
          * Add a Persona 5 Style battle menu as well as a traditional battle menu.
          * Change the array to a list in the simple menu class and you can probably build it using that. (Don't forget about object pooling here!
          * Try to never use the new keyword in this system. we don't need grabage collection ruining the framerate.
          * 
          */
        public string name; //name used internally, like in the save file
        public string displayName; //name displayed for current language
        public string alphabetPlacementAppend = string.Empty; //gets added for each duplicate of an enemy of that type. (Slime A, Slime B, ETC) (NOT IMPLEMENTED YET!)
        public StatsPage stats;
        public bool isInParty=true;
        public bool autoLevel = false;

        public RPG_AI ai= new RPG_AI();
        public bool useAI = false;

        public List<Skill> skills= new List<Skill>();
        public System.Action onHpChangedEvents;

        //---POSITIONAL INFO
        public Transform myTransform;
        public Vector3 targetLocation = Vector3.zero;
        //---
        //. . . . . . . . . . . . . . . . . . . . . . . 
        //ANIMATION HOOKS
        public Action animEventReadyWeapon;
        public Action animEventAttack;
        public Action animEventMagicAttack;

        public void AnimationReadyWeapon()
        {
            //example: pull out sword
            if (animEventReadyWeapon != null) animEventReadyWeapon();
        }
        public void AnimationAttack()
        {
            //example:  move to target and attack!
            //maybe each skill should send info to the actor so the animation manager can pick an animation and move to targets... no idea for now.
            //this function and the magic one may not be needed..
            if (animEventAttack != null) animEventAttack();
        }
        public void AnimationMagicAttack()
        {
            //example:  move to target and cast spell!
            if (animEventMagicAttack != null) animEventMagicAttack();
        }
        public void AnimationUnbindAll()
        {
            animEventReadyWeapon = null;
            animEventAttack = null;
            animEventMagicAttack= null;
        }
        //. . . . . . . . . . . . . . . . . . . . . . . 


        public void LoadSkills(IniGeneralUse iniStatsHolder)
        {
            //TODO clear skill list for now. in the future try not to remove items that are already created, and instead only add new names to the list to avoid garbage collection.
            //still it's not such a big deal for now...
          
            skills.Clear();
            string skillsAsString = string.Empty;
            skillsAsString = IniGameMemory.instance.GetDataString(name,"skills",string.Empty);
            
            if(skillsAsString == string.Empty)
            {
             
                //default to a single attack if no skills found. change it to some other skill. make it unique so that you know it's not supposed to be used, but
                //make it functional so that the game never breaks... (NOT DONE YET)
                skillsAsString=iniStatsHolder.GetDataString(name,"skills", "Hard Slash");
            }
            string[] skillNamesList = skillsAsString.Split(",".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

            foreach(string skillName in skillNamesList)
            {
                //TODO: Load skill from skill list!
                Skill nSkill=null;
                RpgBattleSystemMain.instance.skillsParserRealSkills.skillLookup.TryGetValue(skillName,out nSkill);
                if (nSkill == null) Debug.Log("Skill does not exist:" + skillName);
                if(nSkill!=null) skills.Add(nSkill);
            }
        }
        RPGMenuCommunication myMenuInterface;
        public void SetMenuController(RPGMenuCommunication menu)
        {
            myMenuInterface = menu;
        }
        public RPGMenuCommunication GetMenuInterface()
        {
            return myMenuInterface;
        }
        /// <summary>
        /// This will not damage the actor! it will only pre-calculate the damage. apply the damage when drawing the message about taking the damage!
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <returns></returns>
        public int DamageCalculation(int damageAmount)
        {
            int totalDamage = damageAmount -= (stats.GetDefence()/2);
            if (totalDamage<=0)
            {
                totalDamage = GameMath.GetRandomInt(0, 1);
            }
            preCalculateDamage = totalDamage;
            
            return totalDamage;
        }
        int preCalculateDamage = 0;
        /// <summary>
        /// Make damage real
        /// </summary>
        public void ApplyDamage()
        {
            stats.hp -= preCalculateDamage;
            if (stats.hp <= 0) stats.hp = 0;
            preCalculateDamage = 0;//set to zero for saftey
            if (onHpChangedEvents != null)
            {
                onHpChangedEvents();
            }
            CheckIfDeadAuto();
        }
        public bool IsDeadOrWillBeDead()
        {
            if (stats.hp-preCalculateDamage<=0)return true;
            return false;
        }
        /// <summary>
        /// Call this after damaging. The attack skill is responsible for this just so that messages are in the correct order and not tied to a weird timing mechanism.
        /// </summary>
        public void CheckIfDeadAuto()
        {
            if(IsDeadOrWillBeDead())
            {
                string deathMsg =String.Format (RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("Defeated"),displayName);
                RpgBattleSystemMain.instance.CreateAction(deathMsg);
                RpgBattleSystemMain.instance.CheckForVictoryOrDefeat();
            }
            //TODO: Play death animation here!
            //make hooks for all of these types of actions.
            //make a revive function as well.
        }
        public void LoadDiplayName()
        {
            //TODO get localized name!
            displayName = (name)+alphabetPlacementAppend;
        }
       
        public void StartOfTurn()
        {
           
            if (stats.hp<=0)
            {
                RpgBattleSystemMain.instance.EndTurn();
                return;
            }
            //TODO: Start of turn stuff.
            //proccess status aliments here.
            //add poison, stone, confusion, silence etc.
            //count down and remove ailments as needed.
            // pull open menu, or use AI;
            if(useAI)
            {
                ai.DoAction(this);
                return;
            }
            else if(RpgBattleSystemMain.instance.decideActionsFirst==false) //only do if you don't decide all actions at once
            {
                //only do this if you aren't confused, otherwise use AI, but set targets to hero party...
                //or maybe chose a random skill, chose a random target, and use it! even allow item consumption maybe.
                //give rare items a never use in confusion attribute or something if you don't want to be a dick (not made yet)
                // ai.DoAction(this); //USE AI FOR NOW UNTIL A HUMAN INTERFACE IS MADE!   <REMOVE THIS LINE TO ACCEPT INPUT FROM PLAYER INSTEAD>
                //Make it so that hero party members can also be set too AI! load that from save file.
                if (myMenuInterface == null)
                {
                    Debug.Log(name + " has no menu interface, and isn't using ai, ending turn!");
                    RpgBattleSystemMain.instance.WriteToPrompt(
                        string.Format(RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("DoesNothing"), displayName));
                    RpgBattleSystemMain.instance.EndTurn();
                    return;
                }
                myMenuInterface.SetMenuIsActive(true);
            }
        }

        public void ActorUpdate()
        {
            /*
            * I don't like this. no need for an update, just call an function or something.
            */


            if (myMenuInterface==null)
            {
                Debug.Log(name + " has no menu interface, and isn't using ai, ending turn!");
                RpgBattleSystemMain.instance.WriteToPrompt(
                    string.Format(RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("DoesNothing"),displayName));
                RpgBattleSystemMain.instance.EndTurn();
                return;
            }
            //TODO READ MENU INPUT RESULT HERE? Either that or send a command with the menu via some function
            if (myMenuInterface.IsReadyToUseSkill())
            {
                myMenuInterface.ExecuteAction();
                myMenuInterface.SetMenuIsActive(false);
                //clear the menu so you can choose again next turn.
                myMenuInterface.ClearAll();
                Debug.Log("READY");
            }
        }
        
        //Add a list of items that can be stolen to stats definition if it's an enemy.
        //if its the player, non- key items can be stolen. maybe even add an option for enemies
        //to steal equipment you are wearing!

        public void LoadStatsFromStatsFile(IniGeneralUse iniStatsHolder)
        {
            if(iniStatsHolder.GetDataBool(name,"autoLevel"))
            {
                autoLevel = true;
            }
           

            //TODO: Load the stats from the stats file and place here!
            //If you have 2 stats pages and a level, use leveling curve and level input to interpolate between values for auto leveling.
            //^Use LarpStats for this...
            int groupIndex = iniStatsHolder.GetGroupIndex(name);
            //lvl
            stats.lvl = iniStatsHolder.GetDataValue(groupIndex, "lvl", 1);
            if (autoLevel)
            {
                stats = CreateAutoLevelStats(iniStatsHolder, stats.lvl);
                return;
            }
  
            //hp
            stats.hpMax = iniStatsHolder.GetDataValue(groupIndex, "hp",1);
            stats.hp = stats.GetMaxHP();
            //mp
            stats.mpMax = iniStatsHolder.GetDataValue(groupIndex, "mp",1);
            stats.mp = stats.GetMaxMP();
            //and the rest:
            stats.str = iniStatsHolder.GetDataValue(groupIndex, "str", 1);
            stats.str += iniStatsHolder.GetDataValue(groupIndex, "atk", 1); //just in case I use atk instead of str...
            stats.def = iniStatsHolder.GetDataValue(groupIndex, "def", 1);
            stats.spd = iniStatsHolder.GetDataValue(groupIndex, "spd", 1);
            stats.wis = iniStatsHolder.GetDataValue(groupIndex, "wis", 1);
            stats.luk = iniStatsHolder.GetDataValue(groupIndex, "luk", 1);
            //this doesn't matter if it is an enemy:
            isInParty = iniStatsHolder.GetDataBool(groupIndex, "isInParty");
            //TODO: Add equipment list to definition, but no need to add that to the stats.
            //the GetAttack(), etc. functions will handle that.

        }
        public StatsPage CreateAutoLevelStats(IniGeneralUse iniStatsHolder,int myLevel)
        {
            if (myLevel <= 0) myLevel = 1;
            /*
             * The ideas is that if the player is on "myLevel" then it should take an average number of hits to kill this actor.
             * If the hero is stronger, then it should take even less. For a true auto-level system, set the enemy level to the players max level, but
             * that's not what this is for. It's designed to be an easier way to balance the game without needing to worry about specifics.
             * if you want a boss enemy, increase the hitsToKillMe
             * 
             * 
            [HeroParty.MaxLevelTarget]
	            note=this is a special definition that needs to be defined for auto leveling to work.
	            lvl=100
	            hp=99999
	            mp=99999
	            str=1850
	            def=1850
	            spd=128
	            luk=80
	            wis=1850
               
             * 
             * 
             
             * special stats needed:
             * 
                                   [Dragon]
	                                    autoLevel=true
	                                    hitsToKillMe=2
	                                    hitsToKillPlayer=8
	                                    assumePlayerPartySize=1
                                        speedMin=10
	                                    speedMax=30
	                                    luckMin=10
	                                    luckMax=30

             */
            StatsPage results = new StatsPage();
            const string heroTargetLevelPage = "HeroParty.MaxLevelTarget";
            const string heroStartingLevelPage = "HeroParty.MinLevelTarget";
            const string hitsToKillMe = "hitsToKillMe";
            const string hitsToKillPlayer = "hitsToKillPlayer";
            const string assumePlayerPartySize = "assumePlayerPartySize";
            const string speedMin = "speedMin";
            const string speedMax = "speedMax";
            const string luckMin = "luckMin";
            const string luckMax = "luckMax";


            StatsPage heroTargetMaxLevel = LookupStatsPage(iniStatsHolder, heroTargetLevelPage);
            StatsPage heroTargetStartLevel= LookupStatsPage(iniStatsHolder, heroStartingLevelPage);
            StatsPage approxHeroStatsAtMyLevel = LerpStats(heroTargetStartLevel, heroTargetMaxLevel, myLevel);

            int groupIndex= iniStatsHolder.GetGroupIndex(name);
            var rpgSys=RpgBattleSystemMain.instance;
           
            int heroPartyCount = iniStatsHolder.GetDataValue(groupIndex, assumePlayerPartySize, 1);
            if (heroPartyCount <= 0) heroPartyCount = 1;

            int hitsToKillMe_Value = iniStatsHolder.GetDataValue(groupIndex,hitsToKillMe,2);
            int hitsToKillPlayer_Value = iniStatsHolder.GetDataValue(groupIndex, hitsToKillPlayer, 2);
            int speedMin_Val= iniStatsHolder.GetDataValue(groupIndex, speedMin, 1);
            int speedMax_Val = iniStatsHolder.GetDataValue(groupIndex, speedMax, 15);

            int luckMin_Val = iniStatsHolder.GetDataValue(groupIndex, luckMin, 1);
            int luckMax_Val = iniStatsHolder.GetDataValue(groupIndex, luckMax, 60);

            //LVL
            results.lvl = myLevel;
            //HP CALC
            results.hpMax = hitsToKillMe_Value * (approxHeroStatsAtMyLevel.GetAttack()/ heroPartyCount);
            results.hp = results.GetMaxHP();
            
            //DEF
            results.def = 0; //Don't bother with defence stats!
            //ATTACK
            results.str =  approxHeroStatsAtMyLevel.GetMaxHP()/ hitsToKillPlayer_Value;
            //Debug.Log("hitsToKillPlayer:" + hitsToKillPlayer_Value+ " Divide val:"+ approxHeroStatsAtMyLevel.GetMaxHP() +" RESULT:"+results.str);
            //MAGIC
            results.wis = hitsToKillPlayer_Value / approxHeroStatsAtMyLevel.GetMaxHP();
            //SPEED
            results.spd = MergeStat(speedMin_Val, speedMax_Val, myLevel * 0.01f);
            //LUK
            results.spd = MergeStat(luckMin_Val, luckMax_Val, myLevel * 0.01f);


            return results;
        }
        public StatsPage LerpStats(IniGeneralUse iniStatsHolder, string statPageA,string statPageB, int myLevel)
        {
            StatsPage lowLevel= LookupStatsPage(iniStatsHolder, statPageA);
            StatsPage highLevel= LookupStatsPage(iniStatsHolder,statPageB);
            return LerpStats(lowLevel,highLevel, myLevel);
        }
        
        public StatsPage LerpStats(StatsPage lowLevel, StatsPage highLevel, int myLevel)
        {
            //based on a max 100 level, but with the ability to over level...

            float lerpVal = (myLevel-1) * 0.01f;
          
            StatsPage resultingStats = new StatsPage();
            resultingStats.lvl = myLevel;
            //TODO: Consider running this all through a curv.
            resultingStats.hpMax = MergeStat(lowLevel.hpMax,highLevel.hpMax,lerpVal);
            //Debug.Log("STAT MIN: " + lowLevel.hpMax + " : " + highLevel.hpMax+ "LERP: "+lerpVal+" is "+ resultingStats.hpMax.ToString());
            resultingStats.hp = resultingStats.GetMaxHP();
            resultingStats.str = MergeStat(lowLevel.str, highLevel.str, lerpVal);
            resultingStats.def = MergeStat(lowLevel.def, highLevel.def, lerpVal);
            resultingStats.spd = MergeStat(lowLevel.spd, highLevel.spd, lerpVal);
            resultingStats.wis = MergeStat(lowLevel.wis, highLevel.wis, lerpVal);
            resultingStats.luk = MergeStat(lowLevel.luk, highLevel.luk, lerpVal);

            return resultingStats;
        }


        int MergeStat(int a, int b, float power)
        {
            return Mathf.RoundToInt(Mathf.Lerp(a,b,power));
            //return Mathf.RoundToInt(GameMath.LerpUnclamped(a,b,power,true));
        }
        public StatsPage LookupStatsPage(IniGeneralUse iniStatsHolder, string statPageA)
        {
            StatsPage nStats = new StatsPage();
            int groupIndex = iniStatsHolder.GetGroupIndex(statPageA);
            //lvl
            nStats.lvl = iniStatsHolder.GetDataValue(groupIndex, "lvl", 1);
            //hp
            nStats.hpMax = iniStatsHolder.GetDataValue(groupIndex, "hp", 1);
            nStats.hp = stats.GetMaxHP();
            //mp
            nStats.mpMax = iniStatsHolder.GetDataValue(groupIndex, "mp", 1);
            nStats.mp = stats.GetMaxMP();
            //and the rest:
            nStats.str = iniStatsHolder.GetDataValue(groupIndex, "str", 1);
            nStats.str += iniStatsHolder.GetDataValue(groupIndex, "atk", 1); //just in case I use atk instead of str...
            nStats.def = iniStatsHolder.GetDataValue(groupIndex, "def", 1);
            nStats.spd = iniStatsHolder.GetDataValue(groupIndex, "spd", 1);
            nStats.wis = iniStatsHolder.GetDataValue(groupIndex, "wis", 1);
            nStats.luk = iniStatsHolder.GetDataValue(groupIndex, "luk", 1);
            //this doesn't matter if it is an enemy:
            return nStats;
        }

        public void LoadStatsFromSaveFile(IniGeneralUse fallbackIni)
        {
            //TODO: Consider wrapping the party stats group with a prefix for saving and loading...
            //Example:
            //[Party.Hero] instead of [Hero]


            //unlike the enemy stats, this uses the main save file.
            //be sure to write party stats to that file at the start of the game!
            //consider passing in the enemy stats page with default hero stats.
            //and reading from that in the event that the group index is not found (-1) it could
            //be an easy way to auto initialize things, and keep all actor definitions in a single file.
            if (IniGameMemory.instance==null)
            {
                LoadStatsFromStatsFile(fallbackIni);
                Debug.LogError("IniGameMemory OBJECT MEMORY NOT FOUND");
                return;
            }
            int groupIndex = INI.Get().GetGroupIndex(name);
            if(groupIndex==-1)
            {
                LoadStatsFromStatsFile(fallbackIni);
                Debug.LogWarning(name+"<- GROUP NOT FOUND, LOADIND DEFAULT FROM STATS DEFINITION");
                return;
            }
            //lvl
            stats.lvl = INI.Get().GetDataValue(groupIndex, "lvl", 1);
            //hp
            stats.hpMax = INI.Get().GetDataValue(groupIndex, "hp", 1);
            stats.hp = INI.Get().GetDataValue(groupIndex, "currentHp", stats.GetMaxHP());
            //mp
            stats.mpMax = INI.Get().GetDataValue(groupIndex, "mp", 1);
            stats.mp = stats.GetMaxMP();
            //and the rest:
            stats.str = INI.Get().GetDataValue(groupIndex, "str", 1);
            stats.str += INI.Get().GetDataValue(groupIndex, "atk", 1);  //just in case I use atk instead of str... old habits
            stats.def = INI.Get().GetDataValue(groupIndex, "def", 1);
            stats.spd = INI.Get().GetDataValue(groupIndex, "spd", 1);
            stats.wis = INI.Get().GetDataValue(groupIndex, "wis", 1);
            stats.luk = INI.Get().GetDataValue(groupIndex, "luk", 1);

            isInParty = INI.Get().GetDataBool(groupIndex, "isInParty");
        }
        public void WriteStatsToIniFile()
        {
            //TODO: Consider wrapping the party stats group with a prefix for saving and loading...
            //Example:
            //[Party.Hero] instead of [Hero]


        }

    }
    [System.Serializable]
    public class StatsPage
    {
        public int lvl;
        //hit points
        public int hpMax=1;
        public int hp=1;
        //magic points
        public int mpMax=1;
        public int mp=1;
        //strength is added to the weapons power.
        public int str = 1;
        //defence is added to the armours defence.
        public int def = 1;
        //speed. faster characters attack first.
        public int spd=1;
        //wisdom. basically magic attack power.
        public int wis=1;
        //luck random chance of critical hits. higher number = more crits!
        public int luk=1;



        public int GetAttack()
        {
            //TODO: combine with stats from equipment!
            return str;
        }
        public int GetDefence()
        {
            //TODO: combine with stats from equipment!
            return def;
        }
        public int GetMaxHP()
        {
            //TODO: combine with stats from equipment!
            return hpMax;
        }
        public int GetMaxMP()
        {
            //TODO: combine with stats from equipment!
            return mpMax;
        }
        public int GetLuck()
        {
            //TODO: combine with stats from equipment!
            return luk;
        }
    }
    [System.Serializable]
    public class RPG_AI
    {
        [NonSerialized]
        private List<RPGActor> targets= new List<RPGActor>();
        [NonSerialized]
        private Skill attack = new AttackActionStd();

        public void SetTargets(List<RPGActor> nTargets)
        {
            targets=nTargets;
        }
        public void DoAction(RPGActor myActor)
        {
            //TODO: Select a target(s) via gambit conditions
            //TODO: Do action based on AI gambit settings.
            //create test action!
            /*  RpgBattleSystemMain.instance.CreatAction(myActor.Name+" attacks!", 1f,null,null);
              Action endTurn = () => RpgBattleSystemMain.instance.EndTurn();
              RpgBattleSystemMain.instance.CreatAction(myActor.Name + " does " +myActor.stats.GetAttack() + " DMG!",1f,null, endTurn);
              */
            //Really what should happen is that the skill should be pushed to the queue, but I don't know if that can work with the pool.
            //maybe the ActionNode should just be able to hold a skill and a skill can be its own thing.
            //so for now I'll just simulate it and I'll probably make AttackActionStd NOT inherit ActionNode...
            attack.SetMyActor(myActor);
            attack.SetTargets(targets);
            attack.OnStartOfAction();
        }
   
    }
    [System.Serializable]
    public class Skill
    {
        /// <summary>
        /// Skill name will be localized. use actualInternalName to get the group name! (which is not the same as the overriden name as of now, and may never be)
        /// </summary>
        public string skillName;
        public string actualInternalName;
        [NonSerialized]
        public RPGActor myActor;
        [NonSerialized]
        public  List<RPGActor>targets;
        public Affinity affinity = new Affinity();
        //SKILL not sure if SKILL will have any base...
        public virtual void SetMyActor(RPGActor nActor)
        {
            myActor = nActor;
        }
        public virtual void SetTargets(List<RPGActor>nTargets)
        {
            targets = nTargets;
        }
        public virtual void OnStartOfAction()
        {
            
        }
        public void CaclulateTargetLocation()
        {
            int numberOfTransforms = 0;
            Vector3 averagePosition = Vector3.zero;
            for (int i = 0; i < targets.Count; i++)
            {
                RPGActor actor = (RPGActor)targets[i];
                if (actor.myTransform!=null)
                {
                    if (i == 0) averagePosition = actor.myTransform.position;
                    averagePosition += actor.myTransform.position;
                    numberOfTransforms++;
                }
            }
            if(averagePosition==Vector3.zero)
            {
                //avoid NAN
                averagePosition = myActor.myTransform.position;
                return;
            }
            averagePosition = averagePosition / numberOfTransforms;
            myActor.targetLocation = averagePosition;
        }
        string LocalizeSkillName(string stringToLocalize)
        {
            //if you want a list of skill names, and their equivilent in another language use the csv parser, and get the desired language and return results here.
            //this is not implimented yet, but when you need it it'll be here...
            return stringToLocalize;
        }
        public virtual void ParseFromGroup(GroupData createFromGroup)
        {
            skillName = LocalizeSkillName(createFromGroup.GetValueIgnoreCase("nameOverride", createFromGroup.name));
            actualInternalName = createFromGroup.name; //never localized
            string elementalAttribute = createFromGroup.GetValueIgnoreCase( "affinity", "none");
            this.affinity.SetupMap();
            this.affinity.SetMyAffinity(elementalAttribute);
        }

    }
    [System.Serializable]
    public class AttackActionStd : Skill
    {
        int attackPower = 1;
        int multiplier = 1;
        int wisdom = 1;
        public Action DoOnStartOfAction;
        public Action DoEndOfAction;
        public bool isPhysicalAttack = true;
        public bool useVerboseMessage = false;
        public AttackActionStd()
        {

        }
        public override void ParseFromGroup(GroupData createFromGroup)
        {
            //do standard things first
            base.ParseFromGroup(createFromGroup);
            attackPower = createFromGroup.GetValueIgnoreCase("atk",1);
            multiplier = createFromGroup.GetValueIgnoreCase("multiplier", 1);
            wisdom = createFromGroup.GetValueIgnoreCase("wisdom", 1);
            //default to physical attack, else use a magic attack
            isPhysicalAttack = createFromGroup.GetBoolIgnoreCase("isPhysicalAttack", true);

        }
        public AttackActionStd(string nName)
        {
            skillName = nName;
        }

        /*      Subscribe to skills like this:
            DoOnStartOfAction += doAttackAnimation;
         */


        /*EXAMPLE:
         * [Hard Strike]
	        type=physical
	        attribute=none
	        str=1
	        multiplier=1
	        animation=attack
         */
        public override void OnStartOfAction()
        {
            //Use attack function if the type is physical (default)
            if (isPhysicalAttack)
            {
                AttackOrMagic(attackPower);
            }
            else
            {
                AttackOrMagic(wisdom);
            }
        }
        void AttackOrMagic(int attackOrWisdomStat)
        {
            if(DoOnStartOfAction!=null)DoOnStartOfAction();
            //STRINGS:
            //{hero} attacks! message
            string attacksMessageLight = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("AttacksMsg");
            string attacksMessageVerbose = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("AttacksMsgDetailed");
            string msg = string.Empty;
            //{Slime} takes, {25} DMG!
            string normalHit = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("DamageMsg");
            string criticalHit = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("CriticalDamageMsg");
            //
            //Setup for end of turn:
            Action endTurn = () => RpgBattleSystemMain.instance.EndTurn();
            if(DoEndOfAction!=null) endTurn += DoEndOfAction; //do end turn and any animation cleanup or anything like that here.

            //get location of the enemy and remember it for animation purposes...
            CaclulateTargetLocation();
            //----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----

            bool limitFirstMessageUse = false;
            foreach (RPGActor target in targets)
            {
                //_______________________________________________________________________________________________

                if (target.IsDeadOrWillBeDead()) continue;//skip dead actors!
                //_______________________________________________________________________________________________
                if(useVerboseMessage)
                {
                    msg = String.Format(attacksMessageVerbose, myActor.displayName, target.displayName,skillName);
                    RpgBattleSystemMain.instance.CreateAction(msg, 1f, null, () => myActor.AnimationAttack());
                    myActor.AnimationReadyWeapon();
                }
                else
                {
                    //alternativly or additionally, just display the skill name (not how it works as of now 06/05/20)
                    if (limitFirstMessageUse == false)
                    {
                        msg = String.Format(attacksMessageLight, myActor.displayName);
                        RpgBattleSystemMain.instance.CreateAction(msg, 1f, null, ()=>myActor.AnimationAttack());
                        myActor.AnimationReadyWeapon();
                        limitFirstMessageUse = true;
                    }
                }
                //_______________________________________________________________________________________________


                int attackPower = (myActor.stats.GetAttack() + attackOrWisdomStat) * multiplier;
                //Critical attack!
                bool useCritical = false;
                if (GameMath.GetRandomInt(0, 100)<= myActor.stats.GetLuck())
                {
                    useCritical = true;
                }
                //just a bit more random power
                if (GameMath.GetRandomInt(0, 100)<50)
                {
                    attackPower += GameMath.GetRandomInt(0, 5);
                }
                
                //TODO: Calculate criticals here! if critical, use a crit message instead!
                int damageResult = target.DamageCalculation(attackPower);

                // Calc attack power and damage here. if you want shields to give a chance of deflecting, put that here!
                if (useCritical)
                {
                    msg = String.Format(criticalHit, target.displayName, damageResult.ToString());
                }
                else
                {
                    msg = String.Format(normalHit, target.displayName, damageResult.ToString());

                }
                //send message, and apply damage!
                RpgBattleSystemMain.instance.CreateAction(msg, 1f, null, ()=>target.ApplyDamage());
                //Deal with death here!
                //I'm only doing this in the skill code so the message is in the correct order.
                
                //
            }
            //----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----
            //END THE TURN:
            RpgBattleSystemMain.instance.CreateAction(string.Empty, 0f, null, endTurn);
            //END THE TURN ^
        }
        public void SkillsFromGroup(string group)
        {
            var skillParse = RpgBattleSystemMain.instance.skillData;
            int groupId = skillParse.GetGroupIndex(group);
            attackPower = skillParse.GetDataValue(groupId, "str", 1);
            multiplier = skillParse.GetDataValue(groupId, "multiplier", 1);
            wisdom = skillParse.GetDataValue(groupId, "wis", 1);
        }
    }
}
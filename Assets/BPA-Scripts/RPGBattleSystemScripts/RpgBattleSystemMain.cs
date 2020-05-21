using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ackk.INI.Helpers;


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
        public bool RESET_BATTLE_TEST;
        [Header("Setup")]
        public StringWriter promptDisplay;
        public StringWriter previousPromptDisplay;
        [Header("Game Data")]
        public TextAsset EnemyStatDefinitions;
        public IniGeneralUse enemyStatsData;
        public TextAsset battleMessages;
        public TextAsset skillDefinitions;
        public IniGeneralUse skillData;

        public CommaSeperatedValueParser battleMessage_csv= new CommaSeperatedValueParser();

        public static RpgBattleSystemMain instance;
        public enum BattleType { TurnBased, ATB, MultiTurnSpeedBased };
        [Header("Settings")]
        public BattleType battleType = BattleType.TurnBased;
        [Header("Current Data")]
        public int curTurn = 0;

        public List<RPGActor> heroParty = new List<RPGActor>();
        public List<RPGActor> enemyParty = new List<RPGActor>();

        public List<RPGActor> AllActors = new List<RPGActor>();
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
            enemyStatsData.MemoryFromIniString(EnemyStatDefinitions.text);
            battleMessage_csv.Parse(battleMessages.text);
            skillData.MemoryFromIniString(skillDefinitions.text);

            //initialize default hero party. will be overriden by the party menu probably.
            //this is really only being calleed here right now as a test. remove later:
            MockData();
        }
        void MockData()
        {
            //initialize default hero party. will be overriden by the party menu probably.
            //this is really only being calleed here right now as a test. remove later:
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
                a.LoadStatsFromSaveFile(enemyStatsData);
            }
        }
        //Call before start battle!
        public void SetEnemyParty(List<RPGActor> setEnemyParty)
        {
            enemyParty = setEnemyParty;
            foreach(var a in enemyParty)
            {
                a.useAI=true; //enemies use AI.
                a.LoadStatsFromStatsFile(enemyStatsData);
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
        public void CreatAction(string message)
        {
            CreatAction(message,  1f);
        }
        public void CreatAction(string message, float forTime)
        {
            CreatAction(message, forTime, null, null);
        }
        public void CreatAction(string message, float delay, Action nAction,Action endAction)
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
            if (curTurn >= AllActors.Count) curTurn = 0;
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
                CreatAction("Test #1:" + curTurn, 1f, null,null);
            }
            if(RESET_BATTLE_TEST)
            {
                RESET_BATTLE_TEST = false;
              
                MockData();
                StartBattle();
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
        public void CheckForVictoryOrDefeat()
        {
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
                CreatAction(battleMessage_csv.GetCSVData("EnemyPartyWins"));
                //Defeat!

                return;
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
                CreatAction(battleMessage_csv.GetCSVData("HeroPartyWins"));
                //Victory!
                return;
            }
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
            CreatAction(input, 1f, null,null);
        }
        bool actorTurnLimitOnce=false;
        void TurnBasedUpdate()
        {
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
            delay -= Time.deltaTime;
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
        public StatsPage stats;
        public bool isInParty=true;

        public RPG_AI ai= new RPG_AI();
        public bool useAI = false;

        public int Damage(int damageAmount)
        {
            int totalDamage = damageAmount -= (stats.GetDefence()/2);
            stats.hp -= totalDamage;
            if(totalDamage<=0)
            {
                totalDamage = GameMath.GetRandomInt(0, 1);
            }
            if (stats.hp <= 0) stats.hp = 0;
            return totalDamage;
        }
        /// <summary>
        /// Call this after damaging. The attack skill is responsible for this just so that messages are in the correct order and not tied to a weird timing mechanism.
        /// </summary>
        public void CheckIfDeadAuto()
        {
            if(stats.hp<=0)
            {
                string deathMsg =String.Format (RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("Defeated"),displayName);
                RpgBattleSystemMain.instance.CreatAction(deathMsg);
                RpgBattleSystemMain.instance.CheckForVictoryOrDefeat();
            }
            //TODO: Play death animation here!
            //make hooks for all of these types of actions.
            //make a revive function as well.
        }
        public void LoadDiplayName()
        {
            //get localized name!
            displayName = name;
        }

        public void StartOfTurn()
        {
            if(stats.hp<=0)
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
            }
            else
            {
                //only do this if you aren't confused, otherwise use AI, but set targets to hero party...
                //or maybe chose a random skill, chose a random target, and use it! even allow item consumption maybe.
                //give rare items a never use in confusion attribute or something if you don't want to be a dick (not made yet)
                 ai.DoAction(this); //USE AI FOR NOW UNTIL A HUMAN INTERFACE IS MADE!   <REMOVE THIS LINE TO ACCEPT INPUT FROM PLAYER INSTEAD>
                //Make it so that hero party members can also be set too AI! load that from save file.
               
            }
        }
        public void ActorUpdate()
        {
            //TODO READ MENU INPUT RESULT HERE? Either that or send a command with the menu via some function
        }
        
        //Add a list of items that can be stolen to stats definition if it's an enemy.
        //if its the player, non- key items can be stolen. maybe even add an option for enemies
        //to steal equipment you are wearing!

        public void LoadStatsFromStatsFile(IniGeneralUse iniStatsHolder)
        {
            //TODO: Load the stats from the stats file and place here!
            //If you have 2 stats pages and a level, use leveling curve and level input to interpolate between values for auto leveling.
            int groupIndex = iniStatsHolder.GetGroupIndex(name);
            //lvl
            stats.lvl = iniStatsHolder.GetDataValue(groupIndex, "lvl", 1);
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
        public void LoadStatsFromSaveFile(IniGeneralUse fallbackIni)
        {
            //unlike the enemy stats, this uses the main save file.
            //be sure to write party stats to that file at the start of the game!
            //consider passing in the enemy stats page with default hero stats.
            //and reading from that in the event that the group index is not found (-1) it could
            //be an easy way to auto initialize things, and keep all actor definitions in a single file.
            if(IniGameMemory.instance==null)
            {
                LoadStatsFromStatsFile(fallbackIni);
                return;
            }
            int groupIndex = INI.Get().GetGroupIndex(name);
            if(groupIndex==-1)
            {
                LoadStatsFromStatsFile(fallbackIni);
                return;
            }
            //lvl
            stats.lvl = INI.Get().GetDataValue(groupIndex, "lvl", 1);
            //hp
            stats.hpMax = INI.Get().GetDataValue(groupIndex, "hp", 1);
            stats.hp = stats.GetMaxHP();
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
    public class Skill
    {
        [HideInInspector]
        public RPGActor myActor;
        [HideInInspector]
        public  List<RPGActor>targets;
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


    }
    public class AttackActionStd : Skill
    {
        int attackPower = 1;
        int multiplier = 1;
        int wisdom = 1;
        public Action DoOnStartOfAction;
        public Action DoEndOfAction;
        public bool isPhysicalAttack = true;

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
            string getMessage = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("AttacksMsg");
            string msg = String.Format(getMessage, myActor.displayName, targets[0].displayName);
            //{Slime} takes, {25} DMG!
            string normalHit = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("DamageMsg");
            string criticalHit = RpgBattleSystemMain.instance.battleMessage_csv.GetCSVData("CriticalDamageMsg");
            //
            //Setup for end of turn:
            Action endTurn = () => RpgBattleSystemMain.instance.EndTurn();
            if(DoEndOfAction!=null) endTurn += DoEndOfAction; //do end turn and any animation cleanup or anything like that here.

            //----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----

            foreach (RPGActor target in targets)
            {
                if (target.stats.hp <= 0) continue;//skip dead actors!

                RpgBattleSystemMain.instance.CreatAction(msg, 1f, null, null);

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
                int damageResult = target.Damage(attackPower);

                // Calc attack power and damage here. if you want shields to give a chance of deflecting, put that here!
                if (useCritical)
                {
                    msg = String.Format(criticalHit, target.displayName, damageResult.ToString());

                }
                else
                {
                    msg = String.Format(normalHit, target.displayName, damageResult.ToString());

                }
                RpgBattleSystemMain.instance.CreatAction(msg, 1f, null, null);
                //Deal with death here!
                //I'm only doing this in the skill code so the message is in the correct order.
                target.CheckIfDeadAuto();
                //
            }
            //----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----//----
            //END THE TURN:
            RpgBattleSystemMain.instance.CreatAction(string.Empty, 0f, null, endTurn);
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
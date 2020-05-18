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
        [Header("Game Data")]
        public TextAsset EnemyStatDefinitions;
        public IniGeneralUse enemyStatsData;

        public static RpgBattleSystemMain instance;
        public enum BattleType { TurnBased, ATB, MultiTurnSpeedBased };
        [Header("Settings")]
        public BattleType battleType = BattleType.TurnBased;
        [Header("Current Data")]
        public List<RPGActor> heroParty = new List<RPGActor>();
        public List<RPGActor> enemyParty = new List<RPGActor>();
        public List<RPGActor> AllActors = new List<RPGActor>();
        public int curTurn = 0;
        string battlePrompt;
        //ACTIONS TO PERFORM
        public List<ActionNode> actionQueue = new List<ActionNode>();
        //maybe re-use actions so you don't need to use the New keyword!
        public List<ActionNode> actionPool = new List<ActionNode>();

        private void Awake()
        {
            instance = this;
            enemyStatsData.MemoryFromIniString(EnemyStatDefinitions.text);
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
                a.LoadStatsFromStatsFile(enemyStatsData);
            }
        }
        public void StartBattle()
        {
            curTurn = 0;
            //add all to the acting actors list:
            AllActors.AddRange(enemyParty);
            foreach(RPGActor a in heroParty)
            {
                if (a.isInParty) AllActors.Add(a);
            }
        }
        void SetUpBattleActors()
        {
            //load in the graphics for players and enemis and place them on the field.
            //populate target menus
            //TODO: 
        }
        public void CreatAction(string message, float delay, Action nAction)
        {
            if (actionPool.Count > 0)
            {
                //use pool
                actionPool[0].Setup(message, delay, nAction);
                actionQueue.Add(actionPool[0]);
                actionPool.Remove(actionPool[0]);
                return;
            }
            //if not enough in pool, expand pool size!
            actionQueue.Add(new ActionNode(message, delay, nAction));
        }
        public void TurnEnd()
        {
            curTurn++;
            if (curTurn > AllActors.Count) curTurn = 0;
        }
        public void ConsumeAction()
        {
            actionPool.Add(actionQueue[0]);
            actionQueue.Remove(actionQueue[0]);
        }
        private void Update()
        {
            if(Test1)
            {
                Test1 = false;
                CreatAction("Test #1:" + curTurn, 1f, ()=> curTurn++);
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
        void TurnBasedUpdate()
        {
            //TODO: Define a battle update loop here.
            if (actionQueue.Count > 0)
            {
                //has actions to resolve
                ActionAndMessageUpdate();
            }
            else
            {

            }
        }
        void ActionAndMessageUpdate()
        {
       
            if (actionQueue[0].limitOnce == false)
            {

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
        public bool limitOnce = false;
        public ActionNode()
        {
            //default constructor
        }
        public ActionNode(string nMessage, float nDelay, Action nAction)
        {
            Setup(nMessage, nDelay, nAction);
        }
        public void Setup(string nMessage,float nDelay, Action nAction)
        {
            delay = nDelay;
            message = nMessage;
            doAction = nAction;
            limitOnce = false;
        }
        public virtual void ActionUpdate()
        {
            //Not sure if I'll use delagates or have skills derrived from ActionNode. We'll see...
            if (!limitOnce)
            {
               if(doAction!=null) doAction();
                limitOnce = true;
            }
            //by default when they delay ends, the turn is over.
            //this could be replaced with when an animation ends, or delay could just be set to the time it takes for an animation to finish.
            delay -= Time.deltaTime;
            if(delay<=0)
            {
                EndAction();
            }
        }
        public void EndAction()
        {
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
        public string Name; //name used internally, like in the save file
        public string displayName; //name displayed for current language
        public StatsPage stats;
        public bool isInParty=true;
        //Add a list of items that can be stolen to stats definition if it's an enemy.
        //if its the player, non- key items can be stolen. maybe even add an option for enemies
        //to steal equipment you are wearing!

        public void LoadStatsFromStatsFile(IniGeneralUse iniStatsHolder)
        {
            //TODO: Load the stats from the stats file and place here!
            //If you have 2 stats pages and a level, use leveling curve and level input to interpolate between values for auto leveling.
            int groupIndex = iniStatsHolder.GetGroupIndex(Name);
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
            int groupIndex = INI.Get().GetGroupIndex(Name);
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

}
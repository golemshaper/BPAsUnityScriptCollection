using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.BPA;
namespace RPG.BPA.MENU
{
    public class RPGMenuCommunication
    {

        RPGActor myActor;
        List<Skill> availibleActions = new List<Skill>();
        public List<RPGActor> enemyTargets = new List<RPGActor>();
        public List<RPGActor> heroTargets = new List<RPGActor>();
        public List<RPGActor> availibleTargets = new List<RPGActor>();

        public Skill skillToPerform = null;
        public List<RPGActor> targetsToHit = new List<RPGActor>();
        public System.Action onTerminate;
        bool menuActive = false;
        public void SetMenuIsActive(bool status)
        {
            menuActive = status;
        }
        public bool GetMenuIsActive()
        {
            return menuActive;
        }
        public void Terminate()
        {
            //called when you need to go back in history and terminate the menu early.
            //subscribe to onTerminate to clear any menu drawing that you need to clear.
            targetsToHit.Clear();
            skillToPerform = null;

            SetMenuIsActive(false);
            if(onTerminate!=null)onTerminate();
        }
        public List<Skill> GetSkillList()
        {
            return availibleActions;
        }
        public List<string> GetSkillListAsStringList()
        {
   
            List<string> result = new List<string>();
            foreach(Skill s in availibleActions)
            {
                result.Add(s.skillName);
            }
            return result;
        }
        public List<string> GetAttackTargetListAsStringList(Skill selectedSkill)
        {
            CreateAttackTargetsList(false);
            //~~
            //TODO: assign the target type to availibleAc tions!
            //~~
            List<string> result = new List<string>();
            foreach (RPGActor s in availibleTargets)
            {
                result.Add(s.displayName);
            }
            return result;
        }
        public RPGActor GetMyActor()
        {
            return myActor;
        }
        //Call to obtain actor from name
        public void Initialize(string myActorName)
        {
            Initialize(RpgBattleSystemMain.instance.GetHeroIndex(myActorName));
        }
        public void Initialize(int index)
        {
            myActor = RpgBattleSystemMain.instance.heroParty[index];
            myActor.SetMenuController(this);
            enemyTargets = CloneList(RpgBattleSystemMain.instance.enemyParty);
            CreateAttackTargetsList(false);
            //skills you can perform
            availibleActions = myActor.skills;
            if (availibleActions.Count<=0)
            {
                availibleActions.Add(new AttackActionStd("Debug Attack"));
            }
        }
        public List<RPGActor> CloneList(List<RPGActor> listToClone)
        {
            List<RPGActor> nList = new List<RPGActor>();
            foreach(var e in listToClone)
            {
                nList.Add(e);
            }
            return nList;
        }
        public void CreateAttackTargetsList(bool includeDeadActors)
        {
            if (myActor.isInParty)
            {
                //is a hero?? you may need overrides for guests, etc.
                availibleTargets = enemyTargets;
            }
            else
            {
                availibleTargets = enemyTargets;
            }
            if(includeDeadActors)
            {
                //no need to prune dead actors
                return;
            }
            //prune dead actors...
            foreach(RPGActor a in availibleTargets.ToArray())
            {
                if(a.IsDeadOrWillBeDead())
                {
                    availibleTargets.Remove(a);
                }
            }
        }
        public void PullAvailibleActions()
        {
            availibleActions = myActor.skills;
        }
        public void SetTargets(List<RPGActor> nTargets)
        {
            targetsToHit = nTargets;
        }
        public void SetTargets(RPGActor nTargets)
        {
            var singleTarget = new List<RPGActor>();
            singleTarget.Add(nTargets);
            targetsToHit = singleTarget;
        }
        public void SetSkill(Skill nSkill)
        {
            skillToPerform = nSkill;
        }
        /// <summary>
        /// True if ready to attack/heal/whatever
        /// </summary>
        /// <returns></returns>
        public bool IsReadyToUseSkill()
        {
            if (targetsToHit == null) return false;
            if(targetsToHit.Count>0)
            {
                if(skillToPerform!=null)
                {
                    return true;
                }
            }
            return false;
        }
        public void ExecuteAction()
        {
            skillToPerform.SetTargets(targetsToHit);
            skillToPerform.myActor = GetMyActor();
            skillToPerform.OnStartOfAction();
            //clean up
            targetsToHit.Clear();
            skillToPerform = null;
        }
        /// <summary>
        /// Call after use!
        /// </summary>
        public void ClearAll()
        {

            skillToPerform = null;
            //do not clear targets, just override them..
        }
    }
}

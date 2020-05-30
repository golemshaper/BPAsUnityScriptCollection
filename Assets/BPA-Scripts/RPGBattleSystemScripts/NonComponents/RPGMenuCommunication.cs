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

        public Skill skillToPerform = null;
        public List<RPGActor> targetsToHit = new List<RPGActor>();

        bool menuActive = false;
        public void SetMenuIsActive(bool status)
        {
            menuActive = status;
        }
        public bool GetMenuIsActive()
        {
            return menuActive;
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
            enemyTargets = RpgBattleSystemMain.instance.enemyParty;
            availibleActions = myActor.skills;
            if (availibleActions.Count<=0)
            {
                availibleActions.Add(new AttackActionStd("Debug Attack"));
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

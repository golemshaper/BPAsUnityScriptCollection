using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ackk.INI.Helpers;

namespace RPG.BPA
{
    [System.Serializable]
    public class RPGSkillParser
    {
        /*
         * TODO: Read in skills from a file and create a list of all skills
         * add skills to a dictionary for easy lookup and access.
         * 
         */
        private IniGeneralUse fileData= new IniGeneralUse();
        public List<Skill> skills = new List<Skill>();
        public Dictionary<string, Skill> skillLookup = new Dictionary<string, Skill>();
        public void ParseSkillsFromData(string data)
        {
            fileData.MemoryFromIniString(data);
            BuildSkillsList();
      
        }
        /*
         * REQUIRED DATA:
            [SkillName]
                Type=Attack
                Affinity=Fire

         * 
         */
        void BuildSkillsList()
        {
            //Input all lowercase for now.
            skills.Clear();
            for(int i=0; i< fileData.groups.Count; i++)
            {
           
                string type = fileData.GetDataValue(i, "type","attack");
                Skill nSkill = CreateSkillOfType(type);
                //Send data to new skill:
                nSkill.ParseFromGroup(fileData.groups[i]);
                //each skill class takes care of its own attribute needs
                //add skill to list
                skills.Add(nSkill);
                //add skill to dictionary
                skillLookup.Add(nSkill.skillName, nSkill);
            }
        }
        public Skill CreateSkillOfType(string type)
        {
            //TODO: Do this a better way:
            if(type.ToLower()=="attack")
            {
                return new AttackActionStd();
            }
            return new Skill();
        }
    }
}
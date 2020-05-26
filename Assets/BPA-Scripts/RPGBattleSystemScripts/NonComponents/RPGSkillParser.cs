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
                //SKILL NAME
                nSkill.skillName = fileData.GetDataString(fileData.groups[i].name, "nameOverride", fileData.groups[i].name);
                //AFFINITY:
                string elementalAttribute = fileData.GetDataValue(i, "affinity","none");
                nSkill.affinity.SetupMap();
                nSkill.affinity.SetMyAffinity(elementalAttribute) ; //TODO: Test this to make sure it actually works
                //TODO: Add attack power and wisdom power to skill.
                //Add other class types besides "attack" and "none"
                //make a lookup that can ignore case. maybe make that an option for the INI file.
                /*
                 * 
                 * UPDATED IDEA:
                 Each skill should build itself off of thest the ini group passed to it.
                 that way each skill can have custom settings based on its class type.
                 */


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
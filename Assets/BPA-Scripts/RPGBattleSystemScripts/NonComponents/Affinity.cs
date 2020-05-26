using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Elements
{
    [System.Serializable]
    public class Affinity
    {
        /*
         * 
         * The Affinity class should be used by the TurnLoopSystem.cs class
         * call SetupMap to set up affinity, and add functions here to test damage.
         * damage should probably be done via the DoAttack class or in the Actor Damage() function. not sure which yet...
         * 
         * CharacterAffinity should be an object of Actor.cs class
         * 
         * Alternativly, make the affinity a member of Actor instead, and just have duplicate copys of the Dictionary that defines weaknesses and cut 
         * out the TurnLoopSystem alltogether...
         * 
         * 
         * Affinity results go to the attacker
         * Normal: The affinity's do not effect one another.
         * Weak: The attacking affinity is weak to the opposing affinity so damage is halved.
         * Strong The attacking affinity is stronger the the opposing affinity, so damage is doubled
         

            Fire beats metal (heat)  (
            Metal beats wood (saw) --
            Wood beats earth (trees grow in earth)
            Earth beats water (absorbs) 
            Water beats fire (extinguish)
            --
            Fire is weak to earth (dirt puts out fire)
            earth is week to metal (shovel)
            metal is weak to water (rust)
            water is weak to wood (absorb again??)
            wood weak to fire (bruns)

         */
        public enum AffinityResults
        {
            Normal,Weak,Strong
        };
        public enum AffinityType
        {
            //Fire,Metal,Wood,Earth,Water,None,Ignore (IGNORE IS NOT A REAL AFFINITY! IT JUST MEANS NO WEAKNESS OR STRENGTH! USE NONE IF YOU WANT THIS)
            None,Fire, Metal,Wood,Earth,Water,Ignore
        };
        public void SetMyAffinity(string affinityString)
        {
            myAffinity = (AffinityType)System.Enum.Parse(typeof(AffinityType), affinityString);
        }
        Dictionary<AffinityType, AfinityMiniTable> affinityMaps = new Dictionary<AffinityType, AfinityMiniTable>();
        public AffinityType myAffinity = AffinityType.None;
        public AffinityResults GetAffinityResults(AffinityType attacker,AffinityType defender)
        {
            AffinityResults results = AffinityResults.Normal;
            if(affinityMaps[attacker].strongAgainst == defender)
            {
                results = AffinityResults.Strong;
            }
            if (affinityMaps[attacker].weakagainst == defender)
            {
                results = AffinityResults.Weak;
            }

            return results;
        }
        //Call before using
        public  void SetupMap()
        {
            //---
            AfinityMiniTable fire;
            fire.strongAgainst = AffinityType.Metal;
            fire.weakagainst = AffinityType.Earth;
            affinityMaps.Add(AffinityType.Fire, fire);
            //---
            AfinityMiniTable metal;
            metal.strongAgainst = AffinityType.Wood;
            metal.weakagainst = AffinityType.Water;
            affinityMaps.Add(AffinityType.Metal, metal);
            //---
            AfinityMiniTable wood;
            wood.strongAgainst = AffinityType.Earth;
            wood.weakagainst = AffinityType.Fire;
            affinityMaps.Add(AffinityType.Wood, wood);
            //---
            AfinityMiniTable earth;
            earth.strongAgainst = AffinityType.Water;
            earth.weakagainst = AffinityType.Metal;
            affinityMaps.Add(AffinityType.Earth, earth);
            //---
            AfinityMiniTable water;
            water.strongAgainst = AffinityType.Fire;
            water.weakagainst = AffinityType.Wood;
            affinityMaps.Add(AffinityType.Water, water);
            //---
            AfinityMiniTable none;
            none.strongAgainst = AffinityType.Ignore;
            none.weakagainst = AffinityType.Ignore;
            affinityMaps.Add(AffinityType.None, none);
            //---
        }
    }
    public struct AfinityMiniTable
    {
        public Affinity.AffinityType weakagainst;
        public Affinity.AffinityType strongAgainst;
    }
   
}
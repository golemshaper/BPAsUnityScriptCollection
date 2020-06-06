using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.BPA
{ 
    public class RPGActorPuppet : MonoBehaviour
    {
        private RPGActor watchActor;
        public string actorName;
        public int bindToActorID = 0;
        public bool autoBindByID;
        public bool autoBindByName;
        public bool isInHeroParty;

        public Transform myTransform;
        public GameObject debugPullSword;
        public void Awake()
        {
            //SEE FUNCTION FOR A WARNING ABOUT THE AUTO BINDER!!
            AutoBinders();
            if (myTransform == null) myTransform = this.transform;
            
        }
        private void Setup()
        {
            //called after binding the actor
            watchActor.animEventReadyWeapon += PullWeaponAnim;
        }
        void UnSetup()
        {
            watchActor.animEventReadyWeapon -= PullWeaponAnim;

        }
        private void OnDestroy()
        {
            UnSetup();
        }
        void PullWeaponAnim()
        {
            debugPullSword.gameObject.SetActive(true);
        }
        void AutoBinders()
        {
            /*
            * 
            * WARNING: Auto bind shouldn't be used !
            * instead, create actors from the GFX tag of their stats page, and instantiate them and hook them up in code somewhere, maybe in the battle system, but probably
            * in its own script that interfaces with the battle ssytem via events!
            * 
            * Bind by idea doesn't know what the character should look like! autobinding is for testing only!
            *
            */
            if (autoBindByID)
            {
                if (isInHeroParty) BindToActor(RpgBattleSystemMain.instance.heroParty[bindToActorID]);
                if (!isInHeroParty) BindToActor(RpgBattleSystemMain.instance.enemyParty[bindToActorID]);
            }
            if (autoBindByName)
            {
                //WARNING! Outside of initial testing, never bind by name! it's not going to be useful at all!
                BindToActor(actorName);
            }
        }
        public void BindToActor(string myActorName)
        {
            //WARNING! Outside of initial testing, never bind by name! it's not going to be useful at all!
            if (isInHeroParty)
            {
                int index = RpgBattleSystemMain.instance.GetHeroIndex(myActorName);
                BindToActor(RpgBattleSystemMain.instance.heroParty[index]);
            }
            else
            {
                //WARNING: This will only return the first instance! This isn't really useful except for some work in progress type code.
                int index = RpgBattleSystemMain.instance.GetEnemyIndex(myActorName);
                BindToActor(RpgBattleSystemMain.instance.enemyParty[index]);
            }
        }
        public void BindToActor(RPGActor bindToActor)
        {
            watchActor = bindToActor;
            //lock transform to actor so you know the target location on screen!
            watchActor.myTransform = myTransform;
            Setup();
        }
        /*
         * Respond to events that the actor calls to animate the character.
         * this should act as a go-between for some type of animator and the actor.
         * I'm making a procedural animation component that will be the default way of moving characters, 
         * but you should be able to use something else, or something in addition
         * 
         * the procedural animator could be another class, and I can make an animation wrapper component 
         * that can either send input ot a unity Animator,or to the procedural animamation component
         */
    }
}
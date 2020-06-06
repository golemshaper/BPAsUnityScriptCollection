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
        Vector3 initialPosition=Vector3.zero; //override this at start of movement if you want it to be DQ XI style!

        public enum State {idle,pullWeapon,attack,die,dead,revive};
        AckkStateMachine sm = new AckkStateMachine();
        private void Update()
        {
            sm.UpdateTick();
        }

        //---------------------------SETUP:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        public void Awake()
        {
            //SEE FUNCTION FOR A WARNING ABOUT THE AUTO BINDER!!
            AutoBinders();
            if (myTransform == null) myTransform = this.transform;

            sm.LinkOnEnterState(State.pullWeapon, () => debugPullSword.gameObject.SetActive(true));
            sm.LinkStates(State.attack,()=>MoveToTargetUpdate());
        }
        private void Setup()
        {
            //called after binding the actor
            watchActor.animEventReadyWeapon += PullWeaponAnim;
            watchActor.animEventAttack += AttackAnim;
        }
        void UnSetup()
        {
            watchActor.animEventReadyWeapon -= PullWeaponAnim;
            watchActor.animEventAttack -= AttackAnim;

        }
        private void OnDestroy()
        {
            UnSetup();
        }
        void MoveToTargetUpdate()
        {
            //TODO: Add an option to pause the main battle loops message proccessing until an animation has completed!
            myTransform.position = GameMath.Parabola(initialPosition, watchActor.targetLocation, 2f, sm.TimeInState );

            if (sm.TimeInState > 1f)
            {
                sm.TimeInState = 1f;
                //snap back to position for now...\//when you have an option to halt the message queue until the animation has finished, you can do whatever you would like
                //and take however long you want
                //but until then I need the player back at the start quickly before then enemy attacks...
                myTransform.position = initialPosition;
                sm.SetState(State.idle);
            }

        }
        void AttackAnim()
        {
             sm.SetState(State.attack);
        }
        void PullWeaponAnim()
        {
            sm.SetState(State.pullWeapon);
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
            initialPosition = myTransform.position;
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
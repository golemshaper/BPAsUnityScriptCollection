using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.BPA
{ 
    public class RPGActorPuppet : MonoBehaviour
    {
        private RPGActor myActor;
        public string actorName;
        public int bindToActorID = 0;
        public bool autoBindByID;
        public bool autoBindByName;
        public bool isInHeroParty;

        public Transform myTransform;
        public GameObject debugPullSword;
        Vector3 initialPosition=Vector3.zero; //override this at start of movement if you want it to be DQ XI style!
        public bool captueInitPosBeforeAtk;
        public enum State {idle,pullWeapon,attack,returnToIdle,die,dead,revive};
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
            sm.LinkStates(State.attack,()=>MoveToTargetUpdate(),()=> AttackStart());
            sm.LinkStates(State.returnToIdle,()=>MoveToStartingPositionUpdate());
            sm.speed = 4f; //make speed faster!
        }
        public void AttackStart()
        {
            RpgBattleSystemMain.instance.SetBlockActionQueue(true);
            if (captueInitPosBeforeAtk)
            {
                initialPosition = myTransform.position;
            }
        }
        Vector3 targetPosition=Vector3.zero;
        private void Setup()
        {
            //called after binding the actor
            myActor.animEventReadyWeapon += PullWeaponAnim;
            myActor.animEventAttack += AttackAnim;
            myActor.animEventDeath += DeathAnim;

        }
        void UnSetup()
        {
            myActor.animEventReadyWeapon -= PullWeaponAnim;
            myActor.animEventAttack -= AttackAnim;
            myActor.animEventDeath -= DeathAnim;

        }
        void DeathAnim()
        {
            this.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            UnSetup();
        }
        /// <summary>
        ///TIP: 0 is initial position, 1 is target position. 0.9 is close but not quite on top of target...
        /// </summary>
        /// <param name="percentageOfLine"></param>
        /// <returns></returns>
        public Vector3 CloseInOnTarget(float percentageOfLine)
        {
            return Vector3.Lerp(initialPosition, myActor.targetLocation, percentageOfLine);
        }
        void MoveToTargetUpdate()
        {
            //NOTE: IT BLOCKS THE MESSAGE QUEUE UPON ENTERING THIS STATE.

            //TODO: Add an option to pause the main battle loops message proccessing until an animation has completed!
            float progress = sm.TimeInState;
            if (progress > 1f)
            {
                progress = 1f; //clamp
                //unblock queue when striking target
                RpgBattleSystemMain.instance.SetBlockActionQueue(false);
            }

            myTransform.position = GameMath.Parabola(initialPosition, CloseInOnTarget(0.75f), 2f,progress);

            //pause a bit at the end before returning
            if (sm.TimeInState > 2f)
            {
                
                sm.SetState(State.returnToIdle);
            }

        }
        void MoveToStartingPositionUpdate()
        {
            //NOTE: IT BLOCKS THE MESSAGE QUEUE UPON ENTERING THIS STATE.

            //TODO: Add an option to pause the main battle loops message proccessing until an animation has completed!
            float progress = sm.TimeInState;
            if (progress > 1f)
            {
                myTransform.position = initialPosition;
                progress = 1f; //clamp
            }
            else
            {
                myTransform.position = GameMath.Parabola(CloseInOnTarget(0.9f), initialPosition, 2f, progress);
            }
            //pause a bit at the end before returning
            if (sm.TimeInState > 1f)
            {
               //the queue is already unblocked
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
            myActor = bindToActor;
            //lock transform to actor so you know the target location on screen!
            myActor.myTransform = myTransform;
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
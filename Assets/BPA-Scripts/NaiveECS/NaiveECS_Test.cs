using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NaiveECS
{
    //I'm just using this to test some concepts...
    //This is probably not the right way to implement this.
    //I will update this as I do more research
    //--
    //this is a mess.
    //make a clean scalable version of this please!
    //thanks

    /*
     * I coooould try making a class that has an integer reference to where it's component is, and and what it is. (2 ints)
     * and loop through that to figure out what components need to send data back, but that's no good...
     * 
     * 
     * Entity-Component-System

        -Entity: An entity is just an ID
        -Component: Components are just data.
        -System: Logic that runs on every entity that has acomponent of the system. 
            
        -For example, a "Renderer" system would draw all entities that have a "appearance" component.



        http://vasir.net/blog/game-development/how-to-build-entity-component-system-in-javascript

        https://medium.com/ingeniouslysimple/entities-components-and-systems-89c31464240d

        https://austinmorlan.com/posts/entity_component_system/

        https://gameprogrammingpatterns.com/component.html

    */
    public class NaiveECS_Test : MonoBehaviour
    {
        public List<NaiveEntity> entityList = new List<NaiveEntity>();
        //find a better way to do these...
        public List<Health> healthComponents = new List<Health>();
        public List<PositionCmp> positionComponents = new List<PositionCmp>();
        public List<Movement> movementComponents = new List<Movement>();



        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < entityList.Count; i++)
            {
                entityList[i].id = i;
            }
            SetupPlayer(0,new Vector3(1,0,0),4f);
            SetupPlayer(1, new Vector3(0,1f,0f),1f);
        }
        void SetupPlayer(int id, Vector3 moveDir, float speed)
        {
            Health nHealth;
            nHealth.entityID = id;
            nHealth.currentHealth = 100;
            nHealth.maxHealth = 100;
            healthComponents.Add(nHealth);

            PositionCmp nPosition;
            nPosition.entityID = id;
            nPosition.pos = Vector2.zero;
            positionComponents.Add(nPosition);

            Movement mov;
            mov.entityID = id;
            mov.dir = moveDir;
            mov.speed = speed;
            movementComponents.Add(mov);
        }
        // Update is called once per frame
        void Update()
        {
            //I need some way to keep track of the index of each component. I can't rely on creation time since all entities
            //won't take all components.
            //but this loop sucks when I could just have an index.
            //I need some way to update entity...? am I doing this backwards or something?
            PositionUpdates();
            for (int i = 0; i < entityList.Count; i++)
            {
                foreach (PositionCmp p in positionComponents)
                {
                    if (p.entityID == entityList[i].id)
                    {
                        entityList[i].myTransform.position = p.pos;
                    }
                }

            }
        }
        void PositionUpdates()
        {
            for (int i = 0; i < positionComponents.Count; i++)
            {
                Vector3 modVector = positionComponents[i].pos;
                int movementID = GetMovementComponentIndex(positionComponents[i].entityID);
                modVector += movementComponents[movementID].dir * movementComponents[movementID].speed*Time.deltaTime;
                int id = positionComponents[i].entityID;
                PositionCmp p = positionComponents[i];
                p.entityID = id;
                p.pos = modVector;
                positionComponents[i] = p;
            }
        }
        /// <summary>
        ///See this? I need to find a way to make a genric function like this that will work on any componentn type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int GetMovementComponentIndex(int id)
        {
            for (int i = 0; i < movementComponents.Count; i++)
            {
                if(movementComponents[i].entityID==id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    [System.Serializable]
    public struct Health
    {
        public int entityID;
        public int currentHealth;
        public int maxHealth;

        public int GetID()
        {
            return entityID;
        }

        public void SetID(int set)
        {
            entityID = set;
        }
    }
    [System.Serializable]
    public struct PositionCmp
    {
        public int entityID;
        public Vector3 pos;
    }
    [System.Serializable]
    public struct Movement
    {
        public int entityID;
        public Vector3 dir;
        public float speed;
    }


}
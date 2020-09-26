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
    public class NaiveECS_Test : MonoBehaviour
    {
        public List<NaiveEntity> entityList = new List<NaiveEntity>();
        //find a better way to do these...
        public List<Health> healthComponents = new List<Health>();
        public List<PositionCmp> positionComponents = new List<PositionCmp>();

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < entityList.Count; i++)
            {
                entityList[i].id = i;
            }
            SetupPlayer(0);
        }
        void SetupPlayer(int id)
        {
            Health nHealth = new Health();
            nHealth.entityID = id;
            nHealth.currentHealth = 100;
            nHealth.maxHealth = 100;
            healthComponents.Add(nHealth);

            PositionCmp nPosition = new PositionCmp();
            nPosition.entityID = id;
            nPosition.pos = Vector2.zero;
            positionComponents.Add(nPosition);
        }
        // Update is called once per frame
        void Update()
        {
            PositionUpdates();
            for (int i = 0; i < entityList.Count; i++)
            {
                foreach(PositionCmp p in positionComponents)
                {
                    if(p.entityID == entityList[i].id)
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
                modVector.x += 3 * Time.deltaTime;
                int id = positionComponents[i].entityID;
                PositionCmp p = positionComponents[i];
                p.entityID = id;
                p.pos = modVector;
                positionComponents[i] = p;
            }
        }
    }
   [System.Serializable]
    public struct Health
    {
        public int entityID;
        public int currentHealth;
        public int maxHealth;
    }
    [System.Serializable]
    public struct PositionCmp
    {
        public int entityID;
        public Vector3 pos;
    }
}
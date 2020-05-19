using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//BP@

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    [SerializeField]
    private bool makeLocal=false;
    [SerializeField]
    private bool doNotPopulatePools=false;
    public ObjectPoolEntry[] objectPools;

    // Awake is called before the first frame update
    void Awake()
    {
        if(!makeLocal)
        {
            instance = this;
        }
    }
    private void Start()
    {
        if (doNotPopulatePools) return;
        foreach (ObjectPoolEntry pool in objectPools)
        {
            pool.PopulatePool(this.transform);
        }
    }
    /// <summary>
    /// Spawn by name.
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject SpawnObject(string objName, Vector3 pos)
    {
        foreach(ObjectPoolEntry o in objectPools)
        {
            if(o.Name==objName)
            {
                //Found object potentially. it may be null if the pool is maxed out.
                GameObject spawn = o.SpawnObject();
                if (spawn == null) return null;
                spawn.gameObject.SetActive(true);
                spawn.transform.localPosition = pos;
                return spawn;
            }
        }
        return null;
    }
    /// <summary>
    /// Spawn by ID
    /// </summary>
    /// <param name="objId"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject SpawnObject(int objId, Vector3 pos)
    {
        if (objId <= -1) return null;
        GameObject spawn = objectPools[objId].SpawnObject();
        if (spawn == null) return null;
        spawn.gameObject.SetActive(true);
        spawn.transform.localPosition = pos;
        NavMeshAgent agent= spawn.GetComponent<NavMeshAgent>();
        if(agent!=null)
        {
            agent.Warp(pos);
        }
        return spawn;
    }
    public GameObject SpawnObject(int objId, Vector3 pos,Transform parent)
    {
        if (objId <= -1) return null;
        GameObject spawn = objectPools[objId].SpawnObject();
        if (spawn == null) return null;
        spawn.transform.parent = parent;
        spawn.gameObject.SetActive(true);
        spawn.transform.localPosition = pos;
        return spawn;
    }
    public GameObject SpawnObject(int objId, Vector3 pos, bool useLocal)
    {
        if (objId <= -1) return null;
        GameObject spawn = objectPools[objId].SpawnObject();
        if (spawn == null) return null;
        spawn.gameObject.SetActive(true);
        if (useLocal)
        { 
            spawn.transform.localPosition = pos;
        }
        else
        {
            spawn.transform.position = pos;
        }
        return spawn;
    }
    
    public void RecycleObject(GameObject recycleMe, string retrunToPool)
    {
        RecycleObject(recycleMe, GetPoolFromName(retrunToPool));
    }
    public void RecycleObject(GameObject recycleMe,int id)
    {
        if(id==-1)
        {
            recycleMe.gameObject.SetActive(false);
            return;//Belongs to no pool, so just deactivate it.
        }
        RecycleObject(recycleMe, objectPools[id]);
    }
    public void ShufflePool(string poolName)
    {
        ShufflePool(GetPoolID(poolName));
    }
    public void ShufflePool(int poolID)
    {
        if (poolID == -1) return;
        objectPools[poolID].ShufflePool();
    }
    public void RecycleObject(GameObject recycleMe, ObjectPoolEntry retrunToPool)
    {
        //I don't love needing to specify which pool it needs to be returned to. I'll fix this later.
        if(retrunToPool==null)
        {
            //belongs to no pool, just disable it.
            recycleMe.SetActive(false);
            return;
        }
        retrunToPool.Recycle(recycleMe);
    }
    public int GetPoolID(string poolName)
    {
        for (int i = 0; i < objectPools.Length; i++)
        {
            ObjectPoolEntry o = objectPools[i];
            if (o.Name == poolName)
            {

                return i;
            }
        }
        return -1;
    }
    ObjectPoolEntry GetPoolFromName(string objName)
    {
        ObjectPoolEntry result = null;
        foreach (ObjectPoolEntry o in objectPools)
        {
            if (o.Name == objName)
            {
                result = o;
            }
        }
        return result;
    }
}
[System.Serializable]
public class ObjectPoolEntry
{
    public string Name;
    [SerializeField]
    public GameObject prefab;
    public int objectCount=1;
    
   // [HideInInspector]
    public List<GameObject> pool= new List<GameObject>();
    public void PopulatePool()
    {
        PopulatePool(null);
    }
    public void PopulatePool(Transform parent)
    {
        for(int i=0; i< objectCount;i++)
        {
            //add an option to create more if pool is maxed out.
            GameObject g = GameObject.Instantiate(prefab, parent);
            pool.Add(g);
            g.SetActive(false);
        }
     
    }
    public void ShufflePool()
    {
        System.Random rng = GameManager.instance.RandomMain();
        int n = pool.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = pool[k];
            pool[k] = pool[n];
            pool[n] = value;
        }
        Debug.Log("Shuffled");
    }
    public GameObject SpawnObject()
    {
        GameObject getObj = null;
        if (pool.Count <= 0) return null;
        getObj=pool[0];
        //remove from pool.
        pool.Remove(getObj);
        return getObj;
    }
    public void Recycle(GameObject recycleMe)
    {
        recycleMe.SetActive(false);
        //maybe do a saftey check so you don't double add an object.
        pool.Add(recycleMe);
    }

}
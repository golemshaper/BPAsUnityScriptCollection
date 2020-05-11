using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepoolAfterTime : MonoBehaviour
{
    public float timer = 1f;
    public string PoolName = "Bullet";
    int poolID;
    public GameObject objectToRepool;
    public bool RepoolOnDisable;
    public bool pauseTimer;
    // Start is called before the first frame update
    void Start()
    {
        poolID = ObjectPool.instance.GetPoolID(PoolName);
        if (objectToRepool == null) objectToRepool = this.gameObject;
    }
    private void OnEnable()
    {
        curTimer = 0f;
        doUpdate = true;
    }
    private void OnDisable()
    {
        if (RepoolOnDisable)
        {
            ObjectPool.instance.RecycleObject(objectToRepool, poolID);
            doUpdate = false;
        }
        curTimer = 0f;
    }
    float curTimer = 0f;
    bool doUpdate = true;
    // Update is called once per frame
    void Update()
    {
        if (!doUpdate) return;
        if (curTimer < timer)
        {
           if(!pauseTimer) curTimer += Time.deltaTime;
            return;
        }
        if(RepoolOnDisable)
        {
            //OnDisable will handle repooling, so return so you don't recycle it twice
            this.gameObject.SetActive(false);
            return;
        }
        ObjectPool.instance.RecycleObject(objectToRepool, poolID);
        doUpdate = false;
    }
}

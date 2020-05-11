using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepoolAfterTime : MonoBehaviour
{
    public float timer = 1f;
    public string PoolName = "Bullet";
    int poolID;
    public GameObject objectToRepool;
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
            curTimer += Time.deltaTime;
            return;
        }
        ObjectPool.instance.RecycleObject(objectToRepool, poolID);
        doUpdate = false;
    }
}

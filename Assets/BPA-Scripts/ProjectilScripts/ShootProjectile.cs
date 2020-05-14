using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    public Transform spawnLocation;
    [SerializeField] string projectileName = "Bullet";
    int projectileID;
    //using game object as an interface for shootting so it works with the other scripts that can trigger object activation.
    public GameObject ShootWhileEnabled;
    public float fireRate = 0.1f;
    public bool enableUpdate = true;
    public bool desynchStartTime;
    // Start is called before the first frame update
    void Start()
    {
        projectileID = ObjectPool.instance.GetPoolID(projectileName);
        if (spawnLocation == null) spawnLocation = this.transform;
        if(desynchStartTime)
        {
            timer = Random.Range(0, 2f);
        }
    }
    void CreateBullet()
    {
        //change timer 
        timer = fireRate;
        GameObject bullet = ObjectPool.instance.SpawnObject(projectileID, spawnLocation.position);
        bullet.transform.rotation = spawnLocation.rotation;

    }
    float timer=0f;
    // Update is called once per frame
    void Update()
    {
        if (!enableUpdate) return;
        if(timer>0f)
        {
            timer -= Time.deltaTime;
            return;
        }
        if(ShootWhileEnabled!=null)
        {
            if(ShootWhileEnabled.activeSelf)
            {
                CreateBullet();
            }
        }

    }
}
    
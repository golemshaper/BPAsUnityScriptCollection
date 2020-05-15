using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnViaPoolOnEnable : MonoBehaviour
{
    [SerializeField] string SpawnObject = "Bullet";
    int spawnID = -1;
    public Transform spawnPoint;
    private void Start()
    {
        if (ObjectPool.instance == null) return;
        spawnID = ObjectPool.instance.GetPoolID(SpawnObject);
        if (spawnPoint == null) spawnPoint = this.transform;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        if (spawnID == -1) Start();
        GameObject g= ObjectPool.instance.SpawnObject(spawnID, spawnPoint.position);
        if (g != null) g.transform.rotation = spawnPoint.rotation;
    }

}
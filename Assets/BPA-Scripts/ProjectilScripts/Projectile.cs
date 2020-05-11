using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public Rigidbody myRigidbody;
    public float speed = 10f;
    Transform t;
    private void Awake()
    {
        if (myRigidbody == null) myRigidbody = this.GetComponent<Rigidbody>();
        t = myRigidbody.transform;
    }
    // Start is called before the first frame update
    void Update()
    {
        myRigidbody.velocity = t.TransformDirection(Vector3.forward * speed);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetForceImpulseOnEnable : MonoBehaviour
{
    public Vector3 thrust = new Vector3(1, 1, 1);
    public Rigidbody rb;
    void OnEnable()
    {
        if(rb==null)rb = GetComponent<Rigidbody>();
        rb.AddForce(thrust, ForceMode.Impulse);
        rb.AddTorque(thrust, ForceMode.Impulse);
    }
}

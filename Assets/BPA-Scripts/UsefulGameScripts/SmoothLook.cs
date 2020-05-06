using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothLook : MonoBehaviour {
    public Transform myT;
    public Transform target;
    public Vector3 offset;
    Vector3 lookAtPos;
    public float speed = 5;
    public bool targetMustBeActive;

    // Use this for initialization
    void Start () {
        if (myT == null)
            myT = this.transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(targetMustBeActive)
        {
            if (target.gameObject.activeSelf == false) return;
        }
        Quaternion targetRotation = Quaternion.LookRotation((target.transform.position+offset) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
	}
}

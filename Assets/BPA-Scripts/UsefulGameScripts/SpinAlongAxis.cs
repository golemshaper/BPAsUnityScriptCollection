using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAlongAxis : MonoBehaviour {
    public bool x = false;
    public bool y=true;
    public bool z = false;

    public float speed=1f;
    public Transform myTransform;
    Quaternion originalRot;
    public bool restoreAngleOnDisable;
	// Use this for initialization
	void Start () {
        if(myTransform==null)myTransform = this.gameObject.transform;
        originalRot = myTransform.localRotation;
    }
    private void OnDisable()
    {
        if(restoreAngleOnDisable)
        {
            if (myTransform != null) myTransform.localRotation = originalRot;
        }
    }
    // Update is called once per frame
    void Update ()
    {
        Vector3 eulerAngles = myTransform.localEulerAngles;
        //OLD: if (y)myTransform.RotateAround(transform.position, transform.up, speed*Time.deltaTime);
        if(x)eulerAngles.x += speed * Time.deltaTime;
        if(y)eulerAngles.y += speed * Time.deltaTime;
        if(z)eulerAngles.z += speed * Time.deltaTime;
        myTransform.localEulerAngles = eulerAngles;
    }
}

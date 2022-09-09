using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignToFloorNormals : MonoBehaviour {
    float offsetDistance = 1f;
    RaycastHit hit = new RaycastHit();
    public Transform myTransform;
    int customLayerMask;

    Quaternion previous = Quaternion.identity;
    Quaternion next = Quaternion.identity;
    // Use this for initialization
    void Start () {
       if(myTransform==null) myTransform = transform;

        customLayerMask = LayerMask.GetMask("Characters");
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, customLayerMask))
        {
            offsetDistance = hit.distance;
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
            Quaternion result = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            next = Quaternion.Lerp(previous, result,12f*Time.deltaTime);
            myTransform.rotation = next;
            previous = myTransform.rotation;
        }
    }
}

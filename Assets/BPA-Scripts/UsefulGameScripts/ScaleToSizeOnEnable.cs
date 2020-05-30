using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToSizeOnEnable : MonoBehaviour
{
    float progress = 0f;
    public float speed = 1f;
    bool allowUpdate = true;
    public Transform myTransform;
    public Vector3 startSize = Vector3.zero;
    public Vector3 endSize = new Vector3(1, 1, 1);
    public enum Interpolation {lerp,spring};
    public Interpolation interpolation=Interpolation.lerp;
    private void Awake()
    {
        if (myTransform == null) myTransform = this.transform;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        progress = 0f;
        allowUpdate = true;
        myTransform.localScale = startSize;

    }
    private void OnDisable()
    {
        //make sure scale is correct when disabled in case you decide to parent anything to it while it is inactive
        myTransform.localScale = endSize;
    }
    // Update is called once per frame
    void Update()
    {
        if (!allowUpdate) return;
        progress += speed*Time.deltaTime;
        switch (interpolation)
        {
            case Interpolation.lerp:
                myTransform.localScale = Vector3.Lerp(startSize,endSize,progress);
                break;
            case Interpolation.spring:
                myTransform.localScale = GameMath.Spring(startSize, endSize, progress);
                break;
        }
        if(progress>=1)
        {
            myTransform.localScale = endSize;
            allowUpdate = false;
        }
    }
}

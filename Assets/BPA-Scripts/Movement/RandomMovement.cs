using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public Rigidbody myRigidbody;
    Transform myTransform;
    public Transform rotationObject;
    public float speed=5f;
    public float rotationSpeed=5f;
    public float changeMin = 1f;
    public float changeMax = 2f;
    public float waitMin=0f;
    public float waitMax = 0.1f;

    float changeTime = 1f;
    public bool fourDirection = true;
    System.Random rand = new System.Random(12196);
    float curTimer=0f;


    public Vector3 moveDirection=Vector3.zero;
    public Vector3 changeDirection=Vector3.zero;
    private void Awake()
    {
        if (myRigidbody == null) myRigidbody = this.GetComponent<Rigidbody>();
        myTransform = myRigidbody.transform;
        if (rotationObject == null) rotationObject = myTransform;
        changeDirection = Vector3.forward;
        changeTime = Random.Range(changeMax, changeMax);

        curTimer = Random.Range(waitMin,waitMax);
        int seed =(int) (myTransform.position.x + myTransform.position.y + myTransform.position.z);
        rand = new System.Random(seed);
    }
    bool useWaitTimer = false;
    // Start is called before the first frame update
    void Update()
    {
        MovementUpdate();
        RotationUpdate();
    }
    void RotationUpdate()
    {
        Vector3 targetPos = moveDirection;
        Quaternion targetRotation = Quaternion.LookRotation(targetPos);
        rotationObject.rotation = Quaternion.Slerp(rotationObject.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    void MovementUpdate()
    {
        curTimer += Time.deltaTime;
        if (useWaitTimer)
        {
            if (curTimer >= changeTime)
            {
                curTimer = 0f;
                useWaitTimer = false;
            }
            return;
        }

        if (curTimer >= changeTime)
        {
            useWaitTimer = true;
            curTimer = Random.Range(waitMax, waitMax);
            changeDirection.x = rand.Next(-1, 1);
            changeDirection.z = rand.Next(-1, 1);
            if (fourDirection)
            {
                int dirLock = rand.Next(1, 4);
                if (dirLock == 1)
                {
                    changeDirection.x = 0;
                    if (changeDirection.z == 0)
                    {
                        changeDirection.z = 1;
                    }
                }
                if (dirLock == 2)
                {
                    changeDirection.z = 0;
                    if (changeDirection.x == 0)
                    {
                        changeDirection.x = 1;
                    }
                }
                if (dirLock == 3)
                {
                    changeDirection.z = 0;
                    if (changeDirection.x == 0)
                    {
                        changeDirection.x = -1;
                    }
                }
                if (dirLock == 4)
                {
                    changeDirection.x = 0;
                    if (changeDirection.z == 0)
                    {
                        changeDirection.z = -1;
                    }
                }
            }
            changeDirection.Normalize();
            moveDirection = changeDirection;
        }
        myRigidbody.velocity = myTransform.TransformDirection(changeDirection * speed);


    }
}

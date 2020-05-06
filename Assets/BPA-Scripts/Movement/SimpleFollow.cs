using UnityEngine;
using System.Collections;

public class SimpleFollow : MonoBehaviour
{
    public float damping = 0.1f;

    public Transform follow;
    public Vector3 offset;
    public bool grabOffsetAtStart;
    public Transform myTransform;
    Vector3 velocity = Vector3.zero;
    public bool x = true, y = true, z = true;
    Vector3 movetoPoint;
    Vector3 startingPoint;
    public bool randomlyStopForATime;
    public float minStop = 0.1f;
    public float maxStop = 1.5f;
    float curStop = 0f;
    bool flipStartOrStop;
    public bool targetMustBeActive;

    //temp hack:
    public PlayerMovementBPA onlyMoveWhenThisPlayerMoves;
    // Use this for initialization
    void Start()
    {
        if (myTransform == null)
            myTransform = this.transform;
        if (grabOffsetAtStart)
        {
            Vector3 originalOffset = offset;
            offset = myTransform.position - follow.position;
            if (!x) offset.x = originalOffset.x;
            if (!y) offset.y = originalOffset.y;
            if (!z) offset.z = originalOffset.z;

        }
        startingPoint = myTransform.position;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(targetMustBeActive)
        {
            if (follow.gameObject.activeSelf == false) return;
        }
        Vector3 targetPosition = follow.position;
        if (x == false)
            targetPosition.x = startingPoint.x;
        if (y == false)
            targetPosition.y = startingPoint.y;
        if (z == false)
            targetPosition.z = startingPoint.z;
        if (onlyMoveWhenThisPlayerMoves != null)
        {
            if (onlyMoveWhenThisPlayerMoves.isWalking() == false)
                return;
        }
        if (randomlyStopForATime)
        {
            if (curStop <= 0)
            {
                curStop = GameMath.GetRandomFloat(minStop, maxStop);
                flipStartOrStop = !flipStartOrStop;
            }
            if (curStop > 0)
            {
                curStop -= Time.deltaTime;
                if (flipStartOrStop) return;
            }
        }
        myTransform.position = Vector3.SmoothDamp(myTransform.position, targetPosition + offset, ref velocity, damping);
    }
}

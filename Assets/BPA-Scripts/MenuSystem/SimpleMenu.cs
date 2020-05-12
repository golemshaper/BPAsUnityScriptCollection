using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMenu : MonoBehaviour
{
    [Header("Slots")]
    public SimpleMenuSlot[] menuSlots;
    [Header("Cursor Settings")]
    public int cursorIndex = 0;
    public Transform cursor;
    public Vector3 cursorOffset;
    public bool x, y, z = true;
  
    public enum CursorMovementType {smooth,instant};
    public CursorMovementType cursorMoveType;
    [Header("Movement:")]
    public bool allowXMove=false;
    public bool allowYMove=true;
    public bool dpadOnly=false;
    public bool snapToIndexZeroOnEnable;
    public int ticsTillFastScroll = 2;
    public bool useLocalPosition;
    //Private:
    float slowWait = 0.5f;
    float fastWait = 0.1f;
    float curWaitTime = 0.5f;
    float timer = 0.2f;
    short curTick = 0;
    //Const:
    const float deadZone = 0.01f;
    private void OnEnable()
    {
        if(snapToIndexZeroOnEnable)cursorIndex = 0;
        Clamp();
    }
    Vector2 GetInputMove()
    {
        Vector2 result=PlayerInput.instance.GetMovementRaw(dpadOnly);
        if (!allowXMove) result.x = 0f;
        if (!allowYMove) result.y = 0f;
        return result;
    }
    float GetInput1D()
    {
        return  GetInputMove().normalized.x+ GetInputMove().normalized.y;
    }
    // Update is called once per frame
    void Update()
    {
        if (GetInput1D() <deadZone && GetInput1D() >-deadZone)
        {
            //no delay if not holding cursor move down.
            timer = 0f;
            curWaitTime = slowWait;
            curTick = 0;
        }
         SetCursorPosition();
        if(timer>0f)
        {
            //delay timer
            timer -= Time.deltaTime;
            return;
        }

        if(GetInput1D()>deadZone)
        {
            //Move cursor up
            cursorIndex--;
            timer = curWaitTime;
            //Fast scroll:
            if (curTick>=ticsTillFastScroll)curWaitTime = fastWait;
            curTick++;
        }
        else if (GetInput1D() <-deadZone)
        {
            //move cursor down
            cursorIndex++;
            timer = curWaitTime;
            //Fast scroll:
            if (curTick >= ticsTillFastScroll) curWaitTime = fastWait;
            curTick++;
        }
     
        //Clamp cursor:
        Clamp();
       
        if(PlayerInput.instance.GetFire2_A())
        {
            //SELECT
            menuSlots[cursorIndex].DoSelectAction();
        }
       
    }
    void Clamp()
    {
        if (cursorIndex > menuSlots.Length-1) cursorIndex = 0;
        if (cursorIndex < 0) cursorIndex = menuSlots.Length - 1;

    }

    float smoothTime = 0.05f;
    private Vector3 velocity = Vector3.zero;
    void SetCursorPosition()
    {
        Vector3 slotPosition = menuSlots[cursorIndex].GetPosition(useLocalPosition);
        Vector3 cursorResultLocation = slotPosition;
        Vector3 cursorTPos = cursor.position;
        if (useLocalPosition)
        {
            cursorTPos = cursor.localPosition;
        }
        switch (cursorMoveType)
        {
            case CursorMovementType.smooth:
              cursorResultLocation = Vector3.SmoothDamp(cursorTPos, slotPosition, ref velocity, smoothTime);
                break;
          
            case CursorMovementType.instant:
                cursorResultLocation = slotPosition;
                break;
        }
       
        if (!x) cursorResultLocation.x = cursorTPos.x;
        if (!y) cursorResultLocation.y = cursorTPos.y;
        if (!z) cursorResultLocation.z = cursorTPos.z;

        if (useLocalPosition)
        {
            cursor.localPosition = cursorResultLocation;
        }
        else
        {
            cursor.localPosition = cursorResultLocation;
        }
    }
}

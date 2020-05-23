using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMenu : MonoBehaviour
{
    /*
     * TODO: create a non monobehavior version of this class.
     * No need to make a wraper if this is already in use...
     */
    [Header("Main Settings")]
    public SimpleMenuMaster driveByMaster;
    public bool disableOnClose=true;
    public bool enableOnOpen=true;
    public bool enableBackButton = true;
    [Header("Slots")]
    public List<SimpleMenuSlot> menuSlots= new List<SimpleMenuSlot>();
  
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
    
    public enum SelectButtonType {A_cross,B_circle};
    [Header("Input")]
  
    public SelectButtonType selectButton = SelectButtonType.B_circle;
    //Private:
    float slowWait = 0.5f;
    float fastWait = 0.1f;
    float curWaitTime = 0.5f;
    float timer = 0.2f;
    short curTick = 0;
    SimpleMenu history=null;
    //Const:
    const float deadZone = 0.01f;
    private void OnEnable()
    {
        OpenMenu();
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
    private void Update()
    {
        if(driveByMaster==null)
        {
            DoUpdate();
        }
    }
    public void GoBack()
    {
        if (history == null) return;
        if(driveByMaster!=null)
        {
            //don't record history going backwards!
            driveByMaster.SetActiveMenu(history, false);
        }
    }
    public void OpenMenu()
    {
        OpenMenu(null);
    }
    public void OpenMenu(SimpleMenu nHistory)
    {
        if(nHistory!=null) history = nHistory;
        Clamp();
        if (snapToIndexZeroOnEnable) cursorIndex = 0;
      
        SetCursorPosition(true);
        if (!enableOnOpen) return;
        this.gameObject.SetActive(true);
    }
    public void CloseMenu()
    {
        if (!disableOnClose) return;
        this.gameObject.SetActive(false);
    }
   
    // Update is called once per frame
    public void DoUpdate()
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
        //SELECT
        ReadSelection();
        //GO BACK
        ReadBackButton();


    }
    void ReadBackButton()
    {
        if (!enableBackButton) return;
        switch (selectButton)
        {
            case SelectButtonType.A_cross:
                if (PlayerInput.instance.GetFire1_B())
                {
                    //Opposite of click button on purpose
                    GoBack();
                }
                break;
            case SelectButtonType.B_circle:
                if (PlayerInput.instance.GetFire2_A())
                {
                    //Opposite of click button on purpose
                    GoBack();
                }
                break;
        }
        return;
    }
    void ReadSelection()
    {
        switch (selectButton)
        {
            case SelectButtonType.A_cross:
                if (PlayerInput.instance.GetFire2_A())
                {
                    //SELECT
                    menuSlots[cursorIndex].DoSelectAction();
                }
                break;
            case SelectButtonType.B_circle:
                if (PlayerInput.instance.GetFire1_B())
                {
                    //SELECT
                    menuSlots[cursorIndex].DoSelectAction();
                }
                break;
        }
     
    }
    void Clamp()
    {
        if (cursorIndex > menuSlots.Count-1) cursorIndex = 0;
        if (cursorIndex < 0) cursorIndex = menuSlots.Count - 1;

    }

    float smoothTime = 0.05f;
    private Vector3 velocity = Vector3.zero;
    void SetCursorPosition()
    {
        SetCursorPosition(false);
    }
    void SetCursorPosition(bool snap)
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
        if(snap)
        {
            cursorResultLocation = slotPosition;
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
            cursor.position = cursorResultLocation;
        }
    }
}

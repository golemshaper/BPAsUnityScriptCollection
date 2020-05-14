using UnityEngine;
using System.Collections;
using Ackk.GameEngine.Helpers;

public class FlashObject : MonoBehaviour,IUpdateSlave 
{
    //Variable
    public bool UpdateOnEnable=false;
    public float delay=0f;
    public float duration=1f;
    public float frequency=0.05f;
    bool flashState=false;
    float curCounter=0f;
    float curFreq=0f;
    public GameObject BlinkObject;
    public Renderer BlinkRenderer;
    public bool ActiveAtEnd = true;
    [Header("START")]
    public GameObject[] enableAtStart;
    public GameObject[] disableAtStart;
    [Header("END")]
    public GameObject[] enableAtEnd;
    public GameObject[] disableAtEnd;
    float curDelay=0f;
    public GameObject stopFlashingIfInactive;
    public bool disableSelfAtEnd;

    //Implement interface
    #region IUpdateSlave implementation
    public void DoUpdate()
    {
        if (stopFlashingIfInactive != null)
        {
            if (stopFlashingIfInactive.activeSelf == false)
            {
                EndFlash();
                return;
            }
        }
        if (curDelay < delay)
        {
            curDelay += Time.deltaTime;
            return;
        }
        curCounter +=Time.deltaTime;
        curFreq +=Time.deltaTime;
        if (curCounter < duration)
        {
            if (curFreq < frequency)
            {
                return;
            }
            flashState = !flashState;
            curFreq = 0f;
            if (BlinkObject != null)
            {
                BlinkObject.SetActive(flashState);
            }
            if (BlinkRenderer != null)
            {
                BlinkRenderer.enabled=(flashState);
            }
            return;
        }
        EndFlash();
    }
    #endregion
    public bool IsFlashing()
    {
        return isFlashing;
    }
    public void BeginFlash(float lastsFor,float rate,bool showAtEnd)
    {
        BeginFlash(lastsFor,rate,showAtEnd,0f);

    }
    public void BeginFlash(float lastsFor,float rate,bool showAtEnd,float waitBeforeStarting)
    {
        if (isFlashing)
            return;
        GameObjectOperations.EnableDisableGameObjects(enableAtStart, true);
        GameObjectOperations.EnableDisableGameObjects(disableAtStart, false);
        delay = waitBeforeStarting;
        duration = lastsFor;
        frequency = rate;
        ActiveAtEnd = showAtEnd;
        isFlashing = true;
        UpdateMaster.Register(this as IUpdateSlave);

    }
    public void EndFlash()
    {
        if (BlinkObject != null)
        {
            BlinkObject.SetActive(ActiveAtEnd);
            flashState = !flashState;
        }
        if (BlinkRenderer != null)
        {
            BlinkRenderer.enabled=(ActiveAtEnd);
            flashState = !flashState;
        }
        //reset timers
        curFreq=0f;
        curCounter = 0f;
        curDelay = 0f;
        //unregister
        UpdateMaster.Deregister(this as IUpdateSlave);
        isFlashing = false;

        GameObjectOperations.EnableDisableGameObjects(enableAtEnd, true);
        GameObjectOperations.EnableDisableGameObjects(disableAtEnd,false);

        if (disableSelfAtEnd) this.gameObject.SetActive(false);

    }
    bool isFlashing=false;
    //register/un-register
    void OnEnable()
    {
        if (UpdateOnEnable)
        {
            if (isFlashing)
                return;
            UpdateMaster.Register(this as IUpdateSlave);
            isFlashing = true;
            GameObjectOperations.EnableDisableGameObjects(enableAtStart, true);
            GameObjectOperations.EnableDisableGameObjects(disableAtStart, false);
        }
    }
    void OnDisable()
    {
        if (UpdateOnEnable)
        {
            if (!isFlashing)
                return;
            EndFlash();
            isFlashing = false;
        }
    }
    void OnDestroy() 
    {
        if (isFlashing)
        {
            //you must make sure that you unregister the object if the object is destroyed, or else update will be called even if the object no longer exists!
            EndFlash();
            isFlashing = false;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ackk.GameEngine.ActionTimer;
using System;


public class ActionTimerManager : MonoBehaviour,IUpdateSlave 
{
    public List<ActionAfterTime> ActionTimer;
    public List<ActionAfterTime> ActionTimerPool;

    private static ActionTimerManager instance;
    // Always make an instance if object not found.
    public static ActionTimerManager GetInstance()
    {
        if (applicationIsQuitting == true)
            return null;
        if (instance == null)
        {
            GameObject actionTimerM = new GameObject("/ActionTimerManager/");
            actionTimerM.AddComponent<ActionTimerManager>();
            instance = actionTimerM.GetComponent<ActionTimerManager>();
            if(Application.isPlaying) DontDestroyOnLoad(instance.gameObject);
        }
        return instance;
    }
    private static bool applicationIsQuitting = false;
    public void OnDestroy()
    {
        //needed to stop error when closing game.
        applicationIsQuitting = true;
        //you must make sure that you unregister the object if the object is destroyed, or else update will be called even if the object no longer exists!
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
    const int ID_System = 0;

    #region IUpdateSlave implementation
    //Update:
    public void DoUpdate()
    {
        UpdateActionTimers();
    }
    #endregion

    void UpdateActionTimers()
    {
        if (ActionTimer == null) return;

        ActionAfterTime[] actions = ActionTimer.ToArray();
        for (int i = actions.Length - 1; i >= 0; i--)
        {
            if (ActionTimer[i] == null) continue;

            ActionTimer[i].Update(1f);
            if (ActionTimer[i].clearMe)
            {
                RecycleTimer(ActionTimer [i],ref ActionTimer);
            }
        }
    }
    public void CancelActionByID(int id)
    {
        if (ActionTimer == null) return;
        ActionAfterTime[] actions = ActionTimer.ToArray();
        for (int i = actions.Length - 1; i >= 0; i--)
        {
            if (ActionTimer[i] == null) continue;
            if (ActionTimer[i].cancelID==id)
            {
                //please recycle!
                //ActionTimer.Remove(actions[i]);
                RecycleTimer(ActionTimer [i],ref ActionTimer);
            }
        }
    }

    public void DoActionAfterTime(float timer, Action action)
    {
        DoActionAfterTime(timer,action,ID_System);
    }
    public void DoActionAfterTime(float timer, Action action,Action updateAction)
    {
        DoActionAfterTime(timer,action,updateAction,ID_System);
    }
    public void DoActionAfterTime(float timer, Action action,Action updateAction,float afterTime)
    {
        DoActionAfterTime(timer,action,updateAction,afterTime,ID_System);
    }
    public float GetFirstTimerWithIdDurration(int id)
    {
        foreach (ActionAfterTime ac in ActionTimer.ToArray())
        {
            if (ac.cancelID == id)
            {
                return ac.timer;
            }
        }
        return 0f;
    }
    //---ID:
    //1
    public void DoActionAfterTime(float timer, Action action,int id)
    {
        //Example: DoActionAfterTime(2f,()=>FunctionNameHere());
        if (ActionTimer == null)
            ActionTimer = new List<ActionAfterTime>();
        ActionTimer.Add(CreateTimer(timer,action,id));
    }
    //2
    public void DoActionAfterTime(float timer, Action action,Action updateAction,int id)
    {
        if (ActionTimer == null)
            ActionTimer = new List<ActionAfterTime>();
        ActionTimer.Add(CreateTimer(timer,action,updateAction,id));
    }
    //3
    public void DoActionAfterTime(float timer, Action action,Action updateAction,float afterTime,int id)
    {
        if (ActionTimer == null)
            ActionTimer = new List<ActionAfterTime>();
        ActionTimer.Add(CreateTimer(timer,action,updateAction,afterTime,id));
    }
 
    //ACTION TIMER POOL:
    //1
    ActionAfterTime CreateTimer(float timer, Action action,int id)
    {
        if (ActionTimerPool == null)
            ActionTimerPool = new List<ActionAfterTime>();
        if (ActionTimerPool.Count <= 0)
        {
            ActionTimerPool.Add(new ActionAfterTime(timer, action, id));
            ActionTimerPool.Add(new ActionAfterTime(timer, action, id));
        }
        ActionAfterTime result = ActionTimerPool [0];
        result.SetProperties(timer,action,id);
        ActionTimerPool.Remove(result);
        return result;
    }
    //2
    ActionAfterTime CreateTimer(float timer, Action action,Action updateAction,int id)
    {
        if (ActionTimerPool == null)
            ActionTimerPool = new List<ActionAfterTime>();
        if (ActionTimerPool.Count <= 0)
        {
            ActionTimerPool.Add(new ActionAfterTime(timer, action, updateAction, id));
            ActionTimerPool.Add(new ActionAfterTime(timer, action, updateAction, id));
        }
        ActionAfterTime result = ActionTimerPool [0];
        result.SetProperties(timer,action,updateAction,id);
        ActionTimerPool.Remove(result);
        return result;
    }
    //3
    ActionAfterTime CreateTimer(float timer, Action action,Action updateAction,float afterTime,int id)
    {
        if (ActionTimerPool == null)
            ActionTimerPool = new List<ActionAfterTime>();
        if (ActionTimerPool.Count <= 0)
        {
            ActionTimerPool.Add(new ActionAfterTime(timer, action, updateAction, afterTime, id));
            ActionTimerPool.Add(new ActionAfterTime(timer, action, updateAction, afterTime, id));
        }
        ActionAfterTime result = ActionTimerPool [0];
        result.SetProperties(timer, action, updateAction, afterTime, id);
        ActionTimerPool.Remove(result);
        return result;
    }
    void RecycleTimer(ActionAfterTime a,ref List<ActionAfterTime> removeFrom)
    {
        //Recycled?
        a.clearMe=false;
        ActionTimerPool.Add(a);
        removeFrom.Remove(a);
    }
    //should this be public?
    void RecycleAll(ref List<ActionAfterTime> removeFrom)
    {
        ActionTimerPool.AddRange(removeFrom);
        removeFrom.Clear();
    }



    //register/un-register
    void OnEnable()
    {
        UpdateMaster.Register(this as IUpdateSlave);
    }
    void OnDisable()
    {
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
  
	
}
namespace Ackk.GameEngine.ActionTimer
{
    [System.Serializable]
    public class ActionAfterTime
    {   
        public float timer;
        public float onlyBeginUpdateActionAfterTime=0f;
        public Action e;
        public Action actionPerUpdate;
        public bool clearMe=false;
        public int cancelID = 0;
        public ActionAfterTime()
        {
            cancelID = 0;
        }
        public ActionAfterTime(float afterTime,Action doAction,int id)
        {
            e = doAction;
            actionPerUpdate = null;
            timer = afterTime;
            cancelID = id;
        }
        public ActionAfterTime(float afterTime,Action doAction,Action onUpdate,int id)
        {
            e = doAction;
            actionPerUpdate = onUpdate;
            onlyBeginUpdateActionAfterTime = 0f;
            timer = afterTime;
            cancelID = id;
        }
        public ActionAfterTime(float afterTime,Action doAction,Action onUpdate,float updateCallAfterTime,int id)
        {
            e = doAction;
            actionPerUpdate = onUpdate;
            onlyBeginUpdateActionAfterTime = updateCallAfterTime;
            timer = afterTime;
            cancelID = id;
        }
        //...
        public void SetProperties(float afterTime,Action doAction,int id)
        {
            e = doAction;
            actionPerUpdate = null;
            timer = afterTime;
            cancelID = id;
            clearMe=false;
        }
        public void SetProperties(float afterTime,Action doAction,Action onUpdate,int id)
        {

            e = doAction;
            actionPerUpdate = onUpdate;
            onlyBeginUpdateActionAfterTime = 0f;
            timer = afterTime;
            cancelID = id;
            clearMe=false;
        }
        public void SetProperties(float afterTime,Action doAction,Action onUpdate,float updateCallAfterTime,int id)
        {
            e = doAction;
            actionPerUpdate = onUpdate;
            onlyBeginUpdateActionAfterTime = updateCallAfterTime;
            timer = afterTime;
            cancelID = id;
            clearMe=false;
        }
        //*This Update is not handeled by unity*
        public void Update(float speedMod)
        {
            if (clearMe)
                return;
            if (timer <= 0)
            {
                e();
                clearMe = true;
            } else
            {
                
                if (actionPerUpdate != null)
                {
                    if(onlyBeginUpdateActionAfterTime<=0)actionPerUpdate();
                }
                timer -= speedMod*Time.deltaTime;
                if (onlyBeginUpdateActionAfterTime > 0f)
                    onlyBeginUpdateActionAfterTime -=speedMod* Time.deltaTime;
            }
        }
    }
}
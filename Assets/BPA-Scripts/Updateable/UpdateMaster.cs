using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UpdateMaster : MonoBehaviour 
{
	public List<IUpdateSlave> updateList= new List<IUpdateSlave>();
	public List<IUpdateSlave> lateUpdateList= new List<IUpdateSlave>();
    public List<IUpdateSlave> fixedUpdateList= new List<IUpdateSlave>();
	public IUpdateSlave[] updateArrayForSpeed={};
	public IUpdateSlave[] lateUpdateArrayForSpeed={};
    public IUpdateSlave[] fixedUpdateArrayForSpeed={};
    private static UpdateMaster instance;

	public int numberInUpdate=0;
	public int numberInLateUpdate=0;
	public int numberInFixedUpdate=0;
    // Always make an instance if object not found.
    public static UpdateMaster GetInstance()
    {
        if (applicationIsQuitting == true)
            return null;
        if (instance == null)
        {
            GameObject updateMasterGo = new GameObject("/UpdateMaster/");
            updateMasterGo.AddComponent<UpdateMaster>();
            instance = updateMasterGo.GetComponent<UpdateMaster>();
			if(Application.isPlaying) DontDestroyOnLoad(instance.gameObject);
        }
        return instance;
    }
    private static bool applicationIsQuitting = false;
    public void OnDestroy()
    {
        //needed to stop error when closing game.
        applicationIsQuitting = true;
    }
    //Register an object
	//UPDATE:
    public static void Register(IUpdateSlave updateObject)
    {
        UpdateMaster.GetInstance();
        bool alreadyRegistered = false;
      
        for (int i = 0; i < instance.updateList.Count; i++)
        {
            if (instance.updateList[i] == updateObject)
            {
                alreadyRegistered = true;
                return;
            }
        }
        if (alreadyRegistered == false)
        {
            instance.updateList.Add(updateObject);
        }

        /*
         * to quote somone from unity:
         * "Another thing is that Lists are just slower or 
         * more precisely our Mono compiler generates slower
         * code for them. I ran the test with array on Mono a
         * nd got 0.23ms (was 0.52ms with List) vs. 0.22ms in IL2CPP."
        */

        //keep a copy of the array and the list and convert the list to an array whenever it is changed.
        //do this because looping over an array is "faster in IL2CPP"
        instance.updateArrayForSpeed = instance.updateList.ToArray();
		instance.numberInUpdate = instance.updateArrayForSpeed.Length;
    }
	public static void Deregister(IUpdateSlave updateObject)
	{
		UpdateMaster.GetInstance();
		bool alreadyRegistered = false;

		for (int i = 0; i < instance.updateList.Count; i++)
		{
			if (instance.updateList[i] == updateObject)
			{
				alreadyRegistered = true;
				break;
			}
		}
		if (alreadyRegistered == true)
		{
			instance.updateList.Remove(updateObject);
		}
		//keep a copy of the array and the list and convert the list to an array whenever it is changed.
		//do this because looping over an array is "faster in IL2CPP"
		instance.updateArrayForSpeed = instance.updateList.ToArray();
		instance.numberInUpdate = instance.updateArrayForSpeed.Length;

	}
	//LATE UPDATE:
	public static void LateRegister(IUpdateSlave updateObject)
	{
		UpdateMaster.GetInstance();
		bool alreadyRegistered = false;
		for (int i = 0; i < instance.lateUpdateList.Count; i++)
		{
			if (instance.lateUpdateList[i] == updateObject)
			{
				alreadyRegistered = true;
				return;
			}
		}
		if (alreadyRegistered == false)
		{
			instance.lateUpdateList.Add(updateObject);
		}
		instance.lateUpdateArrayForSpeed = instance.lateUpdateList.ToArray();
		instance.numberInLateUpdate = instance.lateUpdateArrayForSpeed.Length;
	}
	public static void LateDeregister(IUpdateSlave updateObject)
	{
		UpdateMaster.GetInstance();
		bool alreadyRegistered = false;

		for (int i = 0; i < instance.lateUpdateList.Count; i++)
		{
			if (instance.lateUpdateList[i] == updateObject)
			{
				alreadyRegistered = true;
				break;
			}
		}
		if (alreadyRegistered == true)
		{
			instance.lateUpdateList.Remove(updateObject);
		}
		instance.lateUpdateArrayForSpeed = instance.lateUpdateList.ToArray();
		instance.numberInLateUpdate = instance.lateUpdateArrayForSpeed.Length;
	}
	//FIXED UPDATE:
	public static void FixedRegister(IUpdateSlave updateObject)
	{
		UpdateMaster.GetInstance();
		bool alreadyRegistered = false;
		for (int i = 0; i < instance.fixedUpdateList.Count; i++)
		{
			if (instance.fixedUpdateList[i] == updateObject)
			{
				alreadyRegistered = true;
				return;
			}
		}
		if (alreadyRegistered == false)
		{
			instance.fixedUpdateList.Add(updateObject);
		}
		instance.fixedUpdateArrayForSpeed = instance.fixedUpdateList.ToArray();
		instance.numberInFixedUpdate = instance.fixedUpdateArrayForSpeed.Length;
	}
	public static void FixedDeregister(IUpdateSlave updateObject)
	{
		UpdateMaster.GetInstance();
		bool alreadyRegistered = false;

		for (int i = 0; i < instance.fixedUpdateList.Count; i++)
		{
			if (instance.fixedUpdateList[i] == updateObject)
			{
				alreadyRegistered = true;
				break;
			}
		}
		if (alreadyRegistered == true)
		{
			instance.fixedUpdateList.Remove(updateObject);
		}
		instance.fixedUpdateArrayForSpeed = instance.fixedUpdateList.ToArray();
		instance.numberInFixedUpdate = instance.fixedUpdateArrayForSpeed.Length;
	}
	// Update is called once per frame
	void Update () 
    {
        for (int i = 0; i < instance.updateArrayForSpeed.Length; i++)
        {
			// if you get en error here, it probably doesn't have the interface
            if(instance.updateArrayForSpeed[i]!=null)instance.updateArrayForSpeed[i].DoUpdate();
        }
	}
	void LateUpdate()
	{
		for (int i = 0; i < instance.lateUpdateArrayForSpeed.Length; i++)
		{
			// if you get en error here, it probably doesn't have the interface
            if(instance.lateUpdateArrayForSpeed[i]!=null)instance.lateUpdateArrayForSpeed[i].DoUpdate();
		}
	}
	void FixedUpdate()
	{
		
		for (int i = 0; i < instance.fixedUpdateArrayForSpeed.Length; i++)
		{
			// if you get en error here, it probably doesn't have the interface
            if(instance.fixedUpdateArrayForSpeed[i]!=null)instance.fixedUpdateArrayForSpeed[i].DoUpdate();
		}

	}
}
public interface IUpdateSlave
{
    // interface members
    void DoUpdate();
  
}
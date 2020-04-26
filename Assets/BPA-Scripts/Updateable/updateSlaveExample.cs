using UnityEngine;
using System.Collections;

public class updateSlaveExample : MonoBehaviour,IUpdateSlave 
{
    //Variable
    public string printString;
    //Implement interface
    #region IUpdateSlave implementation
    public void DoUpdate()
    {
        print(printString);
    }
    #endregion
    //register/un-register
    void OnEnable()
    {
        UpdateMaster.Register(this as IUpdateSlave);
    }
    void OnDisable()
    {
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
    void OnDestroy() 
    {
        //you must make sure that you unregister the object if the object is destroyed, or else update will be called even if the object no longer exists!
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
	
}

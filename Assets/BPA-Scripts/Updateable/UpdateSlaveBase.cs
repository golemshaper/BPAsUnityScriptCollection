using UnityEngine;
using System.Collections;

public class UpdateSlaveBase : MonoBehaviour,IUpdateSlave
{
    /*
     * Use this when you want a quick and easy way of adding the 
     * update slave funtionality and don't want to implement the 
     * interface yourself.
    */

    #region IUpdateSlave implementation
    public virtual void DoUpdate()
    {
        //todo: update here.
    }
    #endregion
    //register/deregister
    void OnEnable()
    {
        UpdateMaster.Register(this as IUpdateSlave);
    }
    void OnDisable()
    {
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
    void OnDestroy() {
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
	
}

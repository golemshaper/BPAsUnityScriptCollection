using UnityEngine;
using System.Collections;

public class CopyPosition : MonoBehaviour,IUpdateSlave {

    public Transform from;
    public bool usePlayer;
    public Transform to;
    Vector3 coordinates=Vector3.zero;
   
    public bool x,y,z;
    [SerializeField] bool onlyOnEnable;
    #region IUpdateSlave implementation
    public void DoUpdate()
    {
        coordinates = to.position;
        if (x)
            coordinates.x = from.transform.position.x;
        if (y)
            coordinates.y= from.transform.position.y;
        if (z)
            coordinates.z = from.transform.position.z;
        to.transform.position = coordinates;
    }
    #endregion
    void Awake()
    {
        if (to == null)
            to = this.transform;
        if(usePlayer)
        {
            if (GameManager.instance == null) return;
            from = GameManager.instance.Player.transform;
        }
       
    }
    void OnEnable () {
        if (onlyOnEnable)
        {
            DoUpdate();
            return;
        }

        UpdateMaster.LateRegister(this as IUpdateSlave);
    }
    void OnDisable () {
        if (onlyOnEnable)
        {
            return;
        }
        UpdateMaster.LateDeregister(this as IUpdateSlave);
    }
    void OnDestroy () {
        if (onlyOnEnable)
        {
            return;
        }
        UpdateMaster.LateDeregister(this as IUpdateSlave);
    }
}

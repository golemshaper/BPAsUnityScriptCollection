using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour,IUpdateSlave
{
	public static PlayerInput instance;
	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
			instance = this;
	}
	public bool GetFire1_B()
	{
		return fire1;
	}
    public bool GetHoldFire1_B()
    {
        return fire1Hold;
    }
    public bool GetFire2_A()
    {
        return fire2;
    }
    public bool GetHoldFire2_A()
    {
        return fire2Hold;
    }
    public bool GetFire3_Y()
    {
     
        return fire3;
    }
    public bool GetHoldFire3_Y()
    {
        return fire3Hold;
    }
    public bool GetFire4_X()
    {
       // if (fire4) Debug.Log("X?");
        return fire4;
    }
    public bool GetHoldFire4_X()
    {
        return fire4Hold;
    }
	public Vector2 GetMovement()
	{
		return move;
	}
    public bool GetL1()
    {
        return L1;
    }
	public Vector2 GetMovement(bool joystickPlusDPad)
	{
		return move;
	}
    public Vector2 GetMovementRaw(bool joystickPlusDPad)
    {
        return rawMove;
    }
    bool fire1;
    bool fire1Hold;
    bool fire2;
    bool fire2Hold;
    bool fire3;
	bool fire3Hold;
    bool fire4;
    bool fire4Hold;
    bool L1;
    bool L2Hold;
    bool L2;
    bool R2Hold;
    bool R2;
    bool R1;
    bool R1Hold;
    Vector2 move;
    Vector2 rawMove;
    const string TriggerL2 = "TriggerL2";
    const string TriggerR2 = "TriggerR2";
  /*  public bool GetL2()
    {
        if(Input.GetAxis(TriggerL2)>0.5f)
        {
            return true;
        }
        return false;
    }
    bool lastL2State = false;
   public bool GetL2Down()
    {
        if(lastL2State!=GetL2())
        {
            //axis 9
            lastL2State = GetL2();
            return lastL2State;
        }
        return false;
    }
    */
    public bool GetR3()
    {
        return Input.GetKeyDown(KeyCode.Joystick1Button9);
    }
  /*  public bool GetR2()
    {
        if(Input.GetKey(KeyCode.F))
        {
            return true;
        }
        if (Input.GetAxis(TriggerR2) > 0.5f)
        {
            return true;
        }
        return false;
    }*/
    public bool GetR1()
    {
        return R1;
    }
    public bool GetR1Hold()
    {
        return R1Hold;
    }
    bool lastR2State = false;
   /* public bool GetR2Down()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            return true;
        }
        if (lastR2State != GetR2())
        {
            //axis 9
            lastR2State = GetR2();
            return lastR2State;
        }
        return false;
    }*/

    #region IUpdateSlave implementation
    public void DoUpdate()
    {
        fire1 = Input.GetButtonDown ("Fire1");
        fire1Hold = Input.GetButton ("Fire1");
        fire2 = Input.GetButtonDown ("Fire2")||Input.GetKeyDown(KeyCode.Space);
        fire2Hold = Input.GetButton ("Fire1");
        fire3 = Input.GetButtonDown ("Fire3");
        fire3Hold = Input.GetButton ("Fire3");
        //fire4 = Input.GetButtonDown ("Fire4") || Input.GetKeyDown(KeyCode.JJoystickButton3);
      //  fire4Hold = Input.GetButton ("Fire4");
        L1=Input.GetKeyDown(KeyCode.JoystickButton4)||Input.GetKeyDown(KeyCode.R);
        R1 = Input.GetKeyDown(KeyCode.JoystickButton5)||Input.GetKeyDown(KeyCode.T);
        R1Hold = Input.GetKey(KeyCode.JoystickButton5)||Input.GetKey(KeyCode.T);
       // L2 = GetL2Down();//|| Input.GetKeyDown(KeyCode.Q);
       // L2Hold = GetL2();

       // R2 = GetR2Down() || Input.GetKeyDown(KeyCode.E);
       // R2Hold = GetR2() || Input.GetKey(KeyCode.E);

        move.Set (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
        rawMove.Set(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
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

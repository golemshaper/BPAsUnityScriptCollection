using UnityEngine;
using System.Collections;

public class PlayerMovementBPA : MonoBehaviour,IUpdateSlave
{
	//Public:
	[Header("Settings:")]
    public bool autoUpdate = true;
	public float speed = 7.0F;
    public float speedBoost = 1f;

	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	public float rotationalSpeed=10f;
    public bool enableRotation=true;
    public bool enableLean=false;
    public float leanAmount = 1f;
	public bool enableJumpButton=true;
	public bool enableDoubleJump=true;
    public bool enableAutoJump=false;
    public bool enableInAirMovement;
    public bool zeroGravity;
    public bool useLadderControls=false;
    public bool disableLeftAndRightLadder=false;
    public float respawnAtY = -10f;
    public bool lockHorizontalMovement=false;
    public bool lockVerticalMovement=false;
    [Header("Runtime tweaks:")]
    public bool lockInput; //pretend the stick is held at last input val.
    Vector2 lockedInputVal = Vector2.zero;
    Vector2 lockedInputValRaw = Vector2.zero;
    public bool lazySteer = false;
    Vector2 lazySteerVector=Vector2.zero;
    [Header("ComponentReferences:")]
	//needs to be assigned:
	public Transform rotationObj;
	//will auto-assign:
	public CharacterController controller;
	public Transform cam;
	public Transform CameraPosition;
    public Transform myTransform;
    //Private:
    //bool isFalling = false;
    Vector3 moveDirection;
     
   // Vector3 lastMoveDirection;
	Vector3 lastMoveDirectionRaw;
    Vector2 additionalInput=Vector3.zero;
    public Vector2 GetAdditionalInputValue()
    {
       return additionalInput;
    }
    public void SetMoveDirection(Vector3 nMoveDirection)
    {
        moveDirection = nMoveDirection;
    }
    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }
	Quaternion rotation = new Quaternion (0, 0, 0, 0);
	bool hasDoubleJumped;
    public void SetDirection(Vector3 angleVector)
    {
        lastMoveDirectionRaw = angleVector;

      
        Vector3 moveDirCopy = angleVector;
        moveDirCopy.y = 0;
        rotation = Quaternion.LookRotation(moveDirCopy);
        rotationObj.rotation = Quaternion.Slerp(rotationObj.rotation, rotation,1f);


    }
    public bool ignoreInput=false;
    public Vector3 respawnLocation;

    float start_moveRadius;
    float start_moveHeight;
    private void Awake()
    {
        start_moveRadius = controller.radius;
        start_moveHeight = controller.height;

    }
    /// <summary>
    /// SetCustomSize
    /// </summary>
    /// <param name="radi"></param>
    /// <param name="height"></param>
    public void SetControlerSize(float radi,float height)
    {
        controller.radius = radi;
        controller.height = height;
    }
    /// <summary>
    /// Restore default size
    /// </summary>
    public void SetControlerSize()
    {
        controller.radius = start_moveRadius;
        controller.height = start_moveHeight;
    }
    public Vector3 GetLastMoveDirection()
    {
        return lastMoveDirectionRaw;
    }
    public Vector3 GetMoveingSpeed()
    {
        return controller.velocity;
    }
    public bool HasUsedDoubleJump()
    {
        return hasDoubleJumped;
    }
    public void ReturnDoubleJump()
    {
        hasDoubleJumped = false;
    }
    public void Jump()
    {
        requestJump = true;

        //moveDirection.y = jumpSpeed;
    }
    
    public bool isFalling()
    {
        if(controller.velocity.y<0)
        {
            return true;
        }
        return false;
    }
	// Use this for initialization
	void Start () 
	{
		InitializeMovement ();
        if(myTransform==null) myTransform = controller.transform;
        respawnLocation = myTransform.position;
        lastMoveDirectionRaw = new Vector3(0, 1, 0);

	}
    public void SetSpawnPoint(Vector3 spawnPoint)
    {
        respawnLocation = spawnPoint;
    }
   
    public float minimumWalk = 0.2f;
    public bool isWalking()
    {
        if (GetMovement().magnitude > minimumWalk)
            return true;
        return false;
    }
	public void InitializeMovement()
	{
		if(Camera.main==null)return;
		if(CameraPosition==null)
		{
			CameraPosition=Camera.main.transform;
		}
		controller = GetComponent<CharacterController> ();
		cam = Camera.main.transform;
	}
    // Use this for initialization
    void OnEnable () {
        if (hasRegistered)
            return;
        if (!autoUpdate)
            return;
        hasRegistered=true;
        UpdateMaster.Register(this as IUpdateSlave);
    }
    void OnDisable () {
        if (!hasRegistered)
            return;
        if (!autoUpdate)
            return;
        hasRegistered=false;
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
    void OnDestroy () {
        if (!hasRegistered)
            return;
        if (!autoUpdate)
            return;
        hasRegistered=false;
        UpdateMaster.Deregister(this as IUpdateSlave);
    }
    bool hasRegistered=false;

    #region IUpdateSlave implementation

    public bool isGrounded()
    {
        return controller.isGrounded;
    }
    public bool ignoreUpdateCall=false;
    public void DoUpdate()
    {
        if (ignoreUpdateCall) return;

        MovmentUpdate ();
      
        CheckCeiling(true);
       
    }
    public Vector3 GetMovementHypothetically()
    {
        if (Camera.main != null && lockCam == false)
        {
            CameraPosition = Camera.main.transform;
            //the normal way it works in movement. i'm leaving it here as an example only. it will be overridden on the next line...
            if (cam != null) forward = cam.TransformDirection(Vector3.forward);
            //Use forward of player instead!
            forward.y = 0;
            forward = forward.normalized;
            right = new Vector3(forward.z, 0, -forward.x);
        }
        else
        {
            UpdateMainCamera();
        }
        float h = GetMovement().x;
        float v = GetMovement().y;

        //------------------------------------------------------------------
        //Fake movement calculated like the actual move based on the camera orientation...
        Vector3 hypotheticalMoveDir = Vector3.zero;
        float yP = hypotheticalMoveDir.y;

        hypotheticalMoveDir = (h * right + v * forward);
        if (hypotheticalMoveDir.sqrMagnitude > 1)
        {
            hypotheticalMoveDir.Normalize();
        }
        hypotheticalMoveDir *= (speed * speedBoost);
        hypotheticalMoveDir.y = yP;
        return hypotheticalMoveDir;
        //------------------------------------------------------------------
    }
   
    GameObject lastWallKickedOff;
    /// <summary>
    /// If slideing against a wall, fall slower. Allows a wall jump to take place 
    /// </summary>
   
   public bool isSlidingOnWall = false;
    public bool CheckWallSlide()
    {
        //Keep track of the object that you kicked off of. 
        //Never let the player kick off of that last known object until they hit the ground again,
        //or the kick another object!!

        return isSlidingOnWall;
    }
    Vector3 ceilingCheckOffset = new Vector3(0, 1.82f, 0);
    public bool CheckCeiling(bool autoSendToFloor)
    {
        return CheckCeiling(autoSendToFloor, 0.2f);
    }
    public bool CheckCeiling( bool autoSendToFloor,float checkDist)
    {
        Vector3 forwardOffset = rotationObj.forward * 0.4f ;
        //with front offset:
        Ray hit = new Ray(myTransform.position+ ceilingCheckOffset+ forwardOffset, myTransform.up);
        bool results= Physics.Raycast(hit, checkDist);
        //without front offset.
        hit = new Ray(myTransform.position + ceilingCheckOffset, myTransform.up);
        bool results2 =  Physics.Raycast(hit, checkDist);

        if (results || results2)
        {
           if(autoSendToFloor) moveDirection.y = -1f;
            //Debug.Log("hit Ceiling");
            return true;
        }
        return false;
    }
    #endregion
    float horizontalAxis=0f;
	float verticalAxis=0f;

    bool littleHop;

    //...
    Vector3 right = new Vector3(0, 0, 0);
    Vector3 forward = new Vector3(0, 0, 0);
    bool lockCam=false;
    public void LockMovementCameraOrientation(bool doLock)
    {
        lockCam = doLock;
    }
    public bool IsCameraLocked()
    {
        return lockCam;
    }
    void MovmentUpdate()
	{
		//Standard movement update...

		//Get Camera angle
		if(Camera.main!=null && lockCam==false)
		{
            CameraPosition =Camera.main.transform;
            if (cam!=null)	forward = cam.TransformDirection(Vector3.forward);
            forward.y = 0;
			forward = forward.normalized;
			right  = new Vector3(forward.z, 0, -forward.x);
           
        }
		else 
		{
			UpdateMainCamera();
		}
		//remember last movedirection:
	//	lastMoveDirection=  moveDirection;
        if (GetMovementRaw().magnitude > 0)
        {
            lastMoveDirectionRaw = GetMovementRaw();
        }
        //GetMovement From input values
        horizontalAxis =GetMovement().x;
       
		verticalAxis=GetMovement().y;
     
        bool canAcceptMovementInput = controller.isGrounded;
        if (enableInAirMovement)
            canAcceptMovementInput = true;
        if (canAcceptMovementInput)
        {
           
            float yP = moveDirection.y;
            //if is on the floor...
            moveDirection = (horizontalAxis * right + verticalAxis * forward);
            
            if (moveDirection.sqrMagnitude > 1)
            {
                moveDirection.Normalize();
            }
           

            moveDirection *= (speed*speedBoost);
            moveDirection.y = yP;
        }
        if (controller.isGrounded) 
        {
            hasDoubleJumped = false;

            littleHop = false;
			if (enableJumpButton) 
			{
                if (GetJumpInput()) 
				{
					moveDirection.y = jumpSpeed;
                    littleHop = true; //ignore the next little hop
				}
			}
           
		} 
		else 
		{
            if (!littleHop)
            {
                littleHop = true;
                if (enableAutoJump)
                {
                    moveDirection.y = jumpSpeed;
                }
                else
                {
                    //this is needed so that the player doesn't snap to the ground!
                   // moveDirection.y = jumpSpeed*0.2f;
                    moveDirection.y = jumpSpeed*0.2f;

                }
            }
         
			//if not on the floor...
			//double jump if enabled
			if (enableDoubleJump && zeroGravity==false) 
			{
				if (GetJumpInput() && hasDoubleJumped == false) 
				{
					moveDirection.y = jumpSpeed;
					hasDoubleJumped = true;
				}
			}
		}
		//apply gravity
        if (zeroGravity == false)
            moveDirection.y -= gravity * Time.deltaTime;
        else
            moveDirection.y = 0;
        //Fix fall glitch by setting max fall speed to gravity!
        if (moveDirection.y < gravity * -1) moveDirection.y = gravity * -1;

        //DO CLIMB
        if (useLadderControls)
        {
            //please at some poit, snap rotation to look at facing direction...
            moveDirection.y = moveDirection.z;
            //don't zero it out, so that it the player moves towards the platform.... 
            //move towards camera angle.
            //  moveDirection.z = 0f;
            if (controller.isGrounded == false)
            {
                moveDirection.z = forward.z;
            }
         
            RotationLookAtTarget(forward);
            if (GetMovement().magnitude < minimumWalk)
            {
               //make move vector too small to move, but not 0.
                moveDirection.x *= 0.001f;
                moveDirection.z *= 0.001f;
            }
            if (lockHorizontalMovement)
            {
                //  results.x = 0;
                if (HasMovedOppisiteOfFacingAngle())
                {

                    OnWallBreakOffClimb();
                }
                else
                {
                    moveDirection.x = 0;
                    moveDirection.z = 0;

                }

            }

            controller.Move(moveDirection * 0.5f * Time.deltaTime);

            return;
        }

        //apply movement
        if (GetMovement().magnitude < minimumWalk)
        {
            //make move vector too small to move, but not 0.
            moveDirection.x *= 0.001f;
            moveDirection.z *= 0.001f;
        }
        if (lockHorizontalMovement)
        {
            //  results.x = 0;
            if(HasMovedOppisiteOfFacingAngle())
            {
               
                OnWallBreakOffClimb();
            }
            else
            {
                moveDirection.x = 0;
                moveDirection.z = 0;

            }
          
        }
       
        controller.Move(moveDirection * Time.deltaTime);

        if (GetMovement().sqrMagnitude != 0f)
        {
            //rotate rotational object if input is not 0...
            UpdateRotationObject();
            if (enableLean)
            {
                UpdateLean(leanAmount);
            }
        }
        else  if (ignoreInput && enableLean)
        {
            //ignore lean.
            rotation = rotationObj.rotation;
            rotation = Quaternion.Euler(0, rotationObj.rotation.eulerAngles.y, rotationObj.rotation.eulerAngles.z);
            rotationObj.rotation = rotation;
        }
     
		//TODO: Add Animation hooks...
		//TODO: Add state options for ignoring user input etc.	
		//TODO: Add climbing.
	}
    bool HasMovedOppisiteOfFacingAngle()
    {
        Vector2 moveVal2D = GetMovement();
        Vector3 moveVal3D = new Vector3(moveVal2D.x, 0, moveVal2D.y);
        moveVal3D.Normalize();
        Vector3 facingRot = rotationObj.forward;
        facingRot.y = 0f;
        facingRot=facingRot * 500;
        facingRot.Normalize();

       // DEBUG_ANGLE_STICK = moveVal3D;
       // DEBUG_ANGLE_FACE = facingRot;
        
       if(Vector3.Distance(moveVal3D,facingRot)<=0.5f)
       {
           return true;
       } 

        return false;
    }
    System.Action doOnBreakOffWallClimb;
    public void SubscribeToOnWallBreakOff(System.Action a)
    {
        doOnBreakOffWallClimb = a;
    }
    /// <summary>
    /// Stop climbing on wall
    /// </summary>
    void OnWallBreakOffClimb()
    {
        doOnBreakOffWallClimb();
    }
    //public Vector3 DEBUG_ANGLE_STICK;
    // public Vector3 DEBUG_ANGLE_FACE;

    void UpdateRotationObject()
    {
        if (rotationObj == null)
            return;
        if (enableRotation == false)
            return;
        /*  if (isGrounded() == false)
       {
           if (moveDirection.y < 0) return;
       }*/
        Vector3 moveDirCopy = moveDirection;
		moveDirCopy.y = 0;
        rotation = Quaternion.LookRotation(moveDirCopy);

        if (moveDirection.magnitude < minimumWalk)
        {
            
          //  return;
        }

        rotationDirection = AngleBetweenQuaternion(rotationObj.forward, moveDirCopy);

        rotationObj.rotation = Quaternion.Slerp (rotationObj.rotation,rotation,Time.deltaTime*rotationalSpeed);
      
        // storeDirectionWithoutLean = rotationObj.rotation;

    }
    public void SetRotationToTarget()
    {
        if (enableRotation == false)
            return;
        Vector3 moveDirCopy = moveDirection;
        moveDirCopy.y = 0;
        rotation = Quaternion.LookRotation(moveDirCopy);
        rotationDirection = AngleBetweenQuaternion(rotationObj.forward, moveDirCopy);
        rotationObj.rotation = Quaternion.Slerp(rotationObj.rotation, rotation, 1f);
    }
    float rotationDirection=0f;
    public float GetRotationDirection()
    {
        return rotationDirection;
    }
    public float AngleBetweenQuaternion(Vector3 forwardA, Vector3 forwardB)
    {
        // get a "forward vector" for each rotation
      //  var forwardA = rotationA * Vector3.forward;
      //  var forwardB = rotationB * Vector3.forward;


        // get a numeric angle for each vector, on the X-Z plane (relative to world forward)
        var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
        var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;


        // get the signed difference in these angles
        var angleDiff = Mathf.DeltaAngle(angleA, angleB);
        return angleDiff;
    }
    public Quaternion GetRotation()
    {
        if (rotationObj == null) return Quaternion.identity;
        return rotationObj.rotation;
    }
 //   Quaternion storeDirectionWithoutLean=Quaternion.identity;
    void UpdateLean(float lean)
    {
        /*  if (isGrounded() == false)
        {
            if (moveDirection.y < 0) return;
        }*/
        //TODO: Lean into turn.
        Vector3 moveDirCopy = moveDirection;
        moveDirCopy.y = (GetMovement().sqrMagnitude*-1)*lean;
        rotation = Quaternion.LookRotation (moveDirCopy);
     
        rotationObj.rotation = Quaternion.Slerp (rotationObj.rotation,rotation,Time.deltaTime*rotationalSpeed);
    }
 
    public void RotationLookAtTarget(Vector3 targetPos)
    {
        if (rotationObj == null)
            return;
        if (enableRotation == false)
            return;
      /*  if (isGrounded() == false)
        {
            if (moveDirection.y < 0) return;
        }*/
        Vector3 moveDirCopy = targetPos;
        moveDirCopy.y = 0;
        if (moveDirection.magnitude == 0)
            return;
        rotation = Quaternion.LookRotation (moveDirCopy);
      
        rotationObj.rotation = Quaternion.Slerp (rotationObj.rotation,rotation,Time.deltaTime*rotationalSpeed);
    }

	public void UpdateMainCamera()
	{
		if(Camera.main==null)return;
		cam = Camera.main.transform;
		CameraPosition=cam;


	}
	public void UpdateMainCamera(Camera newCam)
	{
		if(newCam==null)return;
		cam = newCam.transform;
		CameraPosition=cam;

	}
    public void CancelJumpOrFall()
    {
        moveDirection.y = 0f;
    }
    public void CancelJumpOrFall(float setYValue)
    {
        moveDirection.y = setYValue;
    }
    public void SimulateJumpInput()
    {
        simulateJumpInput = true;
    }
    public void SimulateJumpInput(bool useRequest)
    {
        simulateJumpInput = true;
        if(useRequest)
        {
            requestJump = true;
        }
    }
    bool simulateJumpInput = false;
	public bool GetJumpInput(bool ignoreInputIgnore)
	{
        if (enableJumpButton == false) return false;
        if (CheckCeiling(false,1f))
        {
            //no jump if ceiling is low
            return false;
        }
        if(requestJump)
        {
            requestJump = false;
            return true;
        }
       
        if (PlayerInput.instance == null)
            return false;
        if(ignoreInputIgnore==false)
        {
            if (ignoreInput == true)
            {
                return false;
            }
        }

        if (simulateJumpInput)
        {
            simulateJumpInput = false;
            return true;
        }
        return PlayerInput.instance.GetFire2_A ();
	}
    public bool GetJumpInput()
    {
        return GetJumpInput(false);
    }
    bool requestJump;
    //disableLeftAndRightLadder
    public Vector2 GetMovement()
    {
        if (PlayerInput.instance == null) 
        {
            return Vector2.zero;
        }
        if (ignoreInput)
        {
            return Vector2.zero+additionalInput;
        }
        Vector2 inputFromStick = PlayerInput.instance.GetMovement(false);
       
        if (disableLeftAndRightLadder)
        {
            //this is a hack I know...but I'll deal with it later I hope...
            inputFromStick.x = 0f;
        }
        Vector2 results = inputFromStick + additionalInput;
        if (lockInput)
        {
            return lockedInputVal;
        }
        else
        {
            lockedInputVal = results;
        }
        if(lazySteer)
        {
            lazySteerVector = Vector2.Lerp(lazySteerVector, results, 0.2f*Time.deltaTime);
            results = lazySteerVector;
        }
        else
        {
            lazySteerVector = results;
        }

        
        return results;
    }

    public Vector2 GetMovementRaw()
    {
        if (PlayerInput.instance == null) 
        {
            return Vector2.zero;
        }
        if (ignoreInput)
        {
            return Vector2.zero;
        }
        if (lockInput)
        {
            return lockedInputValRaw;
        }
        else
        {
            lockedInputValRaw= PlayerInput.instance.GetMovementRaw(false);
           
        }
       
        return PlayerInput.instance.GetMovementRaw(false);
    }
   
    /// <summary>
    /// Simulate input.
    /// </summary>
    /// <param name="inputValue">Input value.</param>
    public void SimulateInput(Vector2 inputValue)
    {
        additionalInput = inputValue;
    }
    /// <summary>
    /// Repeat last movement input
    /// </summary>
    public void SimulateInput()
    {
        SimulateInput(1f);
    }
    public void SimulateInput(float amplify)
    {
        
        lastMoveDirectionRaw = lastMoveDirectionRaw * 100f;
        lastMoveDirectionRaw.Normalize();
        additionalInput = lastMoveDirectionRaw*amplify;

    }

  
    public void ReverseLastMove()
    {
        lastMoveDirectionRaw = lastMoveDirectionRaw * -1f ;
    }
    
}

using UnityEngine;
using System.Collections;

public class SinCosRotation : MonoBehaviour,IUpdateSlave {

	public bool x;
	public bool y;
	public bool z;
	Vector3 CaptureOriginalEularAngles=new Vector3(0,0,0);
	Transform myTransform;

	public bool useCos;
	public bool useAbsoluteValue=false;
	public float speedMultiply=1f;
	public bool useBlenderCorrection;
	public Vector3 correctionAxis=new Vector3(270f,0,0);
	public bool randomSelectSinOrCosOnStart=false;
	Rigidbody rb;
	// Use this for initialization
	public bool addToInitialValue;
	void Awake () 
	{
		rb = this.GetComponent<Rigidbody>();
		if (rb == null)
		{
			rb=this.gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
		}
		myTransform=this.GetComponent<Transform>();
		CaptureOriginalEularAngles=myTransform.localEulerAngles;
		if(randomSelectSinOrCosOnStart)
		{
            useCos= GameMath.GetRandomBool();
		}
	}
	float sinVal=0f;
	public float multiplyEffect=1f;
	public bool limitUpdate=false;
	public float updateEveryXSeconds;
	float curWaitTime=0f;
	// Update is called once per frame
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
	// Update is called once per frame
	public void DoUpdate()
	{
		if(useCos)
		{
			sinVal=Mathf.Cos(Time.time*speedMultiply);
		}
		else
		{
			sinVal=Mathf.Sin(Time.time*speedMultiply);
		}
		if(useAbsoluteValue)sinVal=Mathf.Abs(sinVal);
		float nx=CaptureOriginalEularAngles.x;
		float ny=CaptureOriginalEularAngles.y;
		float nz=CaptureOriginalEularAngles.z;
		
		if(addToInitialValue)
		{
			if(x)nx+=sinVal*multiplyEffect;
			if(y)ny+=sinVal*multiplyEffect;
			if(z)nz+=sinVal*multiplyEffect;
		}
		else
		{
			if(x)nx=sinVal*multiplyEffect;
			if(y)ny=sinVal*multiplyEffect;
			if(z)nz=sinVal*multiplyEffect;
		}
		
		if(limitUpdate)
		{
			if(curWaitTime<updateEveryXSeconds)
			{
				curWaitTime+=Time.deltaTime;
				return;
			}
			else
			{
				curWaitTime=0f;
			}
		}
		if(useBlenderCorrection)
		{
			Vector3 correctForBlender= new Vector3(nx,ny,nz);
			correctForBlender+=correctionAxis;
			myTransform.localEulerAngles=correctForBlender;
			return;
		}
		myTransform.localEulerAngles= new Vector3(nx,ny,nz);
		
	}
}
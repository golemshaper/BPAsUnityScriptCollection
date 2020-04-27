using UnityEngine;
using System.Collections;

public class SinCosScale : MonoBehaviour,IUpdateSlave {

	public bool scaleX;
	public bool scaleY;
	public bool scaleZ;
	Vector3 captureOriginalTransformScale=new Vector3(0,0,0);
	Transform myTransform;
	public float scaleMod=1f;
	public bool useCos;

	public float speedMultiply=1f;
	// Use this for initialization
	public bool addToInitialValue;

	public float multiplyEffect=1f;

	public bool limitUpdate=false;
	public float updateEveryXSeconds;
	float curWaitTime=0f;
	public bool useAbsoluteValue=true;
	public bool randomSelectSinOrCosOnStart=false;
	void Awake () 
	{
		myTransform=this.GetComponent<Transform>();
		captureOriginalTransformScale=myTransform.localScale;
		if(randomSelectSinOrCosOnStart)
		{
            useCos= GameMath.GetRandomBool();
		}
	}
	float sinVal=0f;

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
		sinVal=Mathf.Sin(Time.time*speedMultiply);
		if(useCos)sinVal=Mathf.Cos(Time.time*speedMultiply);
		if(useAbsoluteValue)sinVal=Mathf.Abs(sinVal);

		float nx=captureOriginalTransformScale.x*scaleMod;
		float ny=captureOriginalTransformScale.y*scaleMod;
		float nz=captureOriginalTransformScale.z*scaleMod;
	
		if(addToInitialValue)
		{
			if(scaleX)nx+=(sinVal*multiplyEffect);
			if(scaleY)ny+=(sinVal*multiplyEffect);
			if(scaleZ)nz+=(sinVal*multiplyEffect);
		}
		else
		{
			if(scaleX)nx=(sinVal*multiplyEffect);
			if(scaleY)ny=(sinVal*multiplyEffect);
			if(scaleZ)nz=(sinVal*multiplyEffect);
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
		myTransform.localScale= new Vector3(nx,ny,nz);
			
	}
}

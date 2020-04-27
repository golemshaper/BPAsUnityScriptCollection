using UnityEngine;
using System.Collections;

public class SinCosPosition : MonoBehaviour,IUpdateSlave {
   
	public Transform transformOverride;
	// Use this for initialization
	float XOnStart=0f;
	void Start () {
		if(transformOverride==null)transformOverride=this.transform;
		if(useLocalPosition==false){
			if(lerpX)XOnStart=transformOverride.position.x;
			else if(lerpZ) XOnStart=transformOverride.position.z;
			else 
				XOnStart=transformOverride.position.y;
		}
		else
		{
			if(lerpX)XOnStart=transformOverride.localPosition.x;
			if(!lerpX)XOnStart=transformOverride.localPosition.y;
		}
		if (randomSelectSinOrCosOnStart)
		{
            useCos = GameMath.GetRandomBool();
		}

	}
	public float scaleFactorOfMovement=1f;
	public bool  useLocalPosition=false;
	// Update is called once per frame
	public float sinVal=0;
	public float cosVal=0;
	
	//float lerpValuex=0;

//	public float yMultiply=0.14f;
	public float timeMultiply=5.0f;
	
	public Vector3 offsetPosition=new Vector3(0,0,0);
	public bool useCos;

	public bool lerpX;
	public bool lerpZ;

	public bool useClampLessThenZero;
	public bool useClampGreaterThenZero;

	float localTimer=0f;
	public bool useCustomTime=false;
	public bool randomSelectSinOrCosOnStart=false;
	public bool stopMotionEffect;
	public int maxFrameBlendSkip=8;
	int limitFrameBlend=0;
	public void SetupCustomTime(float setStartTime)
	{
		useCustomTime=true;
		localTimer=setStartTime;
	}
	void OnEnable()
	{
		//fixed registration
		UpdateMaster.FixedRegister(this as IUpdateSlave);
	}
	void OnDisable()
	{
		UpdateMaster.FixedDeregister(this as IUpdateSlave);
	}
	void OnDestroy() 
	{
		//you must make sure that you unregister the object if the object is destroyed, or else update will be called even if the object no longer exists!
		UpdateMaster.FixedDeregister(this as IUpdateSlave);
	}
	//FixedUpdate
	public void DoUpdate () 
	{
		
		sinVal=Mathf.Sin(Time.time*timeMultiply);
		cosVal=Mathf.Cos(Time.time*timeMultiply);
		if(useCustomTime)
		{
			localTimer+=Time.deltaTime;
			if(localTimer>=10000)localTimer=0f;
			sinVal=Mathf.Sin(localTimer*timeMultiply);
			cosVal=Mathf.Cos(localTimer*timeMultiply);
		}
		float sinOrCos=sinVal;
		
		
		if(useCos)sinOrCos=cosVal;
	
		if(useClampLessThenZero)if(sinOrCos<0)return;
		if(useClampGreaterThenZero)if(sinOrCos>0)return;
		if (stopMotionEffect)
		{
			if (limitFrameBlend < maxFrameBlendSkip)
			{
				limitFrameBlend++;
				return;
			}
			limitFrameBlend = 0;
		}
		if (useLocalPosition == false) 
		{
			if (lerpX)
				transformOverride.position = new Vector3(sinOrCos * scaleFactorOfMovement + XOnStart, transformOverride.position.y, transformOverride.position.z)+offsetPosition;
			else if (lerpZ)
				transformOverride.position = new Vector3 (transformOverride.position.x, transformOverride.position.y, sinOrCos * scaleFactorOfMovement + XOnStart)+offsetPosition;
			else
				transformOverride.position = new Vector3 (transformOverride.position.x, sinOrCos * scaleFactorOfMovement + XOnStart, transformOverride.position.z)+offsetPosition;
		} else {
			if (lerpX)
				transformOverride.localPosition = new Vector3 (sinOrCos * scaleFactorOfMovement + XOnStart, transformOverride.localPosition.y, transformOverride.localPosition.z)+offsetPosition;
			else if (lerpZ)
				transformOverride.localPosition = new Vector3 (transformOverride.localPosition.x, transformOverride.localPosition.y,  sinOrCos * scaleFactorOfMovement + XOnStart)+offsetPosition;
			else
				transformOverride.localPosition = new Vector3 (transformOverride.localPosition.x, sinOrCos * scaleFactorOfMovement + XOnStart, transformOverride.localPosition.z)+offsetPosition;
		}
		//TODO: Make wave rotation script for grass blowing in wind....
	}
}

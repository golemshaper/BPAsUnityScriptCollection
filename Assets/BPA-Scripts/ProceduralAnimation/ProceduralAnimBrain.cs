using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ackk.Animation.Procedural;
using UnityEditor;

public class ProceduralAnimBrain : MonoBehaviour
{
    public bool executeInEditor;
    public ProcAnimSkeleton skeleton = new ProcAnimSkeleton();
    ProcAnimFunctions procAnim = new ProcAnimFunctions();
    // Start is called before the first frame update
    void Start()
    {
        skeleton.Initialize();
    }
  /*  TODO: this can work in the editor, but you need a copy of the default state! doin't use for now
   *  private void OnEnable()
    {
        if(executeInEditor)
        {
            EditorApplication.update += WalkCycle;
        }
    }
    private void OnDisable()
    {
       EditorApplication.update -= WalkCycle;
    }
    private void OnDestroy()
    {
       EditorApplication.update -= WalkCycle;
    }*/
    public float curTime;
    public float speed=2f;
    public float extream = 1f;
    public float bobPower=1f;
    public bool enableUpdate=true;
    // Update is called once per frame
    void Update()
    {
        WalkCycle();
    }

    //WALK:
    public Vector3 LegsXYZ = new Vector3(32f, 0, 0);
    public Vector3 ArmsXYZ = new Vector3(64f, 0, 0);
    public float armExtra = 8f;
    public Vector3 SpineXYZ = new Vector3(0, 8f, 0);
    public Vector3 BodyBobXZY = new Vector3(0, 2f, 0);
    void WalkCycle()
    {
        /*
         * TODO:
         * Instead of referencing the limb transforms, store in cutsom vectors and pass the results back using some sort of body wrapper class.
         * Use this class(that doesn't exist yet) to store the initial position and use it to blend animations...
         */
        //TODO: Move this in to a walk animation class!
        if(enableUpdate)curTime += speed * Time.deltaTime;

        //Root Bob: (Multiply cur time for double time bounce
        skeleton.Body.localPosition = new Vector3(skeleton.VectorDictinary[skeleton.Body].x, 
            skeleton.VectorDictinary[skeleton.Body].y 
            +(Mathf.Sin(curTime*2) * BodyBobXZY.y),
            skeleton.VectorDictinary[skeleton.Body].z) * bobPower;
        //LEGSW
        RotateLimbsForWalk(skeleton.LegL,LegsXYZ,false);
        RotateLimbsForWalk(skeleton.LegR, LegsXYZ, true);
        //SPINE
        skeleton.Body.localEulerAngles = procAnim.SinePumpAnim(curTime, Vector3.zero, SpineXYZ)* extream;
        skeleton.Head.localEulerAngles = procAnim.SinePumpAnim(curTime * -1f, Vector3.zero, SpineXYZ * 1.5f)* extream; //multiply head by X to compansate for parent rotation!
        //ARMS
        RotateLimbsForWalk(skeleton.ArmL, ArmsXYZ, true);
        RotateLimbsForWalk(skeleton.ArmR, ArmsXYZ, false);
    }
    public float powerChangeDownChain = 1f;

    void RotateLimbsForWalk(Transform[] Limbs,Vector3 XYZ,bool mirror)
    {
        //Limb variables:
        const float multOffset = -0.8f;
        const float offsetRightSide = -1.5f; //offsett animation in time
        float curOffset = 1f;
        int i = 0; //count current limb
        float timeShift = 0f;
        if (mirror)
        {
            curOffset = curOffset * -1f;
            timeShift = offsetRightSide;
        }
        float power = 1f;
        foreach (Transform t in Limbs)
        {
          
            Vector3 additional = Vector3.one;

            Vector3 modAngle = procAnim.SinePumpAnim((curTime + curOffset) + timeShift, skeleton.VectorDictinary[t], XYZ, power) * extream;
                
            if (i == 0) modAngle.y -= skeleton.Body.localEulerAngles.y; //subtract the rotation from the core
            t.localEulerAngles = modAngle+skeleton.VectorDictinary[t];
            curOffset += multOffset;
            //NOTE: I may be able to replace the cur offset with the powerChangeDownChain technique instead.
            //That would make it change by x amount down chain. that seems better.
            //try not to do both or this will get messy... A value of 0.8 would probably do it...?
            //if that fails, just use the offset I guess
            //the way the pendulom effect is done sometimes is to offset the keys in time. make sure that's what i'm doing...
            power *= powerChangeDownChain;
            i++;
        }
    }
    public Vector3 MultiplyVectorData(Vector3 a, Vector3 b)
    {
         a.x *= b.x;
         a.y *= b.y;
         a.z *= b.z;
        return a;
    }
}
namespace Ackk.Animation.Procedural
{
    [System.Serializable]
    public class ProcAnimSkeleton
    {
        //---
        [Header("Core:")]
        public Transform Head;
        public Transform Body;
        //---
        [Header("Left Arm:")]
        [Tooltip("Closest to body goes in first!")]
        public Transform[] ArmL;
        [Header("Right Arm:")]
        [Tooltip("Closest to body goes in first!")]
        public Transform[] ArmR;
        //---
        [Header("Left Leg:")]
        [Tooltip("Closest to body goes in first!")]
        public Transform[] LegL;
        [Header("Right Leg:")]
        [Tooltip("Closest to body goes in first!")]
        public Transform[] LegR;
        //---
        //Store initial Angle Vectors
        Vector3[] ArmL_Vectors;
        Vector3[] ArmR_Vectors;
        Vector3[] LegL_Vectors;
        Vector3[] LegR_Vectors;

        Vector3 RootPosition;

        public Dictionary<Transform, Vector3> VectorDictinary = new Dictionary<Transform, Vector3>();
        public void Initialize()
        {
            VectorDictinary.Clear();
            //Capture initial rotations
            ArmL_Vectors = CreateVectorArray(ArmL);
            ArmR_Vectors = CreateVectorArray(ArmR);
            LegL_Vectors = CreateVectorArray(LegL);
            LegR_Vectors = CreateVectorArray(LegR);
            //Special Transforms:
            RootPosition = Body.localPosition;
            VectorDictinary.Add(Body, RootPosition); //store body position instead of rotation, since its rotation is zero anyway

        }
        
        Vector3[] CreateVectorArray(Transform[] limbAry)
        {
            int arrayLen = 0;
            Vector3[] returnAry;
            arrayLen = limbAry.Length;
            returnAry = new Vector3[arrayLen];
            for (int i = 0; i < returnAry.Length; i++)
            {
                returnAry[i] = limbAry[i].localEulerAngles;
                VectorDictinary.Add(limbAry[i], returnAry[i]);
            }
            return returnAry;
        }
    }

    public class ProcAnimFunctions
    {
        public Vector3 SinePumpAnim(float progress, Vector3 initialVector, Vector3 XYZ)
        {
            Vector3 result = new Vector3();
            result.x = Mathf.Sin(progress) * XYZ.x;
            result.y = Mathf.Sin(progress) * XYZ.y;
            result.z = Mathf.Sin(progress) * XYZ.z;
            return result;
        }
        public Vector3 SinePumpAnim(float progress, Vector3 initialVector, Vector3 XYZ,float power)
        {
            Vector3 result = new Vector3();
            result.x = Mathf.Sin(progress) * XYZ.x;
            result.y = Mathf.Sin(progress) * XYZ.y;
            result.z = Mathf.Sin(progress) * XYZ.z;
            return result* power;
        }
    }
}

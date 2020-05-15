using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public static class GameMath
{
    //untested:
    public static float LerpUnclamped(float a, float b, float t, bool overshootEnabled = false)
    {
        if (!overshootEnabled)
            t = Mathf.Clamp01(t);
        return t * b + (1 - t) * a;
    }

    public static System.Random rand= new System.Random();
    //spring math
    public static float Spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }
    public static Vector3 SuperSmoothLerp(Vector3 x0, Vector3 y0, Vector3 yt, float t, float k)
    {
        //found at:https://forum.unity3d.com/threads/how-to-smooth-damp-towards-a-moving-target-without-causing-jitter-in-the-movement.130920/
        //haven't tried it yet.
        Vector3 f = x0 - y0 + (yt - y0) / (k * t);
        return yt - (yt - y0) / (k*t) + f * Mathf.Exp(-k*t);
    }
    //added by BP@:
    public static Vector3 Spring(Vector3 start, Vector3 end, float value)
    {
        float x = Spring(start.x, end.x, value);
        float y = Spring(start.y, end.y, value);
        float z = Spring(start.z, end.z, value);

        return new Vector3(x, y, z);
    }
   
    public static Vector3[] CopyVertorArray(Vector3[] ary)
    {
        Vector3[] copyArray = new Vector3[ary.Length];
        for (int i = 0; i < ary.Length; i++)
        {
            copyArray[i] = new Vector3(ary[i].x, ary[i].y, ary[i].z);
        }
        return copyArray;
    }
    public static List<Vector3> VectorArrayToList(Vector3[] ary)
    {
        List<Vector3> returnList= new List<Vector3>();
        for (int i = 0; i < ary.Length; i++)
        {
            returnList.Add(new Vector3(ary[i].x, ary[i].y, ary[i].z));
        }
        return returnList;
    }
    public static Quaternion Spring(Quaternion start, Quaternion end, float value)
    {
        float x = Spring(start.x, end.x, value);
        float y = Spring(start.y, end.y, value);
        float z = Spring(start.z, end.z, value);
        float w = Spring(start.w, end.w, value);

        return new Quaternion(x, y, z,w);
    }
    //str
    public static Quaternion Spring(Quaternion start, Quaternion end, float value,float strength,float Elasticity)
    {
        float x = Spring(start.x, end.x, value, strength,Elasticity);
        float y = Spring(start.y, end.y, value, strength,Elasticity);
        float z = Spring(start.z, end.z, value, strength,Elasticity);
        float w = Spring(start.w, end.w, value, strength,Elasticity);

        return new Quaternion(x, y, z,w);
    }
    public static Vector3 Spring(Vector3 start, Vector3 end,float value,float strength, float Elasticity)
    {
        float x = Spring(start.x, end.x, value, strength,Elasticity);
        float y = Spring(start.y, end.y, value, strength,Elasticity);
        float z = Spring(start.z, end.z, value, strength,Elasticity);

        return new Vector3(x, y, z);
    }
    public static float Spring(float start, float end, float value,float strength,float Elasticity)
    {
        //i'm not using strength yet!
        value = Mathf.Clamp01(value);
        value = ((Mathf.Sin(value * Mathf.PI*Elasticity * (0.2f + 2.5f * value * value * value))) * Mathf.Pow(1f- value,  2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    public static Vector3 ParabolaSpring(Vector3 start, Vector3 end, float height, float t)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Spring(start.y, end.y, t), mid.z);
    }
    public static Vector3 ParabolaSpring(Vector3 start, Vector3 end, float height, float t,float strength,float elasticity)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);
       // var mid = Vector3.Lerp(start, end, t,strength,elasticity);

        return new Vector3(mid.x, f(t) + Spring(start.y, end.y, t,strength,elasticity), mid.z);
    }
    public static Vector3 ParabolaSpringMid(Vector3 start, Vector3 end, float height, float t,float strength,float elasticity)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        //  var mid = Vector3.Lerp(start, end, t);
        var mid = Spring(start, end, t,strength/2,elasticity);

        return new Vector3(mid.x, f(t) + Spring(start.y, end.y, t,strength,elasticity), mid.z);
    }

    public static void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relataive scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.localPosition = FP;
    }


    public static float GetRandomFloat(float min,float max)
    {
        return Random.Range(min,max);
    }
    public static int GetRandomInt(int min,int max)
    {
        return rand.Next(min, max);
    }
    public static bool GetRandomBool()
    {
        bool result = true;

        if (GetRandomInt(0, 5) > 3)
        {
            result = false;
        }

        return result;
    }
}

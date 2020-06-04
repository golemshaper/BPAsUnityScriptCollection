using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform actualBar;
    public Transform ghostBar;
    Vector3 initialSize = Vector3.zero;
    Vector3 lastScale = Vector3.zero;
    Vector3 setToScale = Vector3.zero;
   

    int maxValue =100;
    int currentValue=100;
    public float speed = 2f;
    float progress = 0f;
    public void SetMaxValue(int max)
    {
        maxValue = max;
    }
    public void SetValue(int cur, bool useSlideEffect)
    {
        bool healing = false;
        if (currentValue < cur) healing = true;
        currentValue = cur;
     
        progress = 0f;
        doUpdate = true;
        setToScale = CalculateTargetScale();
        lastScale = actualBar.localScale;
        ghostBar.localScale = lastScale;
        if(healing)
        {
            ghostBar.localScale = setToScale;
        }
        if (useSlideEffect == false)
        {
            doUpdate = false;
            ghostBar.localScale = setToScale;
            actualBar.localScale = setToScale;
            
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        initialSize=actualBar.localScale;
    }
    Vector3 CalculateTargetScale()
    {
        //horizontal for now... add an axis option for X,Y,Z
        float percentage = (float)currentValue / (float)maxValue;
        return new Vector3(initialSize.x*percentage, 1, 1);
    }
    bool doUpdate = false;

    public bool TEST1, TEST2, TEST3;

    // Update is called once per frame
    void Update()
    {
        if (TEST1)
        {
            SetMaxValue(10);
            TEST1 = false;
            SetValue(10, true);
        }
        if (TEST2)
        {
            SetMaxValue(10);
            TEST2 = false;
            SetValue(0, true);
        }
        if (TEST3)
        {
            SetMaxValue(10);
            TEST3 = false;
            SetValue(5, true);
        }

        if (!doUpdate) return;
        progress += speed * Time.deltaTime;
        actualBar.localScale = Vector3.Lerp(lastScale, setToScale, progress);
        if(progress>=1)
        {
            progress = 1f;
            actualBar.localScale = setToScale;
            ghostBar.localScale = setToScale;
            doUpdate = false;
        }
    }
}

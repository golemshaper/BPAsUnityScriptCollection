using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RollingNumberDisplay : MonoBehaviour
{
    StringBuilder sb = new StringBuilder();
    string preText = string.Empty;
    string postText = string.Empty;
    int newValue = 0;
    int lastValue = 0;
    float progress=0f;
    public float speed = 2f;
    public TextMeshProUGUI numberDisplay;
    private void Awake()
    {
        if (numberDisplay == null) numberDisplay = this.GetComponent<TextMeshProUGUI>();
    }
    public void SetPostText(string nPosText)
    {
        postText = nPosText;
    }
    public void SetPreText(string nPreText)
    {
        preText = nPreText;
    }
    void Draw(int number)
    {
        sb.Clear();
        sb.Append(preText);
        sb.Append(number);
        sb.Append(postText);
        numberDisplay.text = sb.ToString();
    }
    public void SetValue(int val,bool useRollEffect)
    {
        if (useRollEffect == false)
        {
            //do not do effect
            doUpdate = false;
            newValue = val;
            lastValue = newValue;
            Draw(val);
            return;
        }
        //do effect
        lastValue = newValue;
        newValue = val;
        progress = 0f;
        doUpdate = true;
    }
    bool doUpdate = true;


    // Update is called once per frame
    void Update()
    {
   

        if (doUpdate==false)
        {
            return;
        }
       
        progress +=speed* Time.deltaTime;
        if (progress >= 1f)
        {
            Draw(newValue);
            doUpdate = false;
            return;
        }
        Draw(Mathf.RoundToInt(Mathf.Lerp(lastValue, newValue, progress)));
        
    }
}

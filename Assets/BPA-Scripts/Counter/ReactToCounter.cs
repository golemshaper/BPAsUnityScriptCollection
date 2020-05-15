using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BPA.Gamedata
{ 
    public class ReactToCounter : MonoBehaviour
    {
        public Counter watchCounter;
        public int value;
        public Counter getValueFromCounter;
        public enum CompareType {Equals,LessThen,LessThenOrEqual,GreaterThen,GreaterThenOrEqual};
        public CompareType compareType = CompareType.Equals;
        public GameObject[] enableIfMatch;
        public GameObject[] disableIfMatch;
        public bool reverseActionIfNoMatch;
        public string result;
        // Start is called before the first frame update
        void Start()
        {
            watchCounter.SubscribeToValueChange(()=> OnValueChanged());
        }
        void ShowResult(string input)
        {
            result = input;
        }
        void OnValueChanged()
        {
            if(getValueFromCounter!=null) value = getValueFromCounter.Value;
            bool foundMatch = false;
            switch (compareType)
            {
                case CompareType.Equals:
                    ShowResult(watchCounter.Value + "=" + value + "= false");

                    if (watchCounter.Value == value)
                    {
                        DoAction();
                        foundMatch = true;
                        ShowResult(watchCounter.Value + "="+value+"= true");
                    }
                    break;
                case CompareType.LessThen:
                    ShowResult(watchCounter.Value + "<" + value + "= false");
                    if (watchCounter.Value < value)
                    {
                        DoAction();
                        foundMatch = true;
                        ShowResult(watchCounter.Value + "<" + value + "= true");

                    }
                    break;
                case CompareType.GreaterThen:
                    if (watchCounter.Value > value)
                    {
                        DoAction();
                        foundMatch = true;
                    }
                    break;
                case CompareType.LessThenOrEqual:
                    if (watchCounter.Value <= value)
                    {
                        DoAction();
                        foundMatch = true;
                    }
                    break;
                case CompareType.GreaterThenOrEqual:
                    if (watchCounter.Value >= value)
                    {
                        DoAction();
                        foundMatch = true;
                    }
                    break;
            }
            if(!foundMatch)
            {
                DoReverseAction();
            }

        }
        void DoAction()
        {
            foreach(GameObject g in enableIfMatch)
            {
                if (g != null) g.SetActive(true);
            }
            foreach (GameObject g in disableIfMatch)
            {
                if (g != null) g.SetActive(false);
            }
        }
        void DoReverseAction()
        {
            foreach (GameObject g in enableIfMatch)
            {
                if (g != null) g.SetActive(false);
            }
            foreach (GameObject g in disableIfMatch)
            {
                if (g != null) g.SetActive(true);
            }
        }
        private void OnDestroy()
        {
            watchCounter.UnsubscribeToValueChange(()=> OnValueChanged());
        }
    }
}
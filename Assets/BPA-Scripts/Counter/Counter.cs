using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BPA.Gamedata
{
    public class Counter : MonoBehaviour
    {
        [Header("Save Data")]
        public string group = "MyGroup";
        public string key = "MyCounter";
        [Header("Data:")]
        [SerializeField]
        private int value;
        List<System.Action> onValueChanged = new List<System.Action>();
        public bool AutoSave = true;
        public bool AutoLoad = true;
        public bool reloadOrSetInitialOnEnable;
        int initialVal = 0;
        public void SubscribeToValueChange(System.Action a)
        {
            onValueChanged.Add(a);
        }
        public void UnsubscribeToValueChange(System.Action a)
        {
            onValueChanged.Add(a);
        }
        private void Awake()
        {
            initialVal = value;
        }
        private void Start()
        {
            
            if (!AutoLoad)
            {
              
                return;
            }
            Load();

        }
        private void OnEnable()
        {
            if(reloadOrSetInitialOnEnable)
            {
                if (AutoLoad) Load();
                else Value = initialVal;
            }
        }
        public int Value
        {
            get => value;
            set => Set(value);
        }
        public void Set(int value)
        {
            //set is vvalled by modifying Value 
            this.value = value;
            ValueChanged();
        }
        public void Add(int addVal)
        {
            Value += addVal;
        }
        public void Sub(int subVal)
        {
            Value -= subVal;
        }
        public void Mult(int mult)
        {
            Value *= mult;
        }
        public void Divide(int divide)
        {
            Value /= divide;
        }
        public void Save()
        {
            if (IniGameMemory.instance == null) return;
            IniGameMemory.instance.WriteData(group, key, Value);
        }
        public void Load()
        {
            if (IniGameMemory.instance == null) return;
            Value = IniGameMemory.instance.GetDataValue(group, key, value);
        }
        public void ValueChanged()
        {
            foreach (var a in onValueChanged)
            {
                a();
            }
            if(AutoSave)
            {
                Save();
            }
        }
    }
}
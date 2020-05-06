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
        public void SubscribeToValueChange(System.Action a)
        {
            onValueChanged.Add(a);
        }
        public void UnsubscribeToValueChange(System.Action a)
        {
            onValueChanged.Add(a);
        }
        private void Start()
        {
            Load();
        }
        public int Value
        {
            get => value;
            set => Set(value);
        }
        public void Set(int value)
        {
            this.value = value;
            ValueChanged();
        }
        public void Add(int addVal)
        {
            Value += addVal;
        }
        public void Sub(int subVal)
        {
            Value += subVal;
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
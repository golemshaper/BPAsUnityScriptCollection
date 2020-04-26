using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BPA.Gamedata
{
    public class ModifyCounter : MonoBehaviour
    {
        [Header("ModifyOnEnable")]
        public Counter modifyCounter;
        public int value = 1;
        public enum ModifyType { Add, Sub, Mult, Div };
        public ModifyType modifyType;

        private void OnEnable()
        {
            if (modifyCounter == null) return;
            switch (modifyType)
            {
                case ModifyType.Add:
                    modifyCounter.Add(value);
                    break;
                case ModifyType.Sub:
                    modifyCounter.Add(value);
                    break;
                case ModifyType.Mult:
                    modifyCounter.Mult(value);
                    break;
                case ModifyType.Div:
                    modifyCounter.Divide(value);
                    break;
            }
        }
    }
}


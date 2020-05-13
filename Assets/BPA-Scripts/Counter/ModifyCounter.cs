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
        public enum ModifyType { Add, Sub, Mult, Div,Set };
        public ModifyType modifyType;
        public bool autoDisable;
        public GameObject ignoreIfActive;
        private void OnEnable()
        {
            if(ignoreIfActive!=null)
            {
                if (ignoreIfActive.activeSelf) return;
            }

            if (modifyCounter == null) return;
            switch (modifyType)
            {
                case ModifyType.Add:
                    modifyCounter.Add(value);
                    break;
                case ModifyType.Sub:
                    modifyCounter.Sub(value);
                    break;
                case ModifyType.Mult:
                    modifyCounter.Mult(value);
                    break;
                case ModifyType.Div:
                    modifyCounter.Divide(value);
                    break;
                case ModifyType.Set:
                    modifyCounter.Set(value);
                    break;
            }
            if(autoDisable)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}


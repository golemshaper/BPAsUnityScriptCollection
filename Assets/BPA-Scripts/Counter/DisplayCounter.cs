using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BPA.Gamedata
{ 
    public class DisplayCounter : MonoBehaviour
    {
        public Counter watchCounter;
        public TextMeshProUGUI guiDisplay;
        public string PreString = "Score: ";
        // Start is called before the first frame update
        void Start()
        {
            watchCounter.SubscribeToValueChange(() => OnValueChanged());
        }
        private void OnDestroy()
        {
            watchCounter.UnsubscribeToValueChange(() => OnValueChanged());
        }
        void OnValueChanged()
        {
            guiDisplay.text = PreString + watchCounter.Value.ToString();
        }
    }
}
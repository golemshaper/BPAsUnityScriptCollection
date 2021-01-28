using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DoomStyle
{ 
    public class DoomStyleSpriteTest : MonoBehaviour
    {
        public SpritePlus[] spriteCollection;
        public SpriteRenderer rndr;
        public float angle = 0f;
        public int spriteIndex = 0;

        public Transform spriteT;
        public Transform cameraT;
        public float localAngle = 0f;
        // Update is called once per frame
        void Update()
        {
            //The basic idea behind the doom sprite math I think.
            //We also probably want to add an internal facing angle for the charater itself
            Vector3 dir = spriteT.position - cameraT.position;
            angle = Mathf.Atan2(dir.z, dir.x)* Mathf.Rad2Deg;
            //local angle is the characters facing angle relative to the world. Get this from the character/object itself
            angle += localAngle;
            if (angle < 0.0)
            {
                angle = angle + 360.0f;
            }
            spriteIndex = Mathf.RoundToInt(angle / 45.0f);
            rndr.sprite = spriteCollection[spriteIndex].sprite;
            rndr.flipX = spriteCollection[spriteIndex].flipX;
            //some billboarding technique here:
            spriteT.LookAt(cameraT);
        }
    }
    [System.Serializable]
    public class SpritePlus
    {
        public Sprite sprite;
        public bool flipX;
    }
}
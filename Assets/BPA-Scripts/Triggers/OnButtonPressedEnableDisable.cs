using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonPressedEnableDisable : MonoBehaviour
{
    public GameObject[] enable;
    public GameObject[] disable;
    void DoAction()
    {
        DoAction(false);
    }
    void DoAction(bool invertAction)
    {
        foreach (GameObject g in enable)
        {
            if (g != null) g.SetActive(!invertAction);
        }
        foreach (GameObject g in disable)
        {
            if (g != null) g.SetActive(invertAction);
        }
    }
    public enum PressType {Press, Hold};
    public PressType press;
    public enum KeyType {fire1,fire2,fire3,fire4};
    public KeyType whichKey;
    // Update is called once per frame
    void Update()
    {
        switch (press)
        {
            case PressType.Press:
                PressAction();
                break;
            case PressType.Hold:
                HoldeAction();
                break;
        }
    }
    void PressAction()
    {
        switch (whichKey)
        {
            case KeyType.fire1:
                if(PlayerInput.instance.GetFire1_B())
                {
                    DoAction();
                }
                break;
            case KeyType.fire2:
                if (PlayerInput.instance.GetFire2_A())
                {
                    DoAction();
                }
                break;
            case KeyType.fire3:
                if (PlayerInput.instance.GetFire3_Y())
                {
                    DoAction();
                }
                break;
            case KeyType.fire4:
                if (PlayerInput.instance.GetFire4_X())
                {
                    DoAction();

                }
                break;
        }
    }
    bool limitOnce = true;
    void HoldeAction()
    {
        switch (whichKey)
        {
            case KeyType.fire1:
                if (PlayerInput.instance.GetHoldFire1_B())
                {
                    if(!limitOnce)
                    {
                        limitOnce = true;
                        DoAction();
                    }
                }
                else
                {
                    if (limitOnce)
                    {
                        limitOnce = false;
                        DoAction(true);
                    }
                }
                break;
            case KeyType.fire2:
                if (PlayerInput.instance.GetHoldFire2_A())
                {
                    if (!limitOnce)
                    {
                        limitOnce = true;
                        DoAction();
                    }
                }
                else
                {
                    if (limitOnce)
                    {
                        limitOnce = false;
                        DoAction(true);
                    }
                }
                break;
            case KeyType.fire3:
                if (PlayerInput.instance.GetHoldFire3_Y())
                {
                    if (!limitOnce)
                    {
                        limitOnce = true;
                        DoAction();
                    }
                }
                else
                {
                    if (limitOnce)
                    {
                        limitOnce = false;
                        DoAction(true);
                    }
                }
                break;
            case KeyType.fire4:
                if (PlayerInput.instance.GetHoldFire4_X())
                {
                    if (!limitOnce)
                    {
                        limitOnce = true;
                        DoAction();
                    }
                }
                else
                {
                    if (limitOnce)
                    {
                        limitOnce = false;
                        DoAction(true);
                    }
                }
                break;
        }
    }
}

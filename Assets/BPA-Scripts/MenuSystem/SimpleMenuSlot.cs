using Ackk.GameEngine.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMenuSlot : MonoBehaviour
{
    public GameObject[] enableOnSelect;
    public GameObject[] disableOnSelect;
    public SimpleMenu linkToNextMenu;
    public void DoSelectAction()
    {
        GameObjectOperations.EnableDisableGameObjects(enableOnSelect, true);
        GameObjectOperations.EnableDisableGameObjects(disableOnSelect, false);

    }
    public Vector3 GetPosition(bool useLocalPosition)
    {
        //TODO: Cache Transform
        if (useLocalPosition) return transform.localPosition;
        return transform.position;
    }
    public void Highlight(bool inHighlightState)
    {
        //TODO: Set from menu
    }
}

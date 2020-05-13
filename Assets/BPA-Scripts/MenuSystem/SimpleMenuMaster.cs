using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMenuMaster : MonoBehaviour
{
    [SerializeField] SimpleMenu currentMenu;
    [SerializeField] SimpleMenu history;

    // Update is called once per frame
    void Update()
    {
        if (currentMenu != null) currentMenu.DoUpdate();
    }
    public void SetActiveMenu(SimpleMenu menu)
    {
        //record history by default
        SetActiveMenu(menu, true);
    }
    public void SetActiveMenu(SimpleMenu menu, bool setHistory)
    {
        //close current menu operation.
        history = currentMenu;
        currentMenu.CloseMenu();
        currentMenu = menu;
        //open new menu
        currentMenu.OpenMenu(history);
    }

}

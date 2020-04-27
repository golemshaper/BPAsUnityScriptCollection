using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * Add any sort of "gloabal" system object references here
     */
    public static GameManager instance;
    public GameObject Player;
    public PlayerMovementBPA PlayerMovement;
    public bool pauseMovementOnReadDialogue=true;

    private System.Random rand = new System.Random();

    public System.Random RandomMain()
    {
        return rand;
    }

    private void Awake()
    {
        instance = this;
    }
    void OnDialogueRead()
    {
        if (!pauseMovementOnReadDialogue) return;
       if(PlayerMovement!=null) PlayerMovement.enabled = false;
    }
    void OnDialogueEnd()
    {
        if (PlayerMovement != null) PlayerMovement.enabled = transform;
    }
    Action OnRead;
    Action OnReadEnd;
    // Start is called before the first frame update
    void Start()
    {
        OnRead = () => OnDialogueRead();
        OnReadEnd = () => OnDialogueEnd();
        if (ScreenplayReader.instance!=null)
        {
            ScreenplayReader.instance.SubscribeOnDialogeRead(OnRead);
            ScreenplayReader.instance.SubscribeOnDialogeEnd(OnReadEnd);
        }

    }
}

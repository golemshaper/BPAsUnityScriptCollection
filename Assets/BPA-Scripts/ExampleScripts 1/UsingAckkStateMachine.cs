using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingAckkStateMachine : MonoBehaviour
{
    public AckkStateMachine sm= new AckkStateMachine();
    enum State { idle, walk, run };

    [SerializeField]
    State testState = State.idle;
    [SerializeField]
    bool changeState;

    // Start is called before the first frame update
    void Start()
    {
        sm.LinkStates((int)State.idle, () => Idle(),
            ()=> Debug.Log("I have begun Idling"),
            ()=> Debug.Log("I have stopped Idling"));
        sm.LinkStates((int)State.walk, () => Walk());
        sm.LinkStates((int)State.run, () => Run());
    }
    private void Update()
    {
        if(changeState)
        {
            changeState = false;
            sm.SetState((int)testState);
        }
        sm.UpdateTick();
    }
    void Idle()
    {
        Debug.Log("I am idle");
    }
    void Walk()
    {
        Debug.Log("I am walking");

    }
    void Run()
    {
        Debug.Log("I am running");
    }
}

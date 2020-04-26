using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * How to use the state machine
 */
public class UsingAckkStateMachine : MonoBehaviour
{
    public AckkStateMachine sm= new AckkStateMachine();
    enum State { idle, walk, run };

    [SerializeField]
    State testState = State.idle;
    [SerializeField]
    bool changeState;
    [Header("Debug:")]
    [SerializeField]
    int debugState;

    // Start is called before the first frame update
    void Start()
    {
        sm.LinkStates(State.idle, () => Idle(),
            ()=> Debug.Log("I have begun Idling"),
            ()=> Debug.Log("I have stopped Idling"));
        sm.LinkStates(State.walk, () => Walk());
        sm.LinkStates(State.run, () => Run());
    }
    private void Update()
    {
        if(changeState)
        {
            changeState = false;
            sm.SetState((int)testState);
        }
        sm.UpdateTick();
        debugState = sm.GetState();

    }
    void Idle()
    {
        Debug.Log("I am idle");
    }
    void Walk()
    {
        Debug.Log("I am walking");
        if (sm.TimeInState > 2f)
        {
            Debug.Log("Tired of walking");
            sm.SetState(State.idle);
        }
    }
    void Run()
    {
        Debug.Log("I am running");
        if (sm.TimeInState > 4f)
        {
            Debug.Log("Tired of running");
            sm.SetState(State.walk);
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AckkStateMachine
{
    int currentState = -1;
    int previousState = -2;
    public Dictionary<int, Action> stateDictionary = new Dictionary<int, Action>();
    public Dictionary<int, Action> onEnterState = new Dictionary<int, Action>();
    public Dictionary<int, Action> onExitState = new Dictionary<int, Action>();
    public float TimeInState = 0f;
    public float speed = 1f;
    //Call me to make the machine run!
    public void UpdateTick()
    {
        Action result = null;
        stateDictionary.TryGetValue(currentState, out result);
        if (result != null) result();
        TimeInState += speed * Time.deltaTime;
    }
    public void LinkStates(int state, Action looping)
    {
        stateDictionary.Add(state, looping);

    }
    public void LinkStates(int state, Action looping, Action onEnter)
    {
        stateDictionary.Add(state, looping);
        onEnterState.Add(state, onEnter);
    }
    public void LinkStates(int state, Action looping, Action onEnter, Action onExit)
    {
        stateDictionary.Add(state, looping);
        onEnterState.Add(state, onEnter);
        onExitState.Add(state, onExit);
    }
    public void LinkOnEnterState(int state, Action onEnter)
    {
        onEnterState.Add(state, onEnter);
    }
    public void LinkOnEnterState(Enum State, Action onEnter)
    {
        int state = Convert.ToInt32(State);
        onEnterState.Add(state, onEnter);
    }
    public void LinkOnExitState(int state, Action onExit)
    {
        onExitState.Add(state, onExit);
    }
    public void LinkOnExitState(Enum State, Action onExit)
    {
        int state = Convert.ToInt32(State);
        onExitState.Add(state, onExit);
    }
    //--generic enum--
    public void LinkStates(Enum State, Action looping, Action onEnter, Action onExit)
    {
        int state = Convert.ToInt32(State);
        LinkStates(state, looping, onEnter, onExit);
    }
    public void LinkStates(Enum State, Action looping, Action onEnter)
    {
        int state = Convert.ToInt32(State);
        LinkStates(state, looping, onEnter);
    }
    public void LinkStates(Enum State, Action looping)
    {
        int state = Convert.ToInt32(State);
        LinkStates(state, looping);
    }

    /// <summary>
    /// Cast an Enum to an int to use it as the state.
    /// </summary>
    /// <param name="nState"></param>
    public void SetState(int nState)
    {
        // Already in state, so do nothing.
        if (nState == currentState) return;
        //EXIT STATE
        Action exitStateAction = null;
        onExitState.TryGetValue(currentState, out exitStateAction);
        if (exitStateAction != null) exitStateAction();
        //Previous State
        previousState = currentState;
        //SET STATE
        currentState = nState;
        //Reset Timer
        TimeInState = 0f;
        //ENTER STATE
        Action enterStateAction = null;
        onEnterState.TryGetValue(currentState, out enterStateAction);
        if (enterStateAction != null) enterStateAction();
    }
    public void SetState (Enum nState,string log_message)
    {
        SetState(Convert.ToInt32(nState));
        //Do nothing with message for now...
    }
    public void SetState(Enum nState)
    {
        SetState(Convert.ToInt32(nState));
    }
    public int GetState()
    {
        return currentState;
    }
    public int GetPreviousState()
    {
        return previousState;
    }
    /// <summary>
    /// Warning do not use with without knowing what you are doing!
    /// </summary>
    public void ForceStateWithoutEnter(Enum nState)
    {
        currentState = Convert.ToInt32(nState);
    }
}
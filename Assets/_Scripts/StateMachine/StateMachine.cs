using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class StateMachine
{
    public class State
    {
        public string stateName;
        public event Action EnterState;
        public event Action ExitState;

        public void Enter() { EnterState?.Invoke(); }
        public void Exit() { ExitState?.Invoke(); }
        public State() { }
        public State(string stateName) { this.stateName = stateName; }
    }

    public State currentState
    {
        get { return _currentState; }
        set
        {
            if (_currentState != value)
            {
                if (_currentState != null)
                {
                    Debug.Log("Exit " + _currentState.stateName);
                    _currentState.Exit();
                }
                _currentState = value;
                Debug.Log("Enter " + _currentState.stateName);
                _currentState.Enter();
            }
        }
    }

        State _currentState;
}
 

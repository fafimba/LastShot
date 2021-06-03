using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchStateMachine : StateMachine
{
    public static State WaittingToStart = new State("WaittingToStart");
    public static State Playing = new State("Playing");
    public static State Finish = new State("Finish");

}



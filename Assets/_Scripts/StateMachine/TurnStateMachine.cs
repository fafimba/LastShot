using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStateMachine : StateMachine
{
    public static State Idle = new State("Idle");
    public static State StartingTurn = new State("StartingTurn");
    public static State CountDown = new State("Countdown");
    public static State SolvingTurn = new State("SolvingTurn");
    public static State AnimatingTurn = new State("AnimatingTurn");
    public static State EndingTurn = new State("EndingTurn");
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public static State Disabled = new State("Disabled");
    public static State Playing = new State("Idle");
    public static State SelectingTarget = new State("SelectingTarget");
    public static State Death = new State("Death");

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MatchController : MonoBehaviour
{

  public static DO_Match Match = new DO_Match();
  public static List<CharacterController> characterControllers = new List<CharacterController>();

    #region StateMachine
    public static StateMachine stateMachine = new StateMachine();
    public static StateMachine.State WaittingToStart = new StateMachine.State("WaittingToStart");
    public static StateMachine.State Playing = new StateMachine.State("Playing");
    public static StateMachine.State Finish = new StateMachine.State("Finish");
    #endregion

    private void OnEnable()
    {
        
        MatchController.Playing.EnterState += StartGame;
        TurnController.EndingTurn.ExitState += CheckEndGame;
    }

    private void OnDisable()
    {
        MatchController.Playing.EnterState -= StartGame;
        TurnController.EndingTurn.ExitState -= CheckEndGame;
    }
    private void Start()
    {
    }

    void StartGame()
    {
    TurnController.stateMachine.currentState = TurnController.StartingTurn;
    }

    void CheckEndGame()
    {
      //  if (!PhotonNetwork.IsMasterClient)
      //  {
      //      return;
      //  }

        List<CharacterController> charactersAlive = characterControllers.Where(x => x.isAlive).ToList();
        if (charactersAlive.Count() == 1)
        {
            PlayerPrefs.SetString("winner", charactersAlive[0].characterData.Player.NickName);
            PhotonNetwork.LoadLevel(3);
        }
        else if (charactersAlive.Count() == 0)
        {
            PlayerPrefs.SetString("winner", "none");
            PhotonNetwork.LoadLevel(3);
        }
    }
}

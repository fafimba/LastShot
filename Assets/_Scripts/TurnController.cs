using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    [SerializeField] Text turnNumber;

    public static DO_Turn CurrentTurn;

    List<string> ClientsSolved;

    PhotonView myPhotonView;

    public static event Action TurnSolved;
    public static event Action<DO_CharacterAction> CharacterAction;

    #region StateMachine
    public static StateMachine stateMachine = new StateMachine();
    public static StateMachine.State Idle = new StateMachine.State("Idle");
    public static StateMachine.State StartingTurn = new StateMachine.State("StartingTurn");
    public static StateMachine.State CountDown = new StateMachine.State("Countdown");
    public static StateMachine.State SolvingTurn = new StateMachine.State("SolvingTurn");
    public static StateMachine.State AnimatingTurn = new StateMachine.State("AnimatingTurn");
    public static StateMachine.State EndingTurn = new StateMachine.State("EndingTurn");
    #endregion

    private void OnEnable()
    {
        TurnTimer.TimeOver += TurnTimeOver;

        PlayerController.SetDecision += SetPlayerDecision;

        TurnController.StartingTurn.EnterState += NewTurn;
        TurnController.SolvingTurn.EnterState += SolveTurn;
       // TurnController.AnimatingTurn.EnterState += AnimateTurn;
       // TurnController.EndingTurn.EnterState += SaveTurn;
        TurnController.EndingTurn.EnterState += EndTurn;

        MatchController.Playing.EnterState += TurnStart;
     }

    private void OnDisable()
    {
        TurnTimer.TimeOver -= TurnTimeOver;

        PlayerController.SetDecision -= SetPlayerDecision;

        TurnController.StartingTurn.EnterState -= NewTurn;
        TurnController.SolvingTurn.EnterState -= SolveTurn;
      //  TurnController.AnimatingTurn.EnterState -= AnimateTurn;
      //  TurnController.EndingTurn.EnterState -= SaveTurn;
        TurnController.EndingTurn.EnterState -= EndTurn;

        MatchController.Playing.EnterState -= TurnStart;
    }

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
    }
 
    void TurnStart()
    {
        Debug.Log("TurnStart");
      //  TurnController.stateMachine.currentState = TurnController.StartingTurn;
    }

    void NewTurn()
    {
        DO_Turn turn = new DO_Turn();
        turn.turn = MatchController.Match.turns.Count;

        turn.playersDecision = new List<DO_CharacterAction>();
        foreach (var character in MatchController.characterControllers)
        {
            var characterAction = new DO_CharacterAction();
            characterAction.owner = character.characterData;
            if (character.ammo>0)
            {
                characterAction.action = 0;
            }
            else
            {
                characterAction.action = 1;
            }
            characterAction.target = character.characterData;
            turn.playersDecision.Add(characterAction);
        }
  
        CurrentTurn = turn;
        turnNumber.text = (CurrentTurn.turn + 1).ToString();
        Debug.Log(CurrentTurn.turn.ToString());
        TurnController.stateMachine.currentState = TurnController.CountDown;
    }

    void TurnTimeOver()
    {
        Debug.Log("Timer over");
        TurnController.stateMachine.currentState = TurnController.SolvingTurn;
    }

    void SetPlayerDecision(DO_CharacterAction characterAction)
    {
        myPhotonView.RPC("RPC_SetTurnDecision", RpcTarget.MasterClient, JsonConvert.SerializeObject( characterAction));
    }

    [PunRPC]
    void RPC_SetTurnDecision(string characterAction)
    {
        Debug.Log("Before writing decision Count:" + CurrentTurn.playersDecision.Count);
        DO_CharacterAction characterActionSended = JsonConvert.DeserializeObject<DO_CharacterAction>(characterAction);
        CurrentTurn.playersDecision.RemoveAll(x => x.owner.CharacterID == characterActionSended.owner.CharacterID);
        CurrentTurn.playersDecision.Add(characterActionSended);

        Debug.Log("After writing decision Count:" + CurrentTurn.playersDecision.Count);

    }

    public void SolveTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ClientsSolved = new List<string>();
            StartCoroutine(CR_WaitingForClientsToSolve());
            myPhotonView.RPC("RPC_SolveTurn", RpcTarget.All, JsonConvert.SerializeObject(CurrentTurn));
        }
    }

    [PunRPC]
    void RPC_SolveTurn(string turn)
    {
         CurrentTurn = JsonConvert.DeserializeObject<DO_Turn>(turn);

        foreach (var playerDecision in CurrentTurn.playersDecision)
        {
            CharacterAction?.Invoke(playerDecision);
        }

        //TurnSolved?.Invoke();
        myPhotonView.RPC("RPC_ConfirmToMaster", RpcTarget.MasterClient, PlayerController.playerData.PlayerID);
        // TurnController.stateMachine.currentState = TurnController.AnimatingTurn;
    }

    [PunRPC]
    void RPC_ConfirmToMaster(string playerID)
    {
        if (PhotonNetwork.PlayerList.Any(x=> x.UserId == playerID))
        {
            ClientsSolved.Add(playerID);
        }
    }

 

    IEnumerator CR_WaitingForClientsToSolve()
    {
        while (true)
        {
            if (ClientsSolved.Count() >= PhotonNetwork.PlayerList.Count())
            {
                myPhotonView.RPC("RPC_AnimateTurn", RpcTarget.All);

                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    [PunRPC]
    void RPC_AnimateTurn()
    {
           TurnController.stateMachine.currentState = TurnController.AnimatingTurn;
    }

    void EndTurn()
    {
        SaveTurn();
        TurnController.stateMachine.currentState = TurnController.StartingTurn;
    }

    void SaveTurn()
    {
         MatchController.Match.turns.Add(CurrentTurn);
    }
}

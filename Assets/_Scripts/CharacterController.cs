using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class CharacterController : MonoBehaviour
{
    public static event Action<CharacterController> DieAction;
    public static event Action<DO_Character> targetedAction;

    public void Die() { DieAction?.Invoke(this); }

    [SerializeField] RectTransform panel;
    [SerializeField] Text txtPlayerNickname;
    [SerializeField] Text txtPlayerAmmo;

    [SerializeField] Material mtDeath;
    [SerializeField] GameObject goModel;
    [SerializeField] GameObject btn_Target;

    public int ammo;
    public bool isAlive;
    public DO_Character characterData = new DO_Character();

    DO_CharacterAction characterAction = new DO_CharacterAction();
    List<DO_CharacterAction> targetOfCharactersDecisions = new List<DO_CharacterAction>();
    
    private void OnEnable()
    {
        TurnController.CharacterAction += ReceivePlayerDecision;

        TurnController.StartingTurn.EnterState += GetReady;
        TurnController.SolvingTurn.ExitState += SolveTurn;
        TurnController.AnimatingTurn.ExitState += UpdateUI;

        PlayerController.SelectingTarget.EnterState += ActivateTargetButton;
        PlayerController.SelectingTarget.ExitState += DisabledTargetButton;
    }

    private void OnDisable()
    {
        TurnController.CharacterAction -= ReceivePlayerDecision;

        TurnController.StartingTurn.EnterState -= GetReady;
        TurnController.SolvingTurn.ExitState -= SolveTurn;
        TurnController.AnimatingTurn.ExitState -= UpdateUI;

        PlayerController.SelectingTarget.EnterState -= ActivateTargetButton;
        PlayerController.SelectingTarget.ExitState -= DisabledTargetButton;
    }

    public void Init(DO_Character characterData)
    {
        this.characterData = characterData;
        txtPlayerNickname.text = characterData.Player.NickName;
        ammo = 1;
        txtPlayerAmmo.text = "Ammo: " + ammo;
        isAlive = true;

        panel.transform.position =  Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
    }

    void GetReady()
    {
        characterAction.action = 0;
        characterAction.owner = characterData;
        characterAction.target = characterData;
     //  SetDecision?.Invoke(characterDecision);
    }

    void ReceivePlayerDecision(DO_CharacterAction playerDecision)
    {
        if (playerDecision.owner.CharacterID == characterData.CharacterID)
        {
            characterAction = playerDecision;
        }

        if (playerDecision.target.CharacterID == characterData.CharacterID)
        {
            targetOfCharactersDecisions.Add(playerDecision);
        }
    }

    public void Btn_Target()
    {
        targetedAction?.Invoke(characterData);
        PlayerController.stateMachine.currentState = PlayerController.Playing;
    }

    void SolveTurn()
    {
        Debug.Log("CharacterAction " + characterAction.action);
        switch (characterAction.action)
        {
            case 0:
                Debug.Log(characterAction.owner.Player.NickName + " reload");
                ammo++;
                break;
            case 1: break;
            case 2: ammo--;
                break;
            default:
                break;
        }
        
        foreach (var targetOfCharacterDecision in targetOfCharactersDecisions)
        {
            if (targetOfCharacterDecision.action == 2)
            {
                if (characterAction.action != 1)
                {
                    if (characterAction.target  != targetOfCharacterDecision.owner)
                    {
                        Debug.Log(characterData.Player.NickName + " die by " + targetOfCharacterDecision.owner.Player.NickName + "'s bullet");

                        isAlive = false;
                    }
                    else
                    {
                        Debug.Log(characterData.Player.NickName + " shot back " + targetOfCharacterDecision.owner.Player.NickName + "'s bullet");
                    }
                }
                else
                {
                    Debug.Log(characterData.Player.NickName + " dodged " + targetOfCharacterDecision.owner.Player.NickName + "'s bullet" );
                }
            }
        }
        targetOfCharactersDecisions.Clear();
    }

    void UpdateUI()
    {
        targetOfCharactersDecisions.Clear();  
        characterAction = new DO_CharacterAction();

        txtPlayerAmmo.text = "Ammo: " + ammo;

        if (!isAlive)
        {
            goModel.GetComponent<MeshRenderer>().material = mtDeath;
        }
    }

    void ActivateTargetButton()
    {
        if (PlayerController.playerData.PlayerID != characterData.Player.PlayerID)
        {
         btn_Target.SetActive(true);
        }
    }

    void DisabledTargetButton()
    {
        btn_Target.SetActive(false);
    }
}

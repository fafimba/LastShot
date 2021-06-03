using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Image pnlButtons;
    [SerializeField] Button btnReload;
    [SerializeField] Button btnProtect;
    [SerializeField] Button btnShot;
    [SerializeField] Text txtPlayerAmmo;

    [SerializeField] Color bulletLoaded;
    [SerializeField] Color bulletUnloaded;
    [SerializeField] Image bullet1;
    [SerializeField] Image bullet2;
    [SerializeField] Image bullet3;


    public static Action<DO_CharacterAction> SetDecision;

    public static DO_Player playerData;
    public  CharacterController playerCharacterController;

    #region StateMachine
    public static StateMachine stateMachine = new StateMachine();
    public static StateMachine.State Disabled = new StateMachine.State("Disabled");
    public static StateMachine.State Playing = new StateMachine.State("Playing");
    public static StateMachine.State SelectingTarget = new StateMachine.State("SelectingTarget");
    public static StateMachine.State Death = new StateMachine.State("Death");
    #endregion

    private void OnEnable()
    {

        PlayerController.Disabled.EnterState += HideUI;
        PlayerController.Playing.EnterState += UpdateUI;

        TurnController.CountDown.EnterState += ToPlayingState;
        TurnController.CountDown.ExitState += ToDisabledState;

        CharacterController.targetedAction += Shoot;
    }

    private void OnDisable()
    {
        PlayerController.Disabled.EnterState -= HideUI;
        PlayerController.Playing.EnterState -= UpdateUI;

        TurnController.CountDown.EnterState -= ToPlayingState;
        TurnController.CountDown.ExitState -= ToDisabledState;

        CharacterController.targetedAction -= Shoot;
    }

    void ToPlayingState()
    {
        PlayerController.stateMachine.currentState = Playing;
    }

    void ToDisabledState()
    {
        PlayerController.stateMachine.currentState = Disabled;
    }


    public void Btn_Reload()
    {
        DO_CharacterAction characterDecision = new DO_CharacterAction();
        characterDecision.owner = playerCharacterController.characterData; 
        characterDecision.action = 0;
        characterDecision.target = playerCharacterController.characterData;
        SetDecision?.Invoke(characterDecision);
        PlayerController.stateMachine.currentState = PlayerController.Playing;
    }

    public void Btn_Protect()
    {
        DO_CharacterAction characterDecision = new DO_CharacterAction();
        characterDecision.owner = playerCharacterController.characterData;
        characterDecision.action = 1;
        characterDecision.target = playerCharacterController.characterData;
        SetDecision?.Invoke(characterDecision);
        PlayerController.stateMachine.currentState = PlayerController.Playing;
    }

    public void Btn_Shoot()
    {
        PlayerController.stateMachine.currentState = PlayerController.SelectingTarget;
    }

    void Shoot(DO_Character target)
    {
        DO_CharacterAction characterDecision = new DO_CharacterAction();
        characterDecision.owner = playerCharacterController.characterData;
        characterDecision.action = 2;
        characterDecision.target = target;
        SetDecision?.Invoke(characterDecision);
    }

    void HideUI()
    {
        pnlButtons.gameObject.SetActive(false);
    }

     void UpdateUI()
    {
        pnlButtons.gameObject.SetActive(playerCharacterController.isAlive);

        if (playerCharacterController.ammo > 0)
        {
            bullet1.color = bulletLoaded;
        }
        else
        {
            bullet1.color = bulletUnloaded;
        }

        if (playerCharacterController.ammo > 1)
        {
            bullet2.color = bulletLoaded;
        }
        else
        {
            bullet2.color = bulletUnloaded;
        }

        if (playerCharacterController.ammo > 2)
        {
            bullet3.color = bulletLoaded;
        }
        else
        {
            bullet3.color = bulletUnloaded;
        }

        btnShot.interactable = playerCharacterController.ammo > 0;
        btnReload.interactable = playerCharacterController.ammo < 4;
    }
}


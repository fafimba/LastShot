using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimer : MonoBehaviour
{

    [SerializeField] MultiplayerSceneController multiplayerSceneController;

    [SerializeField] Text txtTurnTimer;
    [SerializeField] int turnTime;
    float timer;

    public static Action TimeOver;

    public void OnEnable()
    {
        TurnController.CountDown.EnterState += StartTimer;
        TurnController.CountDown.ExitState += StopTimer;
    }
    public void OnDisable()
    {
        TurnController.CountDown.EnterState -= StartTimer;
        TurnController.CountDown.ExitState -= StopTimer;
    }

    void StartTimer()
    {
        Debug.Log("Start Timer");

        txtTurnTimer.gameObject.SetActive(true);

        timer = turnTime;
        string tempTimer = string.Format("{0:00}", timer);
        txtTurnTimer.text = tempTimer;
        StartCoroutine(CR_TurnTimer());
    }

    void StopTimer()
    {
        txtTurnTimer.gameObject.SetActive(false);

        Debug.Log("Stop CR");
       // StopAllCoroutines();
    
    }


    IEnumerator CR_TurnTimer()
    {
        Debug.Log("timer: " + timer);
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (timer > 0)
            {
               string tempTimer = string.Format("{0:00}", timer);
                txtTurnTimer.text = tempTimer;
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
                TimeOver?.Invoke();
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameSceneController : MonoBehaviour
{

    [SerializeField] Text txtWinnerName;

    void Start()
    {
        txtWinnerName.text = PlayerPrefs.GetString("winner");
    }


    public void BTN_EndGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}

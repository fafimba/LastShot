using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManualStartWaitingRoomController : MonoBehaviourPunCallbacks
{


    [SerializeField] int menuSceneIndex;
    [SerializeField] int multiplayerSceneIndex;

    int playerCount;
    int roomSize;
    [SerializeField] int minPlayersToStart;

    [SerializeField] Text roomCountDisplay;
    [SerializeField] Button btnStart;

    private void Start()
    {
        btnStart.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        PlayerCountUpdate();
    }

    void PlayerCountUpdate()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomCountDisplay.text = playerCount + ":" + roomSize;

        btnStart.interactable = playerCount > 0;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }
    
    public void StartGame()
    {
       // if (!PhotonNetwork.IsMasterClient)
       // {
       //     return;
       // }

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    public void ManualStart()
    {
        StartGame();
    }

    public void ManualCancel()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }
}

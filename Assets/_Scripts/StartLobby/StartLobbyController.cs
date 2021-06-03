using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class StartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject StartButton;
    [SerializeField] GameObject CancelButton;
    [SerializeField] Text txtNickName;
    [SerializeField] int RoomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        StartButton.SetActive(true);
    }

    public void BtnStart()
    {
        StartButton.SetActive(false);
        CancelButton.SetActive(true);
        PhotonNetwork.LocalPlayer.NickName = txtNickName.text;
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("quick Start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Create room now");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOp = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize ,PublishUserId=true};
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOp);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a room... trying again");
        CreateRoom();
    }

    public void BtnCancel()
    {
        CancelButton.SetActive(false);
        StartButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}

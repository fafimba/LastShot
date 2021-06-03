using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MultiplayerSceneController : MonoBehaviourPunCallbacks
{

    [SerializeField] PlayerController playerController;
    [SerializeField] List<Transform> playersPositions;
    [SerializeField] GameObject characterPrefab;


    void Start()
    {
        MatchController.stateMachine.currentState = MatchController.WaittingToStart;


        Debug.Log("borrando datos match");
        MatchController.Match = new DO_Match();
        MatchController.Match.turns = new List<DO_Turn>();
        MatchController.characterControllers = new List<CharacterController>();


        Debug.Log("borrados- match.turn.count:" + MatchController.Match.turns.Count);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var player = new DO_Player();
            player.PlayerID = PhotonNetwork.PlayerList[i].UserId;
            player.NickName = PhotonNetwork.PlayerList[i].NickName;

            var character = new DO_Character();
            character.Player = player;
            character.CharacterID = player.PlayerID;
            character.CharacterName = player.NickName;
            character.CharacterPosition = playersPositions[i].position;
            SpawnCharacter(character , playersPositions[i]);
        }
        MatchController.stateMachine.currentState = MatchController.Playing;
    }
  

    public void SpawnCharacter(DO_Character characterData, Transform spawnPosition)
    {
        Debug.Log("Creating Character");

        var characterControllerInstantiate = Instantiate(characterPrefab, spawnPosition.position, spawnPosition.rotation).GetComponent<CharacterController>();
        characterControllerInstantiate.Init(characterData);

        MatchController.characterControllers.Add(characterControllerInstantiate);

        if (PhotonNetwork.LocalPlayer.UserId == characterData.Player.PlayerID)
        {
            PlayerController.playerData = characterData.Player;
            playerController.playerCharacterController = characterControllerInstantiate;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " has left the game");

        foreach (var characterController in MatchController.characterControllers)
        {
            if (characterController.characterData.Player.PlayerID== otherPlayer.UserId)
            {
                characterController.isAlive = false;
            }
        }
    }
}

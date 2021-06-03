using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DO_Character
{
    public string CharacterID { get => characterID; set => characterID = value; }
    public string CharacterName { get => characterName; set => characterName = value; }
    public Vector3 CharacterPosition { get => characterPosition; set => characterPosition = value; }
    public DO_Player Player { get => player; set => player = value; }

    string characterID;
    string characterName;
    private Vector3 characterPosition;
    DO_Player player;

}

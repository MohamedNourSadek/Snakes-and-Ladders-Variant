using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public string playerName;
    public Color playerColor;

    public PlayerInfo()
    {
        playerName = "player";
        playerColor = Color.white;
    }
    public PlayerInfo(string name, Color color)
    {
        this.playerName = name;
        this.playerColor = color;
    }
}

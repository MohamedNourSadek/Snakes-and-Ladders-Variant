using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerInfo playerInfo;
    GridPoint currentPosition;

    public void Initialize(PlayerInfo playerInfo, GridPoint startPoint)
    {
        this.playerInfo = playerInfo;
        SetPoint(startPoint);
    }


    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
        this.GetComponent<MeshRenderer>().material.color = playerInfo.playerColor;
    }
    public PlayerInfo GetPlayerInfo()
    {
        return this.playerInfo; 
    }


    public float SetPoint(GridPoint destination)
    {
        //Animation Time
        float time = 0f;
        currentPosition = destination;
        this.transform.position = currentPosition.transform.position;
        return time;
    }
    public GridPoint GetPoint()
    {
        return currentPosition;
    }
}

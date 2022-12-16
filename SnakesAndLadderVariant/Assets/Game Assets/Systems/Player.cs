using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerInfo playerInfo;
    GridPoint currentPosition;
    
    bool canSkip;
    public int lastRoll;
    int reserveAmount;

    public void Initialize(PlayerInfo playerInfo, GridPoint startPoint)
    {
        SetPlayerInfo(playerInfo);
        SetPoint(startPoint);
        canSkip = true;
        reserveAmount= 0;
        lastRoll= 0;
    }


    //Just Setter and Getters
    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
        this.GetComponent<MeshRenderer>().material.color = playerInfo.playerColor;
    }
    public PlayerInfo GetPlayerInfo()
    {
        return this.playerInfo; 
    }

    public int GetReserveAmount()
    {
        return reserveAmount;
    }
    public void IncreaseReserve(int value)
    {
        reserveAmount+= value;
    }
    public void SetReserve(int value)
    {
        reserveAmount = value;
    }

    public int GetLastRoll()
    {
        return lastRoll;
    }
    public void SetLastRoll(int value)
    {
        lastRoll = value;
    }

    public bool CanSkip()
    {
        return canSkip;
    }
    public void UseSkip()
    {
        canSkip = false;
    }

    public void SetPoint(GridPoint destination)
    {
        currentPosition = destination;
        this.transform.position = currentPosition.transform.position;
    }
    public GridPoint GetPoint()
    {
        return currentPosition;
    }
}

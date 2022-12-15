using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    string playerName { set; get; } 
    GridPoint currentPosition;

    public GridPoint GetPoint()
    {
        return currentPosition; 
    }
    public float SetPoint(GridPoint destination)
    {
        //Animation Time
        float time = 0f;
        currentPosition = destination;
        return time;
    }
}

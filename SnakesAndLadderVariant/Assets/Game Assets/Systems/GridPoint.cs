using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType { Default, Pitfall, ShortCut}

[System.Serializable]
public class GridPoint : MonoBehaviour
{
    [SerializeField] public Vector2 point;
    [SerializeField] public PointType pointType;
    [SerializeField] public Vector2 pointDestination;

    public void Initialize(Vector2 point, PointType pointType, Vector2 pointDestination)
    {
        this.point = point;
        this.pointType = pointType; 
        this.pointDestination = pointDestination;

        name = "(" + point.x + "," + point.y + ")";
    }
}


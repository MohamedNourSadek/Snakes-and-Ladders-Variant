using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType { Default, Pitfall, Skip}
public struct GridPoint
{
    public Vector2 point;
    public PointType pointType;
    public Vector2 pointDestination;
}


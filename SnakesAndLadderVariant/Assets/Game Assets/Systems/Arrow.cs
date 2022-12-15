using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField][Range(0f, .25f)] float arrowStartCull;
    [SerializeField][Range(0f, .25f)] float arrowEndCull;
    [SerializeField] GameObject arrowBody;
    [SerializeField] GameObject arrowHead;
    [SerializeField] Transform arrowHeadPosition;

    public void SetArrowTransform(GridPoint start, GridPoint end)
    {
        //Arrows can only point in 4 Directions, Up, Down, Left, Right
        float rotation = 0;
        float totalScale = 0f;

        /////Compute Transforms
        if (start.point.y == end.point.y)
        {
            if(start.point.x < end.point.x)
            {
                rotation = 0;
            }
            else
            {
                rotation = 180;
            }

            totalScale = Mathf.Abs(end.point.x - start.point.x);    
        }
        else if(start.point.x == end.point.x)
        {
            if (start.point.y < end.point.y)
            {
                rotation = -90;
            }
            else
            {
                rotation = 90;
            }

            totalScale = Mathf.Abs(end.point.y - start.point.y);
        }

        /////Set Transforms
        this.transform.Rotate(new Vector3(0f, rotation, 0f));
        
        //arrow body move
        this.transform.position = start.transform.position + (3*transform.right*(arrowStartCull));

        arrowBody.transform.localScale += (totalScale - arrowBody.transform.lossyScale.x) * Vector3.right;
        arrowBody.transform.localScale += -(arrowStartCull + arrowEndCull) * Vector3.right;

        //arrow head move
        this.arrowHead.transform.position = arrowHeadPosition.position;
    }
}

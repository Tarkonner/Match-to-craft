using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlaceGoal : MonoBehaviour
{
    public Vector2Int GridPlacement { get; private set; }

    public void Setup(Board board, Vector2Int placement)
    {
        foreach (Transform item in board.goalHolder.transform)
        {
            if(item.TryGetComponent(out PlacementGoal goal))
            {
                goal.boardGoals.Add(this);
                break;
            }
        }

        GridPlacement = placement;
    }
}

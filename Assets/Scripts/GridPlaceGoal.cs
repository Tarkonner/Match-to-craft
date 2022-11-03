using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlaceGoal : MonoBehaviour
{
    [SerializeField] dotType opinionFilledGoal = dotType.None;
    
    public Vector2Int GridPlacement { get; private set; }
    public dotType OpinionFilledGoal { get => opinionFilledGoal; set => opinionFilledGoal = value; }

    public void Setup(Board board, Vector2Int placement)
    {
        foreach (Transform item in board.goalHolder.transform)
        {
            if(item.TryGetComponent(out PlacementGoal goal))
            {
                goal.boardGoals.Add(this);
            }
        }

        GridPlacement = placement;
    }
}

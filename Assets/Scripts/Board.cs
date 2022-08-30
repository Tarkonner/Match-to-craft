using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    //Board
    [SerializeField] private Vector2Int gridSize = new Vector2Int(3, 3);
    //private Vector2 pointZero;
    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridPosition;

    //Memori
    private Dot[,] gridMemori;

    [SerializeField] private List<GoalTable> goals;

    void Start()
    {
        gridMemori = new Dot[gridSize.x, gridSize.y];

        sr = GetComponent<SpriteRenderer>();
        sr.size = gridSize;

        //Collider
        GetComponent<BoxCollider2D>().size = gridSize;
    }

    private Vector2 SnapToGrid(Vector2 targetPosition)
    {
        return new Vector2(
            Mathf.Round(targetPosition.x / sizeOfGrid) * sizeOfGrid,
            Mathf.Round(targetPosition.y / sizeOfGrid) * sizeOfGrid);
    }

    public bool PlaceDots(DotTable targetTable)
    {
        //Check if allowed
        foreach (GameObject item in targetTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + gridSize.x / 2,
                (int)SnapToGrid(item.transform.position).y + gridSize.y / 2);

            //Bounderi
            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= gridSize.x || gridPos.y >= gridSize.y)
                return false;

            //Is there already something
            if (gridMemori[gridPos.x, gridPos.y] != null)
                return false;
        }

        //Snap table to grid
        targetTable.transform.position = (Vector3)SnapToGrid(targetTable.transform.position);

        //Place dots in memory
        foreach (GameObject item in targetTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + gridSize.x / 2,
                (int)SnapToGrid(item.transform.position).y + gridSize.y / 2);

            Dot targetDot = item.GetComponent<Dot>();
            gridMemori[gridPos.x, gridPos.y] = targetDot;
            targetDot.gridPos = gridPos;
        }

        CheckGoals();

        return true;
    }

    public DotTable TakeFromBoard(Vector2 clickPosition)
    {
        Vector2 gridPosition = SnapToGrid(clickPosition);
        DotTable table = null;

        //Take table from board
        if (gridMemori[(int)gridPosition.x + gridSize.x / 2, (int)gridPosition.y + gridSize.y / 2] != null)
        {
            table = gridMemori[(int)gridPosition.x + gridSize.x / 2, (int)gridPosition.y + gridSize.y / 2].ownerTable;

            //See if it was part of complete goal
            table.pickedupAction?.Invoke();

            //Remove dots from memori
            for (int i = 0; i < table.content.Count; i++)
            {
                Vector2Int removePos = table.content[i].GetComponent<Dot>().gridPos;
                gridMemori[removePos.x, removePos.y] = null;
            }
        }

        return table;
    }

    private void CheckGoals()
    {
        for (int x = 0; x < gridMemori.GetLength(0); x++)
        {
            for (int y = 0; y < gridMemori.GetLength(1); y++)
            {
                //Check goals
                foreach (GoalTable goal in goals)
                {
                    //Bouncs
                    if (goal.pattern.GetLength(0) + x > gridMemori.GetLength(0)
                        || goal.pattern.GetLength(1) + y > gridMemori.GetLength(1))
                        continue;

                    List<DotTable> pieces = new List<DotTable>();

                    bool match = true;
                    for (int i = 0; i < goal.pattern.GetLength(0); i++)
                    {
                        if (!match)
                            continue;

                        for (int j = 0; j < goal.pattern.GetLength(1); j++)
                        {
                            Dot checking = gridMemori[x + i, y + j];

                            if (goal.pattern[i, j] == null)
                                continue;

                            if (checking == null || checking.type != goal.pattern[i, j].GetComponent<Dot>().type)
                            {
                                match = false;
                                break;
                            }

                            if(!pieces.Contains(checking.ownerTable))
                                pieces.Add(checking.ownerTable);
                        }
                    }

                    if (match)
                    {
                        goal.GoalCompletet();

                        //Save pieces there are part of goal
                        goal.subsubscribers = pieces;
                        foreach (DotTable item in pieces)
                            item.pickedupAction += goal.GoalUncomplet;
                    }
                }
            }
        }
    }
}

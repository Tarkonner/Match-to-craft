using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : SerializedMonoBehaviour
{
    [SerializeField] Board board;

    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridPosition;

    //Memori
    private GameObject[,] gridMemori;


    [SerializeField] private GameObject dotHolder;

    private bool nextLevel = false;
    private float sceneTransistenTime = 1;
    private float sceneClock;


    private void Update()
    {
        if (nextLevel)
        {
            sceneClock += Time.deltaTime;
            if (sceneClock >= sceneTransistenTime)
                board.NextLevel();
        }
    }


    public void SetupPattorn(LevelInfo li)
    {
        //Set size
        GetComponent<BoxCollider2D>().size = li.CurrentGridSize;
        //Set sprite
        GetComponent<SpriteRenderer>().size = li.CurrentGridSize;
        //Set memory
        gridMemori = li.Pattern;
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
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + board.CurrentLevelGridSize.x / 2,
                (int)SnapToGrid(item.transform.position).y + board.CurrentLevelGridSize.y / 2);

            //Bounderi
            if (gridPos.x < 0 || gridPos.y < 0 ||
                gridPos.x >= board.CurrentLevelGridSize.x ||
                gridPos.y >= board.CurrentLevelGridSize.y)
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
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + board.CurrentLevelGridSize.x / 2,
                (int)SnapToGrid(item.transform.position).y + board.CurrentLevelGridSize.y / 2);

            gridMemori[gridPos.x, gridPos.y] = item;
            item.GetComponent<Dot>().gridPos = gridPos;
        }

        bool makedGoal = CheckGoals();

        bool allDone = true;
        foreach (GameObject item in board.currentLevelsGoals)
        {
            GoalTable goalTable = item.GetComponent<GoalTable>();

            if (!goalTable.completet)
            {
                allDone = false;
                break;
            }
        }
        //Is goal maked or level complete?
        if (allDone)
        {
            BoardsSounds.Instance.CompletLevel();
            nextLevel = true;
        }
        else if (makedGoal)
            BoardsSounds.Instance.GoalComplete();

        return true;
    }

    public DotTable TakeFromBoard(Vector2 clickPosition)
    {
        Vector2 gridPosition = SnapToGrid(clickPosition);
        DotTable table = null;

        //Take table from board
        if (gridMemori[(int)gridPosition.x + board.CurrentLevelGridSize.x / 2,
            (int)gridPosition.y + board.CurrentLevelGridSize.y / 2] != null)
        {
            table = gridMemori[(int)gridPosition.x + board.CurrentLevelGridSize.x / 2,
                (int)gridPosition.y + board.CurrentLevelGridSize.y / 2].GetComponent<Dot>().ownerTable;

            //See if it was part of complete goal
            if (table.pieceInGoal != null)
                table.PickupTable();        

            //Remove dots from memori
            for (int i = 0; i < table.content.Count; i++)
            {
                Vector2Int removePos = table.content[i].GetComponent<Dot>().gridPos;
                gridMemori[removePos.x, removePos.y] = null;
            }
        }

        return table;
    }

    private bool CheckGoals()
    {
        bool result = false;

        for (int x = 0; x < gridMemori.GetLength(0); x++)
        {
            for (int y = 0; y < gridMemori.GetLength(1); y++)
            {
                //Check goals
                foreach (GameObject item in board.currentLevelsGoals)
                {
                    GoalTable goal = item.GetComponent<GoalTable>();

                    //Bouncs
                    if (goal.pattern.GetLength(0) + x > gridMemori.GetLength(0)
                        || goal.pattern.GetLength(1) + y > gridMemori.GetLength(1))
                        break;

                    //Pieces there are part of the goal
                    List<DotTable> pieces = new List<DotTable>();

                    //See if there is a match
                    bool match = true;
                    for (int i = 0; i < goal.pattern.GetLength(0); i++)
                    {
                        if (!match)
                            break;

                        for (int j = 0; j < goal.pattern.GetLength(1); j++)
                        {
                            //Spaces in goals
                            if (goal.pattern[i, j] == null)
                                continue;

                            Dot checking;
                            if (gridMemori[x + i, y + j] != null)
                                checking = gridMemori[x + i, y + j].GetComponent<Dot>();
                            else
                            {
                                checking = null;
                                match = false;
                                break;
                            }

                            if (checking.type != goal.pattern[i, j].GetComponent<Dot>().type)
                            {
                                match = false;
                                break;
                            }

                            if (checking.ownerTable != null && !pieces.Contains(checking.ownerTable))
                                pieces.Add(checking.ownerTable);
                        }
                    }

                    //If there was a match
                    if (match)
                    {
                        //Goal completet
                        goal.GoalCompletet();
                        result = true;

                        //Tell then uncompeltet
                        //goal.undoGoal += UncompleteGoal;
                        goal.subsubscribers = pieces;
                        foreach (DotTable p in pieces)
                        {
                            //p.pickedupAction += goal.GoalUncomplet;
                            p.pieceInGoal = goal;
                        }
                    }
                }
            }
        }

        return result;
    }
}

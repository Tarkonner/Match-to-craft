using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : SerializedMonoBehaviour
{
    [SerializeField] Board board;

    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridsUpperLeftCorner;

    //Memori
    private GameObject[,] gridMemori = new GameObject[0, 0];


    [SerializeField] private GameObject dotHolder;

    private bool nextLevel = false;
    private float sceneTransistenTime = 1;
    private float sceneClock;

    [SerializeField] private GameObject[,] debugGrid;

    private void Update()
    {
        if (nextLevel)
        {
            sceneClock += Time.deltaTime;
            if (sceneClock >= sceneTransistenTime)
            {
                nextLevel = false;
                board.NextLevel();
                sceneClock = 0;
            }
        }

        debugGrid = gridMemori;
    }


    public void SetupPattorn(LevelInfo li)
    {
        Vector2 cornorPos = Vector2.zero;
        cornorPos.x = (li.CurrentGridSize.x % 2 == 0) ? -li.CurrentGridSize.x / 2 * sizeOfGrid + sizeOfGrid / 2
            : -li.CurrentGridSize.x / 2 * sizeOfGrid;
        cornorPos.y = (li.CurrentGridSize.y % 2 == 0) ? li.CurrentGridSize.y / 2 * sizeOfGrid - sizeOfGrid / 2
            : li.CurrentGridSize.y / 2 * sizeOfGrid;
        gridsUpperLeftCorner = cornorPos;

        //Set size
        GetComponent<BoxCollider2D>().size = li.CurrentGridSize;
        //Set sprite
        GetComponent<SpriteRenderer>().size = li.CurrentGridSize;

        //Clear memory
        for (int x = 0; x < gridMemori.GetLength(0); x++)
        {
            for (int y = 0; y < gridMemori.GetLength(0); y++)
            {
                //Remove old dots
                if (gridMemori[x, y] != null)
                    Destroy(gridMemori[x, y]);
            }
        }
        //Load levels pattorn
        gridMemori = new GameObject[li.CurrentGridSize.x, li.CurrentGridSize.y];

        //New dots
        for (int x = 0; x < li.Pattern.GetLength(0); x++)
        {
            for (int y = 0; y < li.Pattern.GetLength(1); y++)
            {
                //Place dot
                if (li.Pattern[x, y] != null)
                {
                    //Dot
                    GameObject spawn = Instantiate(li.Pattern[x, y], transform);
                    spawn.transform.localPosition = cornorPos + new Vector2(x, -y) * sizeOfGrid;
                    //Place in memori
                    gridMemori[x, li.Pattern.GetLength(1) - 1 - y] = spawn;

                    //Holder
                    GameObject holder = Instantiate(dotHolder, spawn.transform);
                    holder.transform.localPosition = Vector2.zero;
                }
            }
        }
    }

    private Vector2 SnapToGrid(Vector2 targetPosition)
    {
        Vector2 result;
        result.x = (board.CurrentLevelGridSize.x % 2 != 0) ?
            Mathf.Round(targetPosition.x) :
            Mathf.Round(targetPosition.x - sizeOfGrid / 2) + sizeOfGrid / 2;
        result.y = (board.CurrentLevelGridSize.y % 2 != 0) ?
            Mathf.Round(targetPosition.y) :
            Mathf.Round(targetPosition.y - sizeOfGrid / 2) + sizeOfGrid / 2;

        return result;
    }

    private Vector2Int SnapToGridInt(Vector2 targetPosition)
    {
        Vector2Int result = Vector2Int.zero;
        result.x = (board.CurrentLevelGridSize.x % 2 != 0) ?
            (int)Mathf.Round(targetPosition.x / sizeOfGrid) :
            (int)Mathf.Round(targetPosition.x / sizeOfGrid - sizeOfGrid / 2);
        result.y = (board.CurrentLevelGridSize.y % 2 != 0) ?
            (int)Mathf.Round(targetPosition.y / sizeOfGrid) :
            (int)Mathf.Round(targetPosition.y / sizeOfGrid - sizeOfGrid / 2);

        return result;
    }

    public bool PlaceDots(DotTable targetTable)
    {
        //Check if allowed
        foreach (GameObject item in targetTable.content)
        {
            Vector2Int gridPos = SnapToGridInt(item.transform.position) + board.CurrentLevelGridSize / 2;

            //Bounderi
            if (gridPos.x < 0 || gridPos.y < 0 ||
                gridPos.x >= board.CurrentLevelGridSize.x ||
                gridPos.y >= board.CurrentLevelGridSize.y)
                return false;

            //Is there already something
            if (gridMemori[gridPos.x, gridPos.y] != null)
                return false;
        }

        //Place dots in memory
        foreach (GameObject item in targetTable.content)
        {
            Vector2Int dotPos = SnapToGridInt(item.transform.position) + board.CurrentLevelGridSize / 2;

            gridMemori[dotPos.x, dotPos.y] = item;
            item.GetComponent<Dot>().gridPos = dotPos;
        }

        //Snap table to grid
        targetTable.transform.position = (Vector3)SnapToGrid(targetTable.transform.position);

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
        Vector2Int gridPosition = SnapToGridInt(clickPosition + board.CurrentLevelGridSize / 2);
        DotTable table = null;

        //Take table from board
        if (gridMemori[gridPosition.x, gridPosition.y] != null)
        {
            table = gridMemori[gridPosition.x, gridPosition.y].GetComponent<Dot>().ownerTable;

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

        //if(gridMemori[0, 0] != null)
        //    Debug.Log(gridMemori[0, 0].gameObject.name);

        foreach (GameObject item in board.currentLevelsGoals)
        {
            GoalTable goal = item.GetComponent<GoalTable>();

            for (int y = 0; y < gridMemori.GetLength(1); y++)
            {
                for (int x = 0; x < gridMemori.GetLength(0); x++)
                {
                    //Don't look at empty spaces
                    if (gridMemori[x, y] == null)
                        continue;

                    //Bouncs
                    if (goal.Pattern.GetLength(0) + x > gridMemori.GetLength(0)
                        || goal.Pattern.GetLength(1) + y > gridMemori.GetLength(1))
                        break;

                    //Pieces there are part of the goal
                    List<DotTable> pieces = new List<DotTable>();

                    //See if there is a match
                    bool match = true;
                    for (int l = 0; l < goal.Pattern.GetLength(1); l++)
                    {                     
                        //No match here
                        if (!match)
                            break;

                        for (int v = 0; v < goal.Pattern.GetLength(0); v++)
                        {
                            //Spaces in goals
                            if (goal.Pattern[v, l] == null)
                                continue;

                            //Checking for match
                            int targetY = y + ((goal.Pattern.GetLength(1) - 1) - l);
                            if (gridMemori[x + v, targetY] != null)
                            {
                                Dot checking = gridMemori[x + v, targetY].GetComponent<Dot>();
                                if(checking.type != goal.Pattern[v, l].GetComponent<Dot>().type)
                                {
                                    match = false;
                                    break;
                                }
                                else if (checking.ownerTable != null && !pieces.Contains(checking.ownerTable))
                                    pieces.Add(checking.ownerTable);
                            }
                            else
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    //If there was a match
                    if (match)
                    {
                        if(!goal.completet)
                        {
                            //Goal completet
                            goal.GoalCompletet();
                            result = true;

                            //Tell then uncompeltet
                            goal.subsubscribers = pieces;
                            foreach (DotTable p in pieces)
                                p.pieceInGoal = goal;
                        }
                    }
                }
            }
        }

        return result;
    }
}

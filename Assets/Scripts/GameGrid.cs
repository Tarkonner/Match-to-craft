using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : SerializedMonoBehaviour
{
    [SerializeField] Board board;

    private const float sizeOfGrid = 1;
    private Vector2 gridsUpperLeftCorner;

    //Memori
    private GameObject[,] gridMemori = new GameObject[0, 0];


    [SerializeField] private GameObject dotHolder;
    [SerializeField] private GameObject gridField;

    public bool nextLevel { get; private set; } = false;
    private float sceneClock;


    private PlacementGoal placementGoal;

    [SerializeField] private GameObject[,] debugGrid;

    private void Update()
    {
        if (nextLevel)
        {
            sceneClock += Time.deltaTime;
            if (sceneClock >= TweeningAnimations.Instance.TransistenTime)
            {
                nextLevel = false;
                board.NextLevel();
                sceneClock = 0;

                Mouse.Instance.CantTakePieces = true;
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

        //Clear memory
        for (int x = 0; x < gridMemori.GetLength(0); x++)
        {
            for (int y = 0; y < gridMemori.GetLength(1); y++)
            {
                //Remove old level
                if (gridMemori[x, y] != null)
                    Destroy(gridMemori[x, y]);
            }
        }
        //Clear goals
        if (placementGoal != null)
        {
            for (int i = placementGoal.boardGoals.Count - 1; i >= 0; i--)
                Destroy(placementGoal.boardGoals[i].gameObject);
            placementGoal = null;
        }

        //Make grid
        //Animation
        List<Transform> toTween = new List<Transform>();
        //Remove old
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        //Make new grid
        for (int x = 0; x < li.CurrentGridSize.x; x++)
        {
            for (int y = 0; y < li.CurrentGridSize.y; y++)
            {
                GameObject spawn = Instantiate(gridField, transform);
                spawn.transform.position = cornorPos + new Vector2(x, -y) * sizeOfGrid;

                //Add to animation
                toTween.Add(spawn.transform);
            }
        }

        //Load levels pattorn
        gridMemori = new GameObject[li.CurrentGridSize.x, li.CurrentGridSize.y];

        //New level
        for (int x = 0; x < li.Pattern.GetLength(0); x++)
        {
            for (int y = 0; y < li.Pattern.GetLength(1); y++)
            {
                //Place dot
                if (li.Pattern[x, y] != null)
                {
                    //Spawn item
                    GameObject spawn = Instantiate(li.Pattern[x, y], transform);
                    spawn.transform.localPosition = cornorPos + new Vector2(x, -y) * sizeOfGrid;

                    //Dots
                    if (spawn.TryGetComponent(out Dot d))
                    {
                        //Place in memori
                        gridMemori[x, li.Pattern.GetLength(1) - 1 - y] = spawn;
                        //Holder for dots
                        GameObject holder = Instantiate(dotHolder, spawn.transform);
                        holder.transform.localPosition = Vector2.zero;
                    }
                    //Place goal
                    if (spawn.TryGetComponent(out GridPlaceGoal gpg))
                    {
                        gpg.Setup(board, new Vector2Int(x, li.Pattern.GetLength(1) - 1 - y));
                    }

                    //Add to animation
                    toTween.Add(spawn.transform);
                }
            }
        }
        //Start animation
        TweeningAnimations.Instance.EasingAnimation(toTween, true);

        //Special goals
        foreach (Transform item in board.goalHolder.transform)
        {
            if (item.gameObject.TryGetComponent(out PlacementGoal g))
            {
                placementGoal = g;
            }
        }
        if (placementGoal != null)
            placementGoal.UpdateScore(0);
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

        //Check goals
        bool makedGoal = CheckGoals();
        bool allGoalsDone = true;
        foreach (GameObject item in board.currentLevelsGoals)
        {
            GameGoal goalTable = item.GetComponent<GameGoal>();

            if (!goalTable.GoalState())
            {
                allGoalsDone = false;
                break;
            }
        }
        //Is goal maked or level complete?
        if (allGoalsDone && !nextLevel)
        {
            BoardsSounds.Instance.CompletLevel();
            nextLevel = true;
            Mouse.Instance.CantTakePieces = false;

            //Out animation
            TweeningAnimations.Instance.EaseOut();
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

            //See if there was placement goal
            if (placementGoal != null)
                PlacementGoalUpdate();
        }

        return table;
    }

    private bool CheckGoals()
    {
        bool result = false;

        //Match with goals
        foreach (GameObject item in board.currentLevelsGoals)
        {
            //Matching goal
            if (item.TryGetComponent(out GoalTable goal))
            {
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
                                    if (checking.type != goal.Pattern[v, l].GetComponent<Dot>().type)
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
                            if (!goal.GoalState())
                            {
                                //Goal completet
                                goal.GoalComplete();
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
            //Placing goal
            if (item.TryGetComponent(out PlacementGoal pg))
            {
                PlacementGoalUpdate();
                result = placementGoal.GoalState();
            }

        }

        return result;
    }

    void PlacementGoalUpdate()
    {
        int score = 0;
        for (int i = 0; i < placementGoal.boardGoals.Count; i++)
        {
            Vector2Int targetPlacement = placementGoal.boardGoals[i].GridPlacement;
            if (gridMemori[targetPlacement.x, targetPlacement.y] != null)
            {
                GridPlaceGoal checker = placementGoal.boardGoals[i];
                Dot candidate = gridMemori[targetPlacement.x, targetPlacement.y].GetComponent<Dot>();
                if (checker.OpinionFilledGoal == dotType.None
                    || checker.OpinionFilledGoal == candidate.type)
                {
                    score++;
                }
            }
        }

        //Update UI and tell if goal is complete
        placementGoal.UpdateScore(score);
        if (placementGoal.boardGoals.Count == score && !placementGoal.GoalState())
        {
            placementGoal.GoalComplete();
        }
        else if (placementGoal.GoalState() && placementGoal.boardGoals.Count != score)
        {
            placementGoal.GoalUncomleted();
        }
    }
}

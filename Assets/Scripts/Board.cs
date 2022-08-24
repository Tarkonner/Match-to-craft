using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    //Board
    [SerializeField] private Vector2Int gridSize = new Vector2Int(3, 3);
    //private Vector2 pointZero;
    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridPosition;


    [SerializeField] private GameObject dotPrefab;

    [HideInInspector] public DotTable holdingTable;
    private GameObject holdingGameObject;
    private bool mouseOnBoard = false;

    //Memori
    private Dot[,] gridMemori;

    [SerializeField] private List<GoalTable> goals;

    void Start()
    {
        Instance = this;

        gridMemori = new Dot[gridSize.x, gridSize.y];

        sr = GetComponent<SpriteRenderer>();
        sr.size = gridSize;

        //pointZero = new Vector2(-gridSize.x / 2, -gridSize.y / 2);

        //Collider
        GetComponent<BoxCollider2D>().size = gridSize;
    }

    void Update()
    {
        if (!mouseOnBoard)
            return;

        //Boards boarder
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        gridPosition = SnapToGrid(mousePos);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (holdingGameObject == null)
            {
                //Take table from board
                if (gridMemori[(int)gridPosition.x + 2, (int)gridPosition.y + 2] != null)
                {
                    DotTable table = gridMemori[(int)gridPosition.x + 2, (int)gridPosition.y + 2].ownerTable;
                    table.followMouse = true;
                    TablePickup(gridMemori[(int)gridPosition.x + 2, (int)gridPosition.y + 2].ownerTable.gameObject);

                    //Remove dots from memori
                    for (int i = 0; i < table.content.Count; i++)
                    {
                        Vector2Int removePos = table.content[i].GetComponent<Dot>().gridPos;
                        gridMemori[removePos.x, removePos.y] = null;
                    }
                }
            }
            else
            {
                //Place
                bool result = PlaceDots();
                if (result)
                {
                    holdingTable.followMouse = false;
                    TableDrop();
                }

                //Check Goals
                CheckGoals();
            }
        }
    }

    private Vector2 SnapToGrid(Vector2 targetPosition)
    {
        return new Vector2(
            Mathf.Round(targetPosition.x / sizeOfGrid) * sizeOfGrid,
            Mathf.Round(targetPosition.y / sizeOfGrid) * sizeOfGrid);
    }

    private bool PlaceDots()
    {
        //Check if allowed
        foreach (GameObject item in holdingTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + 2,
                (int)SnapToGrid(item.transform.position).y + 2);

            //Bounderi
            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= gridSize.x || gridPos.y >= gridSize.y)
                return false;

            //Is there already something
            if (gridMemori[gridPos.x, gridPos.y] != null)
                return false;
        }

        //Snap table to grid
        holdingTable.transform.position = (Vector3)SnapToGrid(holdingTable.transform.position);

        //Place dots in memory
        foreach (GameObject item in holdingTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + 2,
                (int)SnapToGrid(item.transform.position).y + 2);

            Dot targetDot = item.GetComponent<Dot>();
            gridMemori[gridPos.x, gridPos.y] = targetDot;
            targetDot.gridPos = gridPos;
        }

        return true;
    }

    private void CheckGoals()
    {
        for (int x = 0; x < gridMemori.GetLength(0); x++)
        {
            for (int y = 0; y < gridMemori.GetLength(1); y++)
            {
                foreach (GoalTable goal in goals)
                {
                    if (goal.pattern.GetLength(0) + x > gridMemori.GetLength(0)
                        || goal.pattern.GetLength(1) + y > gridMemori.GetLength(1))
                        continue;

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
                        }
                    }

                    if (match)
                        goal.GoalCompletet();
                }
            }
        }
    }

    public void TablePickup(GameObject table)
    {
        holdingGameObject = table;
        holdingTable = table.GetComponent<DotTable>();
        holdingTable.HighlightTable();
    }

    public void TableDrop()
    {
        holdingTable.NormalTable();
        holdingGameObject = null;
        holdingTable = null;
    }

    private void OnMouseEnter()
    {
        mouseOnBoard = true;
    }

    private void OnMouseExit()
    {
        mouseOnBoard = false;
    }
}

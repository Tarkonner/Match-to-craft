using System;
using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }

    //Board
    [SerializeField] private Vector2Int gridSize = new Vector2Int(3, 3);
    private Vector2 pointZero;
    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridPosition;


    [SerializeField] private GameObject dotPrefab;

    public DotTable holdingTable;
    private GameObject holdingGameObject;
    private bool mouseOnBoard = false;

    //Memori
    private Dot[,] gridMemori;

    void Start()
    {
        Instance = this;

        gridMemori = new Dot[gridSize.x, gridSize.y];

        sr = GetComponent<SpriteRenderer>();
        sr.size = gridSize;

        pointZero = new Vector2(-gridSize.x / 2, -gridSize.y / 2);

        //Collider
        GetComponent<BoxCollider2D>().size = gridSize;
    }

    void Update()
    {
        //Boards boarder
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mouseOnBoard)
        {
            gridPosition = SnapToGrid(mousePos);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if(holdingGameObject == null)
                {
                    Vector2 testPos = gridPosition + new Vector2(2, 2);
                    if (gridMemori[(int)testPos.x, (int)testPos.y] == null)
                        Debug.Log(testPos);
                    else
                        Debug.Log(gridMemori[(int)testPos.x, (int)testPos.y]);

                    //Take table from board
                    if (gridMemori[(int)gridPosition.x + 2, (int)gridPosition.y + 2] != null)
                    {                        
                        gridMemori[(int)gridPosition.x + 2, (int)gridPosition.y + 2].ownerTable.followMouse = true;
                        TablePickup(gridMemori[(int)gridPosition.x + 2, (int)gridPosition.y + 2].ownerTable.gameObject);
                    }
                }
                else
                {
                    //Place
                    bool result = PlaceDots();
                    if (result)
                    {
                        holdingTable.followMouse = false;

                        holdingGameObject = null;
                        holdingTable = null;
                    }
                }


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

        foreach (GameObject item in holdingTable.content)
        {
            item.transform.position = (Vector3)SnapToGrid(item.transform.position);
        }

        //Copy dots from table
        foreach (GameObject item in holdingTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + 2,
                (int)SnapToGrid(item.transform.position).y + 2);
                       
            gridMemori[gridPos.x, gridPos.y] = item.GetComponent<Dot>();

            Debug.Log(gridMemori[gridPos.x, gridPos.y]);
        }

        return true;
    }

    public void TablePickup(GameObject table)
    {
        holdingGameObject = table;
        holdingTable = table.GetComponent<DotTable>();
    }

    public void TableDrop()
    {
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

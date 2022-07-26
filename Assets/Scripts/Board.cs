using System;
using UnityEngine;

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
    [SerializeField] private GameObject mouseShower;

    public DotTable holding;
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
            gridPosition = new Vector2(
                Mathf.Round(mousePos.x / sizeOfGrid) * sizeOfGrid,
                Mathf.Round(mousePos.y / sizeOfGrid) * sizeOfGrid);

            mouseShower.transform.position = gridPosition;

            if (Input.GetKeyDown(KeyCode.Mouse0) && holding != null)
            {
                //Place
                bool result = PlaceDot(new Vector2Int((int)(gridPosition.x - pointZero.x), (int)(gridPosition.y - pointZero.y)));
                if (result)
                {
                    holding.gameObject.SetActive(false);
                    holding = null;
                }
            }

        }
    }



    private bool PlaceDot(Vector2Int position)
    {
        Vector2Int[,] calculations = new Vector2Int[holding.content.GetLength(1), holding.content.GetLength(0)];

        for (int x = 0; x < holding.content.GetLength(1); x++)
        {
            for (int y = 0; y < holding.content.GetLength(0); y++)
            {
                if (holding.content[x, y] == null)
                    continue;

                Vector2Int cal = position - Vector2Int.one + new Vector2Int(x, y);

                //Bounderi
                if (cal.x < 0 || cal.y < 0 || cal.x >= gridSize.x ||cal.y >= gridSize.y)
                    return false;

                //Is there already something
                if (gridMemori[cal.x, cal.y] != null)
                    return false;         
                
                //Save calculation
                calculations[x, y] = cal;
            }
        }

        //Placeing dots
        for (int x = 0; x < holding.content.GetLength(1); x++)
        {
            for (int y = 0; y < holding.content.GetLength(0); y++)
            {
                if (holding.content[x, y] == null)
                    continue;

                GameObject spawn = Instantiate(holding.content[x, y].gameObject, transform);
                spawn.transform.position = pointZero + ((Vector2)calculations[x, y] * sizeOfGrid); ;
                gridMemori[calculations[x, y].x, calculations[x, y].y] = spawn.GetComponent<Dot>();
            }
        }

        return true;
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

using UnityEngine;

public class Board : MonoBehaviour
{
    //Board
    [SerializeField] private Vector2Int gridSize = new Vector2Int(3, 3);
    private Vector2 pointZero;
    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridPosition;


    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject mouseShower;

    private DotTable holding;
    private bool mouseOnBoard = false;

    //Memori
    private Dot[,] gridMemori;

    void Start()
    {
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

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                bool result = PlaceDot(new Vector2Int((int)(gridPosition.x - pointZero.x), (int)(gridPosition.y - pointZero.y)));
                if (!result)
                    Debug.Log("not empty");
            }

        }
    }



    private bool PlaceDot(Vector2Int position)
    {
        if (gridMemori[position.x, position.y] == null)
        {
            GameObject spawn = Instantiate(dotPrefab);
            Dot dot = spawn.GetComponent<Dot>();
            spawn.transform.position = pointZero + ((Vector2)position * sizeOfGrid);
            gridMemori[position.x, position.y] = dot;
            dot.Setup(dotType.Red);

            return true;
        }
        else
            return false;
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

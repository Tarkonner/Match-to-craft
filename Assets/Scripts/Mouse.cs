using UnityEngine;

public class Mouse : MonoBehaviour
{
    public Mouse Instance { get; private set; }

    //Holding
    public DotTable holdingTable;


    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        //Make table follow mouse
        if (holdingTable != null)
        {
            Vector2 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            holdingTable.transform.position = camPos;
            holdingTable.HoldingUpdate();
        }

        //Left mouse button
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit.collider == null)
                return;

            if (holdingTable == null)
            {
                if (hit.collider.TryGetComponent(out DotTable table))
                {
                    holdingTable = table;
                    table.PickupTable();
                    table.HighlightTable();
                }
                else if(hit.collider.TryGetComponent(out Board board))
                {
                    DotTable result = board.TakeFromBoard(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (result != null)
                    {
                        holdingTable = result;
                        result.HighlightTable();
                    }
                }

            }
            else
            {               
                if (hit.collider.TryGetComponent(out Board board))
                {
                    bool result = board.PlaceDots(holdingTable);

                    if (result)
                    {
                        holdingTable.DropTable();
                        holdingTable = null;
                    }
                }
            }
        }

        //Right mouse button
        if (Input.GetKeyDown(KeyCode.Mouse1) && holdingTable != null)
        {
            holdingTable.RotateTable();
        }

        //Middle mouse buttom
        if (Input.GetKeyDown(KeyCode.Mouse2) && holdingTable != null)
        {
            holdingTable.ResetTable();
            holdingTable.DropTable();
            holdingTable = null;
        }
    }
}

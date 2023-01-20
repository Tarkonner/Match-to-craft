using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;


public class DotTable : InspectorGrid
{
    private BoxCollider2D col;

    [SerializeField] private float inventoryScale = 0.5f;

    public List<GameObject> content { get; private set; } = new List<GameObject>();

    public int tableSize { get; private set; } = 3;
    

    [SerializeField] private GameObject lineLink;

    private Vector2 startPosition;

    //Rotation
    private float targetRotation;
    [SerializeField] private float rotateSpeed = 5;
    private float rotateAmount = 1;
    private float oldRotation;

    [HideInInspector] public GoalTable pieceInGoal;

    [Header("Not rotate")]
    [SerializeField] private bool canNotRotate;
    public bool CanNotRotate { get => canNotRotate; set => canNotRotate = value; }
    //[SerializeField] private Sprite dotsRotateSprite;

    void Start()
    {
        startPosition = transform.position;

        col = GetComponent<BoxCollider2D>();

        //Add dots to memory
        foreach (Transform t in transform)
        {
            if (t.GetComponent<Dot>())
            {
                content.Add(t.gameObject);
                t.GetComponent<Dot>().ownerTable = this;

                //if(canNotRotate)
                //    t.GetComponent<SpriteRenderer>().sprite = dotsRotateSprite;
            }
        }
    }

    public void HoldingUpdate()
    {
        rotateAmount += rotateSpeed * Time.deltaTime;
        if (rotateAmount > 1)
            rotateAmount = 1;

        transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(oldRotation, targetRotation, rotateAmount));
    }

    public void RotateTable()
    {
        if(rotateAmount == 1)
        {
            rotateAmount = 0;
            oldRotation = transform.eulerAngles.z;
            targetRotation = transform.eulerAngles.z - 90;
        }
    }

    public void ResetTable()
    {
        col.enabled = true;

        //Back to noraml
        transform.position = startPosition;
        transform.localScale = new Vector2(inventoryScale, inventoryScale);
        //Rotation
        transform.localEulerAngles = Vector3.zero;
        targetRotation = 0;

        RemoveHighlight();
    }

    public void PickupTable()
    {
        //Follow mouse
        col.enabled = false;
        transform.localScale = Vector3.one;

        //Tell if goal is uncomplete
        if (pieceInGoal != null)
            pieceInGoal.GoalUncomleted();
    }

    public void DropTable()
    {
        RemoveHighlight();
    }

    public void HighlightTable()
    {
        foreach (Transform t in gameObject.transform)
        {
            if (t.gameObject.TryGetComponent(out SpriteRenderer targetSR))
                targetSR.sortingOrder = 6;

            if (t.gameObject.TryGetComponent(out LineRenderer line))
                line.sortingOrder = 4;
        }
    }

    public void RemoveHighlight()
    {
        foreach (Transform t in gameObject.transform)
        {
            if (t.gameObject.TryGetComponent(out SpriteRenderer targetSR))
                targetSR.sortingOrder = 3;

            if (t.gameObject.TryGetComponent(out LineRenderer line))
                line.sortingOrder = 1;
        }
    }

    #region BuildPrefab
    [FoldoutGroup("Pattorn")]
    [Button("Place dots")]
    public override void SpawnDots()
    {
        //Place dots
        base.SpawnDots();
        Vector2 cornorPos = new Vector2(-1, 1);

        for (int x = 0; x < pattern.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                if (pattern[x, y] == null)
                    continue;

                bool foundNeighbor = false;

                Vector2 calPosition = cornorPos + new Vector2(x, -y);

                //Horizontal and vertical
                //Right
                if (x + 1 < pattern.GetLength(0) && 
                    pattern[x + 1, y] != null)
                {
                    foundNeighbor = true;

                    ConnectLine(calPosition, Vector2.right);
                }
                //Down
                if (y + 1 < pattern.GetLength(1) && 
                    pattern[x, y + 1] != null)
                {
                    foundNeighbor = true;

                    ConnectLine(calPosition, Vector2.down);
                }

                if (!foundNeighbor)
                {
                    //Down right
                    if (y + 1 < pattern.GetLength(1) &&
                        x + 1 < pattern.GetLength(0) &&
                        pattern[x + 1, y + 1] != null)
                    {
                        ConnectLine(calPosition, new Vector2(1, -1));                        
                    }
                    //Down left
                    if (y + 1 < pattern.GetLength(1) &&
                        x - 1 >= 0 &&
                        pattern[x - 1, y + 1] != null)
                    {
                        ConnectLine(calPosition, new Vector2(-1, -1));
                    }
                }
            }
        }
    }



    private GameObject ConnectLine(Vector2 spawnPosition, Vector2 direction)
    {
        GameObject spawn = Instantiate(lineLink, transform);
        spawn.transform.localPosition = spawnPosition;

        //Set line
        spawn.GetComponent<TableLinkSetup>().DrawDirection(direction, canNotRotate);

        return spawn;
    }
    #endregion
}

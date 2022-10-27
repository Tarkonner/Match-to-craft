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
    private float rotateAmount;
    private float oldRotation;

    [HideInInspector] public GoalTable pieceInGoal;

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
        rotateAmount = 0;
        oldRotation = transform.eulerAngles.z;
        targetRotation = transform.eulerAngles.z - 90;
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
        {
            pieceInGoal.GoalUncomplet();
        }
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
                targetSR.sortingOrder = 4;

            if (t.gameObject.TryGetComponent(out LineRenderer line))
                line.sortingOrder = 3;
        }
    }

    public void RemoveHighlight()
    {
        foreach (Transform t in gameObject.transform)
        {
            if (t.gameObject.TryGetComponent(out SpriteRenderer targetSR))
                targetSR.sortingOrder = 2;

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

        List<GameObject> connectedDots = new List<GameObject>();
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
                if (x + 1 < pattern.GetLength(0) && pattern[x + 1, y] != null)
                {
                    foundNeighbor = true;

                    if (!connectedDots.Contains(pattern[x + 1, y]))
                        connectedDots.Add(pattern[x + 1, y]);
                    if (!connectedDots.Contains(pattern[x, y]))
                        connectedDots.Add(pattern[x, y]);

                    ConnectLine(calPosition, Vector2.right);
                }
                //Left
                if (x - 1 >= 0 && pattern[x - 1, y] != null)
                {
                    foundNeighbor = true;

                    if (!connectedDots.Contains(pattern[x - 1, y]))
                        connectedDots.Add(pattern[x - 1, y]);
                    if (!connectedDots.Contains(pattern[x, y]))
                        connectedDots.Add(pattern[x, y]);

                    ConnectLine(calPosition, Vector2.left);
                }
                //Up
                //if (y - 1 >= 0 && pattern[x, y - 1] != null)
                //{
                //    foundNeighbor = true;

                //    if (!connectedDots.Contains(pattern[x, y - 1]))
                //        connectedDots.Add(pattern[x, y - 1]);
                //    if (!connectedDots.Contains(pattern[x, y]))
                //        connectedDots.Add(pattern[x, y]);

                //    ConnectLine(calPosition, Vector2.up);
                //}
                //Down
                if (y + 1 < pattern.GetLength(1) && pattern[x, y + 1] != null)
                {
                    foundNeighbor = true;

                    if (!connectedDots.Contains(pattern[x, y + 1]))
                        connectedDots.Add(pattern[x, y + 1]);
                    if (!connectedDots.Contains(pattern[x, y]))
                        connectedDots.Add(pattern[x, y]);

                    ConnectLine(calPosition, Vector2.down);
                }

                if (!foundNeighbor)
                {
                    //Down right
                    if (y + 1 < pattern.GetLength(1) &&
                        x + 1 < pattern.GetLength(0) &&
                        pattern[x + 1, y + 1] != null)
                    {
                        if (!connectedDots.Contains(pattern[x + 1, y + 1]))
                            connectedDots.Add(pattern[x + 1, y + 1]);
                        if (!connectedDots.Contains(pattern[x, y]))
                            connectedDots.Add(pattern[x, y]);

                        ConnectLine(calPosition, new Vector2(1, -1));
                    }

                    //Down left
                    if (y + 1 < pattern.GetLength(1) &&
                        x - 1 >= 0 &&
                        pattern[x - 1, y + 1] != null)
                    {
                        if (!connectedDots.Contains(pattern[x - 1, y + 1]))
                            connectedDots.Add(pattern[x - 1, y + 1]);
                        if (!connectedDots.Contains(pattern[x, y]))
                            connectedDots.Add(pattern[x, y]);

                        ConnectLine(calPosition, new Vector2(-1, -1));
                    }
                }
            }
        }
    }



    private void ConnectLine(Vector2 spawnPosition, Vector2 direction)
    {
        GameObject spawn = Instantiate(lineLink, transform);
        spawn.transform.localPosition = spawnPosition;

        //Set line
        LineRenderer line = spawn.GetComponent<LineRenderer>();
        line.SetPosition(0, new Vector3(0, 0, 0));
        line.SetPosition(1, Vector3.Lerp(Vector3.zero, direction, .25f));
        line.SetPosition(2, Vector3.Lerp(Vector3.zero, direction, .5f));
        line.SetPosition(3, Vector3.Lerp(Vector3.zero, direction, .75f));
        line.SetPosition(4, new Vector3(direction.x, direction.y, 0));
    }
    #endregion
}

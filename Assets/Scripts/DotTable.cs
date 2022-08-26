using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DotTable : SerializedMonoBehaviour
{
    private BoxCollider2D col;
    private SpriteRenderer sr;

    [SerializeField] private float inventoryScale = 0.5f;

    [BoxGroup("Pattern")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
    public GameObject[,] pattern = new GameObject[3, 3];

    public List<GameObject> content { get; private set; } = new List<GameObject>();

    public int tableSize { get; private set; } = 3;

    [SerializeField] private GameObject lineLink;

    private Vector2 startPosition;

    //Rotation
    private float targetRotation;
    [SerializeField] private float rotateSpeed = 5;
    private float rotateAmount;
    private float oldRotation;

    //Being moved
    public Action pickedupAction;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        foreach (Transform t in transform)
        {
            if (t.GetComponent<Dot>())
                content.Add(t.gameObject);
        }
    }

    public void HoldingUpdate()
    {
        rotateAmount += rotateSpeed * Time.deltaTime;
        if (rotateAmount > 1)
            rotateAmount = 1;

        transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(oldRotation, targetRotation, rotateAmount));
    }

    private void Update()
    {
        //if (followMouse)
        //{
        //    //Follow mouse
        //    Vector2 cal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    transform.position = cal;

        //    //Drop table
        //    if (Input.GetKeyDown(KeyCode.Mouse2))
        //    {
        //        Board.Instance.TableDrop();
        //        followMouse = false;
        //        col.enabled = true;

        //        //Back to noraml
        //        transform.position = startPosition;
        //        sr.enabled = true;
        //        transform.localScale = new Vector2(inventoryScale, inventoryScale);
        //        //Rotation
        //        transform.localEulerAngles = Vector3.zero;
        //        targetRotation = 0;
        //    }

        //    //Rotate table
        //    if (Input.GetKeyDown(KeyCode.Mouse1))
        //    {
        //        rotateAmount = 0;
        //        oldRotation = transform.eulerAngles.z;
        //        targetRotation = transform.eulerAngles.z - 90;
        //    }
        //    if(transform.rotation.z != targetRotation)
        //    {
        //        rotateAmount += rotateSpeed * Time.deltaTime;
        //        if(rotateAmount > 1)
        //            rotateAmount = 1;

        //        transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(oldRotation, targetRotation, rotateAmount));
        //    }
        //}
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
        sr.enabled = true;
        transform.localScale = new Vector2(inventoryScale, inventoryScale);
        //Rotation
        transform.localEulerAngles = Vector3.zero;
        targetRotation = 0;

        RemoveHighlight();
    }

    public void PickupTable()
    {
        //Save origon
        startPosition = transform.position;

        //Follow mouse
        //followMouse = true;
        col.enabled = false;
        transform.localScale = Vector3.one;

        //Disable background
        sr.enabled = false;
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

            if(t.gameObject.TryGetComponent(out LineRenderer line))
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
    [Button("Build pattorn")]
    private void BuildPattorn()
    {
        //Destroy old
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        //Make dots
        for (int x = 0; x < pattern.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                if (pattern[x, y] != null)
                {
                    GameObject spawn = Instantiate(pattern[x, y], transform);
                    Vector2 calPos = new Vector2(x - 1, y - 1);
                    spawn.transform.localPosition = calPos;
                    spawn.GetComponent<Dot>().ownerTable = this;
                }
            }
        }

        //Make lines
        //neighbor
        bool haveNeighbor = false;
        for (int x = 0; x < pattern.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.GetLength(1) - 1; y++)
            {
                if (pattern[x, y] == null)
                    continue;

                //Over
                if (pattern[x, y + 1] != null)
                {
                    ConnectLine(new Vector2(x, y), Vector2.up);
                    haveNeighbor = true;
                }
                //Right
                if (x < pattern.GetLength(0) - 1 && pattern[x + 1, y] != null)
                {
                    ConnectLine(new Vector2(x, y), Vector2.right);
                    haveNeighbor |= true;
                }
            }
        }
        //Diagonal* Lag scaleability
        if (!haveNeighbor)
        {
            for (int x = 0; x < pattern.GetLength(0); x++)
            {
                if (pattern[x, 1] == null)
                    continue;

                //Right up
                if (x < pattern.GetLength(0) - 1 && pattern[x + 1, 2] != null)
                    ConnectLine(new Vector2(x, 1), new Vector2(1, 1));
                //Right down
                if (x < pattern.GetLength(0) - 1 && pattern[x + 1, 2] != null)
                    ConnectLine(new Vector2(x, 1), new Vector2(1, -1));

                //Left up
                if (x > 0 && pattern[x - 1, 2] != null)
                    ConnectLine(new Vector2(x, 1), new Vector2(-1, 1));
                //Left down
                if (x > 0 && pattern[x - 1, 2] != null)
                    ConnectLine(new Vector2(x, 1), new Vector2(-1, -1));
            }
        }
    }

    private void ConnectLine(Vector2 spawnPosition, Vector2 direction)
    {
        GameObject spawn = Instantiate(lineLink, transform);
        spawn.transform.localPosition = spawnPosition - Vector2.one;
        LineRenderer line = spawn.GetComponent<LineRenderer>();
        line.SetPosition(0, new Vector3(0, 0, 0));
        line.SetPosition(1, new Vector3(direction.x, direction.y, 0));
    }
    #endregion
}

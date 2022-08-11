using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DotTable : SerializedMonoBehaviour
{
    private BoxCollider2D col;

    [BoxGroup("Pattern")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
    public GameObject[,] pattern = new GameObject[3, 3];

    public List<GameObject> content { get; private set; } = new List<GameObject>();

    private bool followMouse = false;

    public int tableSize { get; private set; } = 3;

    private Vector2 startPosition;


    void Start()
    {
        col = GetComponent<BoxCollider2D>();

        foreach (Transform t in transform)
        {
            content.Add(t.gameObject);
        }
    }

    [Button("Build pattorn")]
    private void BuildPattorn()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        for (int x = 0; x < pattern.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                if (pattern[x, y] != null)
                {
                    GameObject spawn = Instantiate(pattern[x, y], transform);
                    Vector2 calPos = new Vector2(x - 1, y - 1);
                    spawn.transform.localPosition = calPos;
                }
            }
        }
    }

    private void Update()
    {
        if(followMouse)
        {
            //Follow mouse
            Vector2 cal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cal;

            //Drop table
            if(Input.GetKeyDown(KeyCode.Mouse2))
            {
                Board.Instance.TableDrop();
                followMouse = false;
                col.enabled = true;
                transform.position = startPosition;
            }

            //Rotate table
            if(Input.GetKeyDown(KeyCode.Mouse1))
            {
                pattern = ClockwiseRotateTable(pattern);


                for (int x = 0; x < pattern.GetLength(0); x++)
                {
                    for (int y = 0; y < pattern.GetLength(1); y++)
                    {
                        if (pattern[x, y] != null)
                        {
                            Debug.Log($"{x}, {y}");

                            Debug.Log("Old: " + pattern[x, y].transform.localPosition);
                            Vector2 calPos = new Vector2(y - 1, x - 1);
                            pattern[x, y].transform.localPosition = calPos;

                            Debug.Log("New: " + pattern[x, y].transform.localPosition);
                        }
                    }
                }
            }
        }
    }

    private GameObject[,] ClockwiseRotateTable(GameObject[,] intake)
    {
        GameObject[,] result = new GameObject[intake.GetLength(0), intake.GetLength(1)];

        int j = 0;
        int p = 0;
        int q = 0;
        int i = intake.GetLength(0) - 1;

        for (int k = 0; k < intake.GetLength(0); k++)
        {
            while (i >= 0)
            {
                result[p, q] = intake[i, j];

                q++;
                i--;
            }
            j++;
            i = intake.GetLength(0) - 1;
            q = 0;
            p++;

        }
        return result;
    }

    private void OnMouseDown()
    {
        startPosition = transform.position;

        Board.Instance.TablePickup(gameObject);
        followMouse = true;
        col.enabled = false;
    }
}

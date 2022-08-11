using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DotTable : SerializedMonoBehaviour
{
    private BoxCollider2D col;

    [HorizontalGroup("Base")]
    [VerticalGroup("Base/colum1")] 
    public GameObject[] colum1 = new GameObject[3];
    [VerticalGroup("Base/colum2")]
    public GameObject[] colum2 = new GameObject[3];
    [VerticalGroup("Base/colum3")]
    public GameObject[] colum3 = new GameObject[3];

    public List<GameObject> content { get; private set; } = new List<GameObject>();

    private bool followMouse = false;

    public int tableSize { get; private set; } = 3;

    private Vector2 startPosition;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();

        for (int i = 0; i < 3; i++)
        {
            if (colum1[i] != null)
            {
                GameObject dot = Instantiate(colum1[i], transform);
                Vector2 calPosition = new Vector2(-1, 1 - i);
                dot.transform.localPosition = calPosition;
                content.Add(dot);
            }

            if (colum2[i] != null)
            {
                GameObject dot = Instantiate(colum2[i], transform);
                Vector2 calPosition = new Vector2(0, 1 - i);
                dot.transform.localPosition = calPosition;
                content.Add(dot);
            }

            if (colum3[i] != null)
            {
                GameObject dot = Instantiate(colum3[i], transform);
                Vector2 calPosition = new Vector2(1, 1- i);
                dot.transform.localPosition = calPosition;
                content.Add(dot);
            }
        }
    }


    private void Update()
    {
        if(followMouse)
        {
            Vector2 cal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cal;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                Board.Instance.TableDrop();
                followMouse = false;
                col.enabled = true;
                transform.position = startPosition;
            }

        }
    }


    private void OnMouseDown()
    {
        startPosition = transform.position;

        Board.Instance.TablePickup(gameObject);
        followMouse = true;
        col.enabled = false;
    }
}

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DotTable : SerializedMonoBehaviour
{
    private BoxCollider2D col;

    [BoxGroup("Content")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
    public GameObject[,] content = new GameObject[3, 3];

    private bool followMouse = false;

    public int tableSize { get; private set; } = 3;


    void Awake()
    {
        col = GetComponent<BoxCollider2D>();

        for (int x = 0; x < content.GetLength(0); x++)
        {
            for (int y = 0; y < content.GetLength(1); y++)
            {
                if (content[x, y] == null)
                    continue;

                GameObject spawn = Instantiate(content[x, y], transform);
                Vector2 calPosition = new Vector2(x, y) - Vector2.one;
                spawn.transform.localPosition = calPosition;
            }
        }
    }

    private void Update()
    {
        if(followMouse)
        {
            Vector2 cal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = cal;
        }
    }

    private void OnMouseDown()
    {
        Board.Instance.TablePickup(gameObject);
        followMouse = true;
        col.enabled = false;
    }
}

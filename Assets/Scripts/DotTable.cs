using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTable : MonoBehaviour
{
    private BoxCollider2D col;

    [SerializeField] private dotType[,] content = new dotType[3, 3];

    private bool followMouse = false;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();

        content[0, 1] = dotType.Blue;
        content[1, 1] = dotType.Green;
        content[2, 1] = dotType.Red;
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
        followMouse = true;
        col.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTable : MonoBehaviour
{
    private BoxCollider2D col;

    public Dot[,] content = new Dot[3, 3];

    private bool followMouse = false;

    public int tableSize { get; private set; } = 3;

    [SerializeField] private GameObject dotPrefab;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();

        GameObject spawn = Instantiate(dotPrefab, transform);
        spawn.transform.position += Vector3.left;
        Dot dot = spawn.GetComponent<Dot>();
        dot.Setup(dotType.Red);
        content[0, 1] = dot;

        spawn = Instantiate(dotPrefab, transform);
        dot = spawn.GetComponent<Dot>();
        dot.Setup(dotType.Blue);
        content[1, 1] = dot;

        spawn = Instantiate(dotPrefab, transform);
        spawn.transform.position += Vector3.right;
        dot = spawn.GetComponent<Dot>();
        dot.Setup(dotType.Green);
        content[2, 1] = dot;
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
        Board.Instance.holding = this;
        followMouse = true;
        col.enabled = false;
    }
}

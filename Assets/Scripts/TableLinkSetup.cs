using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLinkSetup : MonoBehaviour
{
    [SerializeField] Gradient notRotateColor;

    public void DrawDirection(Vector2 dir, bool notRotatebul)
    {
        //mine line
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, Vector2.zero);
        lr.SetPosition(1, dir);

        //Set color
        if (notRotatebul)
            lr.colorGradient = notRotateColor;
    }
}

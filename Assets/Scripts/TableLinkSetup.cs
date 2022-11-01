using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLinkSetup : MonoBehaviour
{
    public void DrawDirection(Vector2 dir)
    {
        //mine line
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, Vector2.zero);
        lr.SetPosition(1, dir);

        //Child line
        LineRenderer cLr = transform.GetChild(0).GetComponent<LineRenderer>();
        cLr.SetPosition(0, Vector2.zero);
        cLr.SetPosition(1, dir);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public dotType type;

    [HideInInspector] public DotTable ownerTable;
    [HideInInspector] public Vector2Int gridPos;
}

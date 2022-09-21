using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JetBrains.Annotations;

public class InspectorGrid : SerializedMonoBehaviour
{
    [FoldoutGroup("Pattorn")]
    [SerializeField] protected bool sameLength = true;
    [FoldoutGroup("Pattorn")][ShowIf("sameLength")]
    [SerializeField][Range(1, 8)] protected int gridSize;
    [FoldoutGroup("Pattorn")][HideIf("sameLength")]
    [SerializeField][Range(1, 8)] protected int gridSizeX = 3;
    [FoldoutGroup("Pattorn")][HideIf("sameLength")]
    [SerializeField][Range(1, 8)] protected int gridSizeY = 3;
    protected Vector2Int currentGridSize
    {
        get
        {
            if (sameLength)
                return new Vector2Int(gridSize, gridSize);
            else
                return new Vector2Int(gridSizeX, gridSizeY);
        }
    }

    //[BoxGroup("Pattern")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")] [FoldoutGroup("Pattorn")]
    [SerializeField] protected GameObject[,] pattern;
    public GameObject[,] Pattern { get { return pattern; }}

    [Button("Resize grid")]
    [FoldoutGroup("Pattorn")]
    void MakeGrid()
    {
        pattern = new GameObject[currentGridSize.x, currentGridSize.y];
    }
}

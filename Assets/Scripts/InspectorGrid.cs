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
    [SerializeField][Range(1, 8)] protected int gridSize = 3;
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
    public Vector2Int CurrentGridSize => currentGridSize;

    [FoldoutGroup("Pattorn")][SerializeField] protected GameObject[,] pattern;
    public GameObject[,] Pattern { get { return pattern; }}

    [Button("Resize grid")]
    [FoldoutGroup("Pattorn")]
    public virtual void MakeGrid()
    {
        pattern = new GameObject[currentGridSize.x, currentGridSize.y];
    }

    [FoldoutGroup("Pattorn")]
    [Button("Place dots")]
    public virtual void SpawnDots()
    {
        //Destroy old
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject, true);
        }

        //Make dots
        Vector2 cornorPos = Vector2.zero;
        float gridSize = 1;
        cornorPos.x = (currentGridSize.x % 2 == 0) ? -currentGridSize.x / 2 + gridSize / 2
            : -currentGridSize.x / 2 * gridSize;
        cornorPos.y = (currentGridSize.y % 2 == 0) ? currentGridSize.y / 2 - gridSize / 2
            : currentGridSize.y / 2;
        
        for (int x = 0; x < pattern.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                if (pattern[x, y] != null)
                {
                    GameObject spawn = Instantiate(pattern[x, y], transform);
                    spawn.transform.localPosition = cornorPos + new Vector2(x, -y);
                }
            }
        }
    }
}

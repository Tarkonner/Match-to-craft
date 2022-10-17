using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Level info")]
public class LevelInfo : SerializedScriptableObject
{
    public List<GameObject> pieces;

    public List<GameObject> goals;

    [FoldoutGroup("Pattorn")]
    [SerializeField] protected bool sameLength = true;
    [FoldoutGroup("Pattorn")]
    [ShowIf("sameLength")]
    [SerializeField][Range(1, 8)] protected int gridSize = 3;
    [FoldoutGroup("Pattorn")]
    [HideIf("sameLength")]
    [SerializeField][Range(1, 8)] protected int gridSizeX = 3;
    [FoldoutGroup("Pattorn")]
    [HideIf("sameLength")]
    [SerializeField][Range(1, 8)] protected int gridSizeY = 3;
    public Vector2Int CurrentGridSize
    {
        get
        {
            if (sameLength)
                return new Vector2Int(gridSize, gridSize);
            else
                return new Vector2Int(gridSizeX, gridSizeY);
        }
    }


    [FoldoutGroup("Pattorn")][SerializeField] protected GameObject[,] pattern;
    public GameObject[,] Pattern { get { return pattern; } }

    [Button("Resize grid")]
    [FoldoutGroup("Pattorn")]
    public virtual void MakeGrid()
    {
        pattern = new GameObject[CurrentGridSize.x, CurrentGridSize.y];
    }
}

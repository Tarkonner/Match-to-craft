using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Board : SerializedMonoBehaviour
{
    [SerializeField] GameGrid grid;

    public GameObject piecesHolder;
    public Vector2 piecesOffest = new Vector2(50, 50);

    public GameObject goalHolder;
    public Vector2 goalsOffest = new Vector2(50, 50);

    [SerializeField] private LevelInfo[] levels;

    private int targetLevel = 0;

    //public LevelInfo CurrentLevel { get { return levels[targetLevel]; } }
    [HideInInspector] public List<GameObject> currentLevelsPieces = new List<GameObject>();
    [HideInInspector] public List<GameObject> currentLevelsGoals = new List<GameObject>();
    public Vector2Int CurrentLevelGridSize { get; private set; }

    public void Start()
    {
        LoadLevel(targetLevel);
    }


    public void NextLevel()
    {
        targetLevel++;

        if (targetLevel >= levels.Length)
            SceneLoader.Instance.LoadScene(0);
        else
            LoadLevel(targetLevel);
    }

    public void LoadLevel(int index)
    {
        CurrentLevelGridSize = levels[index].CurrentGridSize;

        //Pieces
        //Remove old
        currentLevelsPieces.Clear();
        if (piecesHolder.transform.childCount > 0)
        {
            for (int i = piecesHolder.transform.childCount - 1; i >= 0; i--)
                Destroy(piecesHolder.transform.GetChild(i));
        }
        //Add new        
        for (int i = 0; i < levels[targetLevel].Pieces.Count; i++)
        {
            GameObject spawn = Instantiate(levels[targetLevel].Pieces[i], piecesHolder.transform);
            spawn.transform.localPosition = new Vector2(0, -i * piecesOffest.y);
            currentLevelsPieces.Add(spawn);
        }

        //Goals
        //Remove old
        currentLevelsGoals.Clear();
        if (goalHolder.transform.childCount > 0)
        {
            for (int i = goalHolder.transform.childCount - 1; i >= 0; i--)
                Destroy(goalHolder.transform.GetChild(i));
        }
        //Add new
        for (int i = 0; i < levels[targetLevel].Goals.Count; i++)
        {
            GameObject spawn = Instantiate(levels[targetLevel].Goals[i], goalHolder.transform);
            spawn.transform.localPosition = new Vector2(0, -i * goalsOffest.y);
            currentLevelsGoals.Add(spawn);
        }

        //Grid
        grid.SetupPattorn(levels[targetLevel]);
    }
}

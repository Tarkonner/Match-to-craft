using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : SerializedMonoBehaviour
{
    [SerializeField] GameGrid grid;

    [Header("Holders")]
    public GameObject piecesHolder;
    public GameObject goalHolder;
    public Vector2 goalsOffest = new Vector2(50, 50);
    [SerializeField] private float xSize = 5;
    [SerializeField] private float ySize = 7;

    [SerializeField] private float piecesHolderOffset = 2;
    [SerializeField] private float piecesSize = 1.5f;
    private Vector2 piecesHolderTop;
    [SerializeField] private GameObject piecesBackground;
    [SerializeField] private float backgroundScale = 1.5f;
    private int maxVerticalPieces = 4;

    private Vector2 goalsHolderTop;

    [Header("Levels")]
    [SerializeField] private LevelInfo[] levels;
    private int targetLevel = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelInfo;

    [HideInInspector] public List<GameObject> currentLevelsPieces = new List<GameObject>();
    [HideInInspector] public List<GameObject> currentLevelsGoals = new List<GameObject>();
    public Vector2Int CurrentLevelGridSize { get; private set; }



    public void Start()
    {
        goalsHolderTop = goalHolder.transform.position + new Vector3(0, ySize / 2, 0);
        piecesHolderTop = piecesHolder.transform.position + new Vector3(0, ySize / 2, 0);

        targetLevel = SceneLoader.Instance.levelIndex;
        LoadLevel(targetLevel);
    }


    public void NextLevel()
    {
        //Reset UI
        Mouse.Instance.acDrop?.Invoke();

        //Find next level
        targetLevel++;
        if (targetLevel >= levels.Length)
            SceneLoader.Instance.LoadScene(0);
        else
            LoadLevel(targetLevel);
    }

    public void LoadLevel(int index)
    {
        CurrentLevelGridSize = levels[index].CurrentGridSize;

        //Update UI
        levelInfo.text = "Level " + (targetLevel + 1);

        //Pieces
        //Remove old
        currentLevelsPieces.Clear();
        if (piecesHolder.transform.childCount > 0)
        {
            for (int i = piecesHolder.transform.childCount - 1; i >= 0; i--)
                Destroy(piecesHolder.transform.GetChild(i).gameObject);
        }
        //Add new
        for (int i = 0; i < levels[targetLevel].Pieces.Count; i++)
        {
            //Calculate position
            Vector2 targetPosition;
            if (levels[targetLevel].Pieces.Count < 5)
                targetPosition = new Vector2(0, piecesHolderTop.y - piecesHolderOffset - i * piecesSize);
            else
            {
                int cal = i % 2;
                if (cal == 0)
                    targetPosition = new Vector2(-piecesSize / 2, piecesHolderTop.y - piecesHolderOffset - Mathf.Round(i / 2) * piecesSize);
                else
                    targetPosition = new Vector2(piecesSize / 2, piecesHolderTop.y - piecesHolderOffset - Mathf.Round(i / 2) * piecesSize);
            }

            //Piece
            GameObject spawn = Instantiate(levels[targetLevel].Pieces[i], piecesHolder.transform);
            spawn.transform.localPosition = targetPosition;
            currentLevelsPieces.Add(spawn);

            //Background
            spawn = Instantiate(piecesBackground, piecesHolder.transform);
            spawn.transform.localScale = new Vector3(backgroundScale, backgroundScale, backgroundScale);
            spawn.transform.localPosition = targetPosition;
        }

        //Goals
        //Remove old
        currentLevelsGoals.Clear();
        if (goalHolder.transform.childCount > 0)
        {
            for (int i = goalHolder.transform.childCount - 1; i >= 0; i--)
                Destroy(goalHolder.transform.GetChild(i).gameObject);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 6);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(piecesHolder.transform.position, new Vector3(xSize, ySize));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(goalHolder.transform.position, new Vector3(xSize, ySize));
    }
}

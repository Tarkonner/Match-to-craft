using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : SerializedMonoBehaviour
{
    [SerializeField] Board board;

    private const float sizeOfGrid = 1;
    private SpriteRenderer sr;
    private Vector2 gridPosition;

    //Memori
    private GameObject[,] gridMemori;


    [SerializeField] private GameObject dotHolder;

    private bool nextLevel = false;
    private float sceneTransistenTime = 1;
    private float sceneClock;

    [Header("Sound")]
    [SerializeField] float minPitch = .9f;
    [SerializeField] float maxPitch = 1;
    private AudioSource audioSource;
    [SerializeField] AudioClip goalComplete;
    [SerializeField] AudioClip winLevel;
    [SerializeField] AudioClip undoGoal;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (nextLevel)
        {
            sceneClock += Time.deltaTime;
            if (sceneClock >= sceneTransistenTime)
                SceneLoader.NextScene();
        }
    }


    public void SetupPattorn(LevelInfo li)
    {
        //Set size
        GetComponent<BoxCollider2D>().size = li.CurrentGridSize;
        //Set sprite
        GetComponent<SpriteRenderer>().size = li.CurrentGridSize;
        //Set memory
        gridMemori = li.Pattern;
    }

    private Vector2 SnapToGrid(Vector2 targetPosition)
    {
        return new Vector2(
            Mathf.Round(targetPosition.x / sizeOfGrid) * sizeOfGrid,
            Mathf.Round(targetPosition.y / sizeOfGrid) * sizeOfGrid);
    }

    public bool PlaceDots(DotTable targetTable)
    {
        //Check if allowed
        foreach (GameObject item in targetTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + board.CurrentLevel.CurrentGridSize.x / 2,
                (int)SnapToGrid(item.transform.position).y + board.CurrentLevel.CurrentGridSize.y / 2);

            //Bounderi
            if (gridPos.x < 0 || gridPos.y < 0 || 
                gridPos.x >= board.CurrentLevel.CurrentGridSize.x || 
                gridPos.y >= board.CurrentLevel.CurrentGridSize.y)
                return false;

            //Is there already something
            if (gridMemori[gridPos.x, gridPos.y] != null)
                return false;
        }

        //Snap table to grid
        targetTable.transform.position = (Vector3)SnapToGrid(targetTable.transform.position);

        //Place dots in memory
        foreach (GameObject item in targetTable.content)
        {
            Vector2Int gridPos = new Vector2Int((int)SnapToGrid(item.transform.position).x + board.CurrentLevel.CurrentGridSize.x / 2,
                (int)SnapToGrid(item.transform.position).y + board.CurrentLevel.CurrentGridSize.y / 2);

            gridMemori[gridPos.x, gridPos.y] = item;
            item.GetComponent<Dot>().gridPos = gridPos;
        }

        bool makedGoal = CheckGoals();

        bool allDone = true;
        foreach (GameObject item in board.CurrentLevel.goals)
        {
            GoalTable goalTable = item.GetComponent<GoalTable>();

            if (!goalTable.completet)
            {
                allDone = false;
                break;
            }
        }
        if (allDone)
        {
            PlayAudio(winLevel);
            nextLevel = true;
        }
        else if (makedGoal)
            PlayAudio(goalComplete);

        return true;
    }

    public DotTable TakeFromBoard(Vector2 clickPosition)
    {
        Vector2 gridPosition = SnapToGrid(clickPosition);
        DotTable table = null;

        //Take table from board
        if (gridMemori[(int)gridPosition.x + board.CurrentLevel.CurrentGridSize.x / 2, 
            (int)gridPosition.y + board.CurrentLevel.CurrentGridSize.y / 2] != null)
        {
            table = gridMemori[(int)gridPosition.x + board.CurrentLevel.CurrentGridSize.x / 2, 
                (int)gridPosition.y + board.CurrentLevel.CurrentGridSize.y / 2].GetComponent<Dot>().ownerTable;

            //See if it was part of complete goal
            table.pickedupAction?.Invoke();

            //Remove dots from memori
            for (int i = 0; i < table.content.Count; i++)
            {
                Vector2Int removePos = table.content[i].GetComponent<Dot>().gridPos;
                gridMemori[removePos.x, removePos.y] = null;
            }
        }

        return table;
    }

    private bool CheckGoals()
    {
        bool result = false;

        for (int x = 0; x < gridMemori.GetLength(0); x++)
        {
            for (int y = 0; y < gridMemori.GetLength(1); y++)
            {
                //Check goals
                foreach (GameObject item in board.CurrentLevel.goals)
                {
                    GoalTable goal = item.GetComponent<GoalTable>();

                    //Bouncs
                    if (goal.pattern.GetLength(0) + x > gridMemori.GetLength(0)
                        || goal.pattern.GetLength(1) + y > gridMemori.GetLength(1))
                        continue;

                    List<DotTable> pieces = new List<DotTable>();

                    bool match = true;
                    for (int i = 0; i < goal.pattern.GetLength(0); i++)
                    {
                        if (!match)
                            continue;

                        for (int j = 0; j < goal.pattern.GetLength(1); j++)
                        {
                            //Is there somehing
                            if (gridMemori[x + i, y + j] == null)
                                continue;
                            Dot checking = gridMemori[x + i, y + j].GetComponent<Dot>();

                            if (goal.pattern[i, j] == null)
                                continue;


                            if (checking == null || checking.type != goal.pattern[i, j].GetComponent<Dot>().type)
                            {
                                match = false;
                                break;
                            }

                            if (checking.ownerTable != null && !pieces.Contains(checking.ownerTable))
                                pieces.Add(checking.ownerTable);
                        }
                    }

                    if (match)
                    {
                        goal.GoalCompletet();

                        goal.undoGoal += UncompleteGoal;

                        result = true;

                        goal.subsubscribers = pieces;
                        foreach (DotTable p in pieces)
                            p.pickedupAction += goal.GoalUncomplet;
                    }
                }
            }
        }

        return result;
    }

    private void UncompleteGoal()
    {
        PlayAudio(undoGoal);
    }

    public void PlayAudio(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    public void PlayAudio(AudioClip[] clips)
    {
        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }
}

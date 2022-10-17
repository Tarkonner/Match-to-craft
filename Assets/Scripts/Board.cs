using Sirenix.OdinInspector;
using UnityEngine;

public class Board : SerializedMonoBehaviour
{
    [SerializeField] GameGrid grid;

    public GameObject piecesHolder;
    public Vector2 piecesOffest = new Vector2(50, 50);

    public GameObject goalHolder;
    public Vector2 goalsOffest = new Vector2(50, 50);

    public LevelInfo[] levels;

    private int targetLevel = 0;

    public LevelInfo CurrentLevel { get { return levels[targetLevel]; } }

    public void Start()
    {
        LoadLevel(targetLevel);
    }

    public void LoadLevel(int index)
    {
        //Pieces
        //Remove old
        if (piecesHolder.transform.childCount > 0)
        {
            for (int i = piecesHolder.transform.childCount - 1; i >= 0; i--)
                Destroy(piecesHolder.transform.GetChild(i));
        }
        //Add new
        for (int i = 0; i < levels[targetLevel].pieces.Count; i++)
        {
            GameObject spawn = Instantiate(levels[targetLevel].pieces[i], piecesHolder.transform);
            spawn.transform.localPosition = new Vector2(0, -i * piecesOffest.y);
        }

        //Goals
        //Remove old
        if (goalHolder.transform.childCount > 0)
        {
            for (int i = goalHolder.transform.childCount - 1; i >= 0; i--)
                Destroy(goalHolder.transform.GetChild(i));
        }
        //Add new
        for (int i = 0; i < levels[targetLevel].goals.Count; i++)
        {
            GameObject spawn = Instantiate(levels[targetLevel].goals[i], goalHolder.transform);
            spawn.transform.localPosition = new Vector2(0, -i * goalsOffest.y);
        }

        //Grid
        grid.SetupPattorn(levels[targetLevel]);
    }
}

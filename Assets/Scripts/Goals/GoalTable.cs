using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public abstract class GoalTable : SerializedMonoBehaviour
{
    public int size;

    [BoxGroup("Pattern")]
    [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
    public GameObject[,] pattern;

    [HideInInspector] public List<DotTable> subsubscribers;

    public Color winColor = new Color(255, 129, 66, 255);
    public Color norColor = new Color(255, 247, 248, 255);

    private SpriteRenderer sr;

    public bool completet { get; private set; } = false;

    [Header("Sound")]
    [SerializeField] float minPitch = .9f;
    [SerializeField] float maxPitch = 1;
    private AudioSource audioSource;

    public Action undoGoal;

    #region Make piece
    [Button("Resize grid")]
    void MakeGrid()
    {
        pattern = new GameObject[size, size];
    }

    [Button("Place dots")]
    void SpawnDots()
    {
        //Destroy old
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        //Make dots
        for (int x = 0; x < pattern.GetLength(0); x++)
        {
            for (int y = 0; y < pattern.GetLength(1); y++)
            {
                if (pattern[x, y] != null)
                {
                    GameObject spawn = Instantiate(pattern[x, y], transform);

                    Vector2 calPos;
                    if (size % 2 == 0)
                        calPos = new Vector2(x - 0.5f, y - 0.5f);
                    else
                        calPos = new Vector2(x - 1, y - 1);

                    spawn.transform.localPosition = calPos;
                }
            }
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.size = new Vector2(size, size);
    }
    #endregion


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void GoalCompletet()
    {
        sr.color = winColor;

        completet = true;
    }

    public void GoalUncomplet()
    {
        sr.color = norColor;

        undoGoal?.Invoke();

        foreach (DotTable item in subsubscribers)
        {
            item.pickedupAction -= GoalCompletet;
        }

        completet = false;
    }
}

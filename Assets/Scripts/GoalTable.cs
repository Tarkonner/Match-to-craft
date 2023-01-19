using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public  class GoalTable : InspectorGrid, GameGoal
{
    [SerializeField] private GameObject backgroundField;
    [HideInInspector] public List<DotTable> subsubscribers;

    public Color winColor = new Color(255, 129, 66, 255);
    public Color norColor = new Color(255, 247, 248, 255);

    private bool completet = false;

    private List<SpriteRenderer> fieldsRendere = new List<SpriteRenderer>();


    private void Start()
    {
        //Background
        for (int x = 0; x < CurrentGridSize.x; x++)
        {
            for (int y = 0; y < CurrentGridSize.y; y++)
            {
                Vector2 offset = CurrentGridSize / 2;
                if (CurrentGridSize.x % 2 == 0)
                    offset.x -= .5f;
                if (CurrentGridSize.y % 2 == 0)
                    offset.y -= .5f;

                GameObject spawn = Instantiate(backgroundField, gameObject.transform);
                spawn.transform.localPosition = offset - new Vector2(x, y);
                fieldsRendere.Add(spawn.GetComponent<SpriteRenderer>());
            }
        }

        TweeningAnimations.Instance.EasingAnimation(transform, true);
    }


    public void GoalComplete()
    {
        foreach (SpriteRenderer item in fieldsRendere)
            item.color = winColor;

        completet = true;
    }

    public void GoalUncomleted()
    {
        foreach (SpriteRenderer item in fieldsRendere)
            item.color = norColor;

        foreach (DotTable item in subsubscribers)
        {
            item.pieceInGoal = null;
        }
        subsubscribers.Clear();

        BoardsSounds.Instance.UndoGoal();

        completet = false;
    }

    public bool GoalState()
    {
        return completet;
    }
}

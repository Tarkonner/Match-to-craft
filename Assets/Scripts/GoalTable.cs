using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public  class GoalTable : InspectorGrid, GameGoal
{
    [HideInInspector] public List<DotTable> subsubscribers;

    public Color winColor = new Color(255, 129, 66, 255);
    public Color norColor = new Color(255, 247, 248, 255);

    private SpriteRenderer sr;

    private bool completet = false;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    public void GoalComplete()
    {
        sr.color = winColor;
        completet = true;
    }

    public void GoalUncomleted()
    {
        sr.color = norColor;

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


    [FoldoutGroup("Pattorn")]
    [Button("Place dots")]
    public override void SpawnDots()
    {
        base.SpawnDots();

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.size = new Vector2(currentGridSize.x, currentGridSize.y);
    }


}

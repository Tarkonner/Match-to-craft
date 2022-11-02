using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PlacementGoal : MonoBehaviour, GameGoal
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI scoreText;

    public Color winColor = new Color(255, 129, 66, 255);
    public Color norColor = new Color(255, 247, 248, 255);

    private bool completet = false;

    public List<GridPlaceGoal> boardGoals = new List<GridPlaceGoal>();


    public void UpdateScore(int score)
    {
        scoreText.text = score + "/" + boardGoals.Count;
    }

    private void Start()
    {
        scoreText.text = 0 + "/" + boardGoals.Count;
    }

    public void GoalComplete()
    {
        background.color = winColor;
        completet = true;
    }

    public void GoalUncomleted()
    {
        background.color = norColor;
        completet = false;
    }

    public bool GoalState()
    {
        return completet;
    }
}

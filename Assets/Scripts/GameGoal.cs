using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameGoal : MonoBehaviour
{    public bool completet { get; private set; } = false;

    public abstract bool GoalComplete();
    public abstract bool GoalUncomleted();
}

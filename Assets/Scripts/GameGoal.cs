using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GameGoal
{
    public abstract void GoalComplete();
    public abstract void GoalUncomleted();

    public abstract bool GoalState();
}

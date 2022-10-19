using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardsSounds : SoundPiece
{
    public static BoardsSounds Instance;

    [SerializeField] AudioClip goalComplete;
    [SerializeField] AudioClip completLevel;
    [SerializeField] AudioClip[] undoGoal;

    private void Awake()
    {
        Instance = this;
    }

    public void GoalComplete() => PlayAudioWithRandomPitch(goalComplete);
    public void CompletLevel() => PlayAudioWithRandomPitch(completLevel);
    public void UndoGoal() => PlayAudioWithRandomPitch(undoGoal);
}

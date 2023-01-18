using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweeningAnimations : MonoBehaviour
{
    public static TweeningAnimations Instance;



    [Header("Transition")]
    [SerializeField] private float waitBeforeTransition = .5f;
    public float WaitBeforeTransition => waitBeforeTransition;
    [SerializeField] private float transistenTime = .2f;
    public float TransistenTime => transistenTime;
    [SerializeField] private float easingIn = 0.2f;
    [SerializeField] private float randomTime = .05f;

    private List<List<Transform>> gotIn = new List<List<Transform>>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void EaseOut()
    {
        foreach (List<Transform> item in gotIn)
            EasingAnimation(item, false);

        gotIn.Clear();
    }

    public void EasingAnimation(Transform target, bool easIn)
    {
        //Ease out leter
        if(easIn)
            gotIn.Add(new List<Transform> { target});

        float targetTime = easIn ? easingIn + Random.Range(0, randomTime) : transistenTime - Random.Range(0, randomTime) - .01f;
        float targetScale = easIn ? target.localScale.x : 0;
        if (easIn)
            target.localScale = Vector3.zero;

        target.DOScale(new Vector3(targetScale, targetScale, targetScale), targetTime);
    }
    public void EasingAnimation(List<Transform> target, bool easIn)
    {
        //Ease out leter
        if(easIn)
            gotIn.Add(target);

        float targetTime = easIn ? easingIn + Random.Range(0, randomTime) : transistenTime - Random.Range(0, randomTime) - .01f;
        List<float> targetScale = new List<float>();

        foreach (Transform item in target)
        {
            float scaleTarget;
            if (easIn)
                scaleTarget = item.localScale.x;
            else
                scaleTarget = 0;
            targetScale.Add(scaleTarget);

            if (easIn)
                item.localScale = Vector3.zero;

            item.DOScale(new Vector3(targetScale[targetScale.Count - 1], targetScale[targetScale.Count - 1], targetScale[targetScale.Count - 1]), targetTime);
        }

    }

}

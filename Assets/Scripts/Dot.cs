using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private SpriteRenderer sr;

    public dotType type;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Setup(dotType dotsColor)
    {
        type = dotsColor;

        switch (dotsColor)
        {
            case dotType.Red:
                sr.color = Color.red;
                break;
            case dotType.Blue:
                sr.color = Color.blue;
                break;
            case dotType.Green:
                sr.color = Color.green;
                break;
            default:
                break;
        }
    }
}

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
}

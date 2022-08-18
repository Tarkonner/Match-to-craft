using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public dotType type;

    public Vector2 tablePosition;

    public void GoToPosition(Vector3 intake)
    {
        //transform.localPosition = Vector2.zero;

        //foreach (var item in intake)
        //{
        //    if (item == this)
        //        transform.position = Vector2.zero;
        //}
        transform.localPosition = intake;
    }
}

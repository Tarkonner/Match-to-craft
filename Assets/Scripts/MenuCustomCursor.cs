using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCustomCursor : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;

    void Start()
    {
        UnityEngine.Cursor.SetCursor(cursorTexture, new Vector2(31, 21), CursorMode.Auto);
    }
}

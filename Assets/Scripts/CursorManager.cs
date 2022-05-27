using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D cursorDefault;
    [SerializeField] Texture2D cursorPressed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.SetCursor(cursorPressed, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
        }
    }
}

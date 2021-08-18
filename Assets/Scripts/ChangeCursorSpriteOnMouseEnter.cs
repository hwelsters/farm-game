using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursorSpriteOnMouseEnter : MonoBehaviour
{
    [SerializeField] Texture2D cursorSprite;
    public void ChangeSprite()
    {
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    public void RevertSprite()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}

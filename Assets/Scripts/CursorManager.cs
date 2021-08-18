using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D giftCursor;
    [SerializeField] private Texture2D plusCursor;
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private Texture2D fingerPointCursor;
    [SerializeField] private Texture2D magnifyingGlassCursor;
    [SerializeField] private Texture2D dialogCursor;

    public static CursorManager instance = null;

    void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public static void SetCursorBackToNormal()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToGift()
    {
         Cursor.SetCursor(instance.giftCursor, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToHand()
    {
        Cursor.SetCursor(instance.handCursor, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToFingerPoint()
    {
        Cursor.SetCursor(instance.fingerPointCursor, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToMagnifyingGlass()
    {
        Cursor.SetCursor(instance.magnifyingGlassCursor, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToPlus()
    {
        Cursor.SetCursor(instance.plusCursor, Vector2.zero, CursorMode.Auto);
    }

    public static void SetCursorToDialog()
    {
        Cursor.SetCursor(instance.dialogCursor, Vector2.zero, CursorMode.Auto);
    }
}

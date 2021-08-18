using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTranslucent : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Coroutine coroutine;

    float alphaValue = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bobber"))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            coroutine = StartCoroutine(ChangeAlpha(false));
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bobber"))
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            StartCoroutine(ChangeAlpha(true));
        }
    }

    IEnumerator ChangeAlpha (bool visible)
    {
        if (visible)
        {
            while (alphaValue < 1)
            {
                alphaValue += 0.02f;
                spriteRenderer.color = new Color(1f, 1f, 1f, alphaValue);
                yield return null;
            }
        }
        else
        {
            while (alphaValue > 0.5)
            {
                alphaValue -= 0.02f;
                spriteRenderer.color = new Color(1f, 1f, 1f, alphaValue);
                yield return null;
            }
        }
    }
}

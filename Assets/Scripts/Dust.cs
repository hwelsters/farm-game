using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour
{
    private float alphaValue = 1f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        alphaValue -= 0.005f;
        alphaValue = Mathf.Clamp(alphaValue, 0f, 1f);

        if (alphaValue < 0.2f) Destroy(gameObject);

        spriteRenderer.color = new Color(1f, 1f, 1f, alphaValue);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightLight : MonoBehaviour
{
    [SerializeField] private Sprite dayVersion;
    [SerializeField] private Sprite nightVersion;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SwitchToDay()
    {
        spriteRenderer.sprite = dayVersion;
    }

    public void SwitchToNight()
    {
        spriteRenderer.sprite = nightVersion;
    }
}

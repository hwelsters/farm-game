using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{
    [SerializeField] private GameObject raindrop;
    public Coroutine rainCoroutine;
    public static Weather instance = null;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StopRaining()
    {
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
            rainCoroutine = null;
        }
    }

    public void StartRaining()
    {
        rainCoroutine = StartCoroutine(Rain());
    }

    private IEnumerator Rain()
    {
        while (true)
        {
            CreateRainDrop();
            yield return new WaitForSeconds(0.0005f);
        }
    }

    void CreateRainDrop()
    {
        float spawnY = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float spawnX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        Instantiate(raindrop, spawnPosition, Quaternion.identity);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class CropPlants : MonoBehaviour
{
    private ItemTable itemTable;
    [SerializeField] private int growthTime;
    [SerializeField] private Sprite[] growthStagesSprites;
    [SerializeField] private bool multipleHarvests;
    [SerializeField] private int minYield;
    [SerializeField] private int maxYield;
    [SerializeField] private int repeatHarvestTime;
    [SerializeField] private int cropDroppedIndex;

    public int plantIndex;

    private Tilemap wateredTilemap;

    private SpriteRenderer spriteRenderer;

    private int stages;

    private float stageGap;

    private int currentStage = 0;

    [HideInInspector] public int daysPassed = 0;

    private Vector3Int plantPosition;

    private Coroutine shakeCoroutine = null;

    void Start()
    {
        itemTable = GameObject.FindGameObjectWithTag("ItemTable").GetComponent<ItemTable>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        stages = growthStagesSprites.Length;
        stageGap = growthTime / (stages);
        wateredTilemap = GameObject.FindGameObjectWithTag("WateredTilemap").GetComponent<Tilemap>();
        plantPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y - 1, (int)transform.position.z);
    }

    public void Grow ()
    {
        if (wateredTilemap.GetTile(plantPosition) != null)
        {
            daysPassed++;

            daysPassed = (int)Mathf.Clamp(daysPassed, 0, growthTime);
            currentStage = (int)(daysPassed / stageGap);


            currentStage = Mathf.Clamp(currentStage, 0, stages - 2);

            UpdatePlantSprite();
        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentStage != 0)
        {   
            if (shakeCoroutine == null) shakeCoroutine = StartCoroutine(Shake(0.5f, 5f));
        }
    }

    private IEnumerator Shake (float duration, float magnitude)
    {
        transform.eulerAngles = new Vector3(0,0,0);
        Vector3 originalRotation = transform.eulerAngles;

        float shakeVelocity = 70f / (float)Math.PI;
        float elapsed = 0;
        float zRotation = 0f;
        
        float zSinVal = (float) Math.Sin(zRotation);

        while (elapsed < duration || zSinVal - 0f > float.Epsilon)
        {
            zSinVal = (float) Math.Sin(zRotation);
            zRotation += shakeVelocity * Time.deltaTime;

            float currentZRotation = zSinVal * magnitude;
            transform.eulerAngles = new Vector3(0,0,currentZRotation);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.eulerAngles = originalRotation;

        shakeCoroutine = null;
    }

    private void UpdatePlantSprite()
    {
        spriteRenderer.sprite = growthStagesSprites[currentStage];

        if (daysPassed >= growthTime)
            spriteRenderer.sprite = growthStagesSprites[stages - 1];
    }

    public void GetHarvested()
    {
        if (daysPassed >= growthTime)
        {
            int yield = Random.Range(minYield, maxYield);

            
            itemTable.CreateItem(transform.position, cropDroppedIndex, yield);

            if (multipleHarvests)
            {
                daysPassed = growthTime - repeatHarvestTime - 1;
                UpdatePlantSprite();
            }

            else
                Destroy(gameObject);
        }
    }

    public bool CanBeHarvested()
    {
        return daysPassed >= growthTime;
    }
}

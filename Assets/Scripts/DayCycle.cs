using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DayCycle : MonoBehaviour
{
    [SerializeField] private float dayLength;
    [SerializeField] private float noonTime;

    [SerializeField] private GameObject[] debris;

    private CanvasGroup canvasGroup;
    private float objectAlpha;

    [SerializeField] private float debrisStartingX;
    [SerializeField] private float debrisStartingY;
    [SerializeField] private int debrisColumns;
    [SerializeField] private int debrisRows;

    private List<Vector2> debrisGridPositions = new List<Vector2>();

    private bool newGame = true;

    [SerializeField] private Transform farmManager;

    [SerializeField] private Transform arrowTransform;

    private TileAim tileAim;

    [SerializeField] private string[] seasonNames;

    [SerializeField] private string[] dayNames;

    [SerializeField] private Text timeDisplaytext;
    private float time = 0;

    private int minutes;
    public int hours = 0;
    private int hourToDisplay;

    public static int day = -1;
    public static int weather = 0;

    private int[] weatherForEntireYear;

    [SerializeField] private Tilemap wateredTileMap;
    [SerializeField] private RuleTile wateredTile;

    [SerializeField] private int chanceOfRain;

    [SerializeField] private int daysInAMonth;

    [SerializeField] private int daysInALunarMonth;

    private bool newYear;

    public int season = 0;

    private Animator fadeAnimator;

    private Coroutine cycleCoroutine = null;
    
    void Start()
    {
        weatherForEntireYear = new int[daysInAMonth * 4];
        InitializeWeatherForEntireYear();

        tileAim = GameObject.FindGameObjectWithTag("TileAim").GetComponent<TileAim>();
        canvasGroup = GetComponent<CanvasGroup>();
        objectAlpha = canvasGroup.alpha;

        fadeAnimator = GameObject.FindGameObjectWithTag("Fade").GetComponent<Animator>();

        InitializeList();

        NewDay();

        if (newGame)
        {
            NewGame();
        }
    }

    IEnumerator Cycle()
    {
        time = 0;
        while (time <= dayLength)
        {
            time += Time.deltaTime;
            objectAlpha = (time - noonTime) / (dayLength - noonTime);
            objectAlpha = Mathf.Clamp(objectAlpha, 0f, 1f);

            canvasGroup.alpha = objectAlpha;

            float rotation = time / dayLength * 360;
            arrowTransform.rotation = Quaternion.Euler(0, 0, -rotation);

            if (time == dayLength / 2)
            {
                SwitchToNight();
            }

            ConvertToStandardTime();

            yield return null;
        }
    }   

    private void ConvertToStandardTime()
    {
        hours = (int)(time / dayLength * 18) + 6;
        minutes = (int)((time / dayLength * 18 * 3) % 3);
        minutes *= 20;

        string timePlacement = hours / 12 % 2 == 0 ? "AM" : "PM";

        hourToDisplay = hours;
        hourToDisplay %= 12;
        hourToDisplay = hours == 0 ? 12 : hours;

        string hourString = hourToDisplay.ToString();
        string minuteString = minutes.ToString();

        string timeString = (hourString.PadLeft(2, '0') + ":" + minuteString.PadLeft(2, '0') + timePlacement).ToString();
        timeDisplaytext.text = timeString;
    }

    public void NewDay()
    {
        fadeAnimator.SetTrigger("Leaving");
        tileAim.talkedToNPCs = new bool[60];
        tileAim.giftedNPCToday = new bool[60];
        
        time = 0;

        day++;
        weather = weatherForEntireYear[day];

        if (cycleCoroutine != null) StopCoroutine(cycleCoroutine);
        cycleCoroutine = StartCoroutine(Cycle());

        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plants");

        foreach (GameObject p in plants)
        {
            CropPlants cropPlants = p.GetComponent<CropPlants>();

            if (cropPlants != null)
                cropPlants.Grow();
        }

        wateredTileMap.ClearAllTiles();

        tileAim.CalculateEarnings();

        if (time > dayLength)
            tileAim.wasExhausted = true;

        tileAim.RenewEnergy();

        GameObject[] sprinklers = GameObject.FindGameObjectsWithTag("Sprinklers");

        if (sprinklers.Length != 0)
        {
            foreach (GameObject s in sprinklers)
            {
                Sprinkler sprinkler = s.GetComponent<Sprinkler>();

                if (sprinkler != null)
                    sprinkler.Sprinkle();
            }
        }

        int randomCount = Random.Range(0, 10);
        CreateNewDebris(randomCount);

        RemoveSoil();

        Debug.Log("5");

        Weather.instance.StopRaining();
        if (weather == 2)
        {
            Rain();
        }
        SwitchToDay();
    }

    void Rain()
    {
        Tilemap plowedSoil = tileAim.plowedSoil;

        plowedSoil.CompressBounds();

        foreach (var position in plowedSoil.cellBounds.allPositionsWithin)
        {
            if (plowedSoil.GetTile(position) != null)
            {
                tileAim.WaterTile(position);
            }
        }
    }

    void NewGame()
    {
        CreateNewDebris(1000);

        //The first six days would be the same for every playthrough
        weatherForEntireYear[0] = 1;
        weatherForEntireYear[1] = 1;
        weatherForEntireYear[2] = 1;
        weatherForEntireYear[3] = 1;
        weatherForEntireYear[4] = 1;
        weatherForEntireYear[5] = 2;

        newGame = false;
    }

    void InitializeList()
    {
        debrisGridPositions.Clear();

        for (int x = 0; x < debrisColumns; x++)
            for (int y = 0; y < debrisRows; y++)
                debrisGridPositions.Add(new Vector3(debrisStartingX + x, debrisStartingY + y, 0f));
    }

    void CreateNewDebris (int count)
    {
        for (int i = 0; i < count; i++)
        {
            InstantiateDebris();
        }
    }

    public void InstantiateDebris()
    {
        int randomPositionIndex = (int)Random.Range(0, debrisGridPositions.Count);
        Vector3 randomPosition = debrisGridPositions[randomPositionIndex];

        Collider2D[] collider = Physics2D.OverlapCircleAll(randomPosition, 0.1f);

        if (collider.Length <= 0)
        {
            int randomDebrisIndex = Random.Range(0, debris.Length);
            GameObject objectToInstantiate = debris[randomDebrisIndex];
            GameObject instantiatedObject = Instantiate(objectToInstantiate, randomPosition, Quaternion.identity);
            instantiatedObject.transform.SetParent(farmManager);
        }
    }

    public void NewMonth()
    {

    }

    public void NewYear()
    {
        InitializeWeatherForEntireYear();
    }

    private void InitializeWeatherForEntireYear()
    {
        
        for (int i = 0; i < weatherForEntireYear.Length; i++)
        {
            int futureWeather = RandomChance(chanceOfRain) ? 2 : 1;

            weatherForEntireYear[i] = futureWeather;
        }
    }

    private bool RandomChance(float chance)
    {
        float randomNumber = Random.Range(0, 100);

        if (randomNumber <= chance)
            return true;

        return false;
    }

    private void SwitchToNight()
    {
        GameObject[] nightLights = GameObject.FindGameObjectsWithTag("NightLight");

        foreach (GameObject n in nightLights)
        {
            n.GetComponent<NightLight>().SwitchToNight();
        }
    }

    private void SwitchToDay()
    {
        GameObject[] nightLights = GameObject.FindGameObjectsWithTag("NightLight");

        foreach (GameObject n in nightLights)
        {
            n.GetComponent<NightLight>().SwitchToDay();
        }
    }

    private void RemoveSoil()
    {
        int amountToRemove = Random.Range(0, tileAim.plowedSoilPositions.Count / 10);

        if (RandomChance(10))
        {
            amountToRemove++;
        }
        for (int i = 0; i < amountToRemove; i++)
        {
            if (tileAim.plowedSoilPositions.Count > 0)
            {
                int randomNumber = Random.Range(0, tileAim.plowedSoilPositions.Count);
                Vector3Int randomPlowedPosition = tileAim.plowedSoilPositions[randomNumber];

                Collider2D[] collider = Physics2D.OverlapCircleAll((Vector2Int)randomPlowedPosition, 0.9f);

                foreach (Collider2D c in collider)
                {
                    if (c.GetComponent<CropPlants>() != null)
                    {
                        return;
                    }
                }

                tileAim.plowedSoil.SetTile(randomPlowedPosition, null);
                tileAim.plowedSoilPositions.RemoveAt(randomNumber);
            }
        }
      
    }
}

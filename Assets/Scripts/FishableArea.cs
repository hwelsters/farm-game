using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishableArea : MonoBehaviour
{
    public enum Weather
    {
        NONE, SUNNY, RAINY
    };

    public enum Season
    {
        NONE, SPRING, SUMMER, AUTUMN, WINTER
    };

    [Serializable]
    public class Fish
    {
        public int fishIndex;
        public int chances;
        public int fishAvailableHourStart;
        public int fishAvailableHourEnd;
        public Weather fishAvailableWeather;
        public Season fishAvailableSeason;
    }

    private DayCycle dayCycle;
    [SerializeField] private Fish[] fish;

    void Start()
    {
        dayCycle = GameObject.FindGameObjectWithTag("DayCycle").GetComponent<DayCycle>();
    }

    public int GetFish()
    {
        int totalChances = 0;

        foreach (Fish f in fish)
            if (dayCycle.hours >= f.fishAvailableHourStart && dayCycle.hours <= f.fishAvailableHourEnd)
                if ((int)f.fishAvailableWeather == DayCycle.weather || (int)f.fishAvailableWeather == 0)
                    totalChances += f.chances;

                    
        int randomIndex = Random.Range(0, totalChances - 1);

        foreach (Fish f in fish)
            if (dayCycle.hours >= f.fishAvailableHourStart && dayCycle.hours <= f.fishAvailableHourEnd)
                if ((int)f.fishAvailableWeather == DayCycle.weather || (int)f.fishAvailableWeather == 0)
                {
                    randomIndex -= f.chances;
                    if (randomIndex <= 0)
                        return f.fishIndex;
                }


        return 0;
    }
}

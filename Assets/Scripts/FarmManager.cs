using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmManager : MonoBehaviour
{
    //In this script, I will remember things about the farm
    //This will be activated when the player leaves the farm
    //Will save debris, crop positions, soil positions, 

    public class Object
    {
        public Vector3Int objectPosition;
        public int objectIndex;
        public int daysPassed;
        public Sprite currentSprite;

        public Object(Vector3Int objectPosition, int objectIndex, int daysPassed = 0, Sprite currentSprite = null)
        {
            this.objectPosition = objectPosition;
            this.objectIndex = objectIndex;
            this.daysPassed = daysPassed;
            this.currentSprite = currentSprite;
        }

        public Object(Vector3Int objectPosition, int objectIndex)
        {
            this.objectPosition = objectPosition;
            this.objectIndex = objectIndex;
        }

    }


    public List<Object> cropList = new List<Object>();
    public List<Object> debrisList = new List<Object>();
    public List<Vector3Int> wateredPositions = new List<Vector3Int>();
    public List<Vector3Int> hoedSoilPositions = new List<Vector3Int>();

    private GameObject[] plants;
    [SerializeField] private GameObject[] debris;

    public static FarmManager instance = null;

    public RuleTile hoedSoil;
    public RuleTile wateredSoil;
    public Tilemap hoedSoilTilemap;
    public Tilemap wateredTilemap;

    public void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        TileAim tileAim = GameObject.FindGameObjectWithTag("TileAim").GetComponent<TileAim>();
        plants = tileAim.plants;
    }


    public void SaveFarm()
    {
        cropList.Clear();
        debrisList.Clear();
        GameObject[] taggedPlants = GameObject.FindGameObjectsWithTag("Plants");
        GameObject[] taggedDebris = GameObject.FindGameObjectsWithTag("Debris");

        foreach(GameObject p in taggedPlants)
        {
            Vector3Int positionToSave = new Vector3Int((int)p.transform.position.x, (int)p.transform.position.y, (int)p.transform.position.z);
            CropPlants cropPlants = p.GetComponent<CropPlants>();
            Sprite cropSprite = p.GetComponent<SpriteRenderer>().sprite;
            Object plant = new Object(positionToSave, cropPlants.plantIndex, (int) cropPlants.daysPassed, cropSprite);
            cropList.Add(plant);
        }

        foreach (GameObject d in taggedDebris)
        {
            Vector3Int positionToSave = new Vector3Int((int)d.transform.position.x, (int)d.transform.position.y, (int)d.transform.position.z);
            Hittable hittable = d.GetComponent<Hittable>();
            Object debris = new Object(positionToSave, hittable.debrisIndex);
        }
    }

    public void LoadFarm()
    {
        foreach (Vector3Int w in wateredPositions)
            wateredTilemap.SetTile(w, wateredSoil);

        foreach (Vector3Int s in hoedSoilPositions)
            hoedSoilTilemap.SetTile(s, hoedSoil);

        foreach (Object c in cropList)
        {
            Vector3Int position = c.objectPosition;
            GameObject plantToInstantiate = plants[c.objectIndex];
            GameObject instantiatedPlant = Instantiate(plantToInstantiate, position, Quaternion.identity);
            CropPlants cropPlants = instantiatedPlant.GetComponent<CropPlants>();
            cropPlants.daysPassed = c.daysPassed;
            SpriteRenderer spriteRenderer = instantiatedPlant.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = c.currentSprite;
        }

        foreach (Object d in debrisList)
        {
            Vector3Int position = d.objectPosition;
            GameObject debrisToInstantiate = debris[d.objectIndex];
            Instantiate(debrisToInstantiate, position, Quaternion.identity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Sprinkler : MonoBehaviour
{
    [SerializeField] Tilemap wateredTilemap;
    [SerializeField] Tilemap plowedTilemap;
    [SerializeField] RuleTile wateredSoil;
    [SerializeField] Vector3Int[] tilesToWater;

    void Start()
    {
        wateredTilemap = GameObject.FindGameObjectWithTag("WateredTilemap").GetComponent<Tilemap>();
        plowedTilemap = GameObject.FindGameObjectWithTag("PlowedTilemap").GetComponent<Tilemap>();
    }

    public void Sprinkle()
    {
        foreach (Vector3Int v in tilesToWater)
        {
            Vector3Int waterPosition = new Vector3Int((int) transform.position.x + v.x, (int)transform.position.y + v.y - 1, (int)transform.position.z);
            if (plowedTilemap.GetTile(waterPosition) != null)
                wateredTilemap.SetTile(waterPosition, wateredSoil);
        }
    }
}

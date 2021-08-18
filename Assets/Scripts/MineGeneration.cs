using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MineGeneration : MonoBehaviour
{
    enum MineType
    {
        FLOWER_MINES,
        STONE_MINES,
        TECH_MINES,
        LAVA_MINES,
        CORE_MINES,
        MINE_TYPE_TOTAL
    };

    private Vector3Int[] randomDirections =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0)
    };

    [Serializable]
    private class ObjectArray
    {
        public GameObject[] objectArray;
    }

    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap sideTilemap;
    [SerializeField] private Tilemap variationTilemap;
    [SerializeField] private Tilemap pathTilemap;

    [SerializeField] private RuleTile[] baseTile;
    [SerializeField] private RuleTile[] sideTile;
    [SerializeField] private RuleTile[] variationTile;
    [SerializeField] private RuleTile[] pathTile;

    [SerializeField] private ObjectArray[] objects;
    [SerializeField] private GameObject colliderObject;

    [SerializeField] private GameObject ladderGameObject;
    
    private const byte maxWidth = 15;
    private const byte maxHeight = 15;

    private const ushort minLineLength = 100;
    private const ushort maxLineLength = 500;

    private int currSideLength = 0;
    private int currentBasePositionIndex = 0;

    public static int level = 0;

    List<Vector3Int> basePositions = new List<Vector3Int>();

    private Transform boardParent;
    private int debrisCount = 0;

    public static bool inMines = false;

    void Start()
    {
        boardParent = new GameObject("Board").transform;
    }

    public void GenerateMines()
    {
        debrisCount = 0;
        int mineType = level / 20;

        //BASE
        int lineLength = Random.Range(minLineLength, maxLineLength);
        LoadTiles(baseTilemap, baseTile[mineType], new Vector3Int((int) transform.position.x, (int) transform.position.y, 0), RandomWalk, 1, true, lineLength);

        //VARIATION
        int variationClumps = Random.Range(5, 10);
        lineLength /= 10;
        if (variationTile != null && variationTilemap != null)
        {
            for (int i = 0; i < variationClumps; i++)
            {
                currentBasePositionIndex = Random.Range(0, basePositions.Count);
                LoadTiles(variationTilemap, variationTile[mineType], new Vector3Int(0, 0, 0), BasePositionWalk, 1, false, lineLength);
            }
        }
        //PATH
        variationClumps = Random.Range(5, 10);
        lineLength /= 5;
        if (pathTile != null && pathTilemap != null)
        {
            for (int i = 0; i < variationClumps; i++)
            {
                currentBasePositionIndex = Random.Range(0, basePositions.Count);
                LoadTiles(pathTilemap, pathTile[mineType], new Vector3Int(0,0,0), BasePositionWalk, 1, false, lineLength);
            }
        }

        CreateObjects(objects[mineType].objectArray);

        SurroundMineWithColliders();
    }

    private void LoadTiles(Tilemap currTilemap, RuleTile currTile, Vector3Int currPosition, Func<Vector3Int, Vector3Int> positionChangeMethod, int sideLength, bool savePosition, int times)
    {
        //QUESTIONABLE CODE
        currSideLength = sideLength;

        for (int currLength = 0; currLength < times; currLength++)
        {
            FillSquareAreaTiles(currTilemap, currTile, currPosition, sideLength, savePosition);
            currPosition = positionChangeMethod(currPosition) ;
        }
    }

    private void FillSquareAreaTiles(in Tilemap currTilemap, in RuleTile currTile, in Vector3Int currPosition, in int sideLength, in bool savePosition)
    {
        for (int x = 0; x < sideLength; x++)
        {
            for (int y = 0; y < sideLength; y++)
            {
                Vector3Int direction = new Vector3Int(x, -y, 0);
                Vector3Int endPosition = currPosition + direction;

                currTilemap.SetTile(endPosition, currTile);
                if (currTilemap.GetTile(endPosition + new Vector3Int(0, -1, 0)) == null) sideTilemap.SetTile(endPosition + new Vector3Int(0, -1, 0), sideTile[MineManager.level / 20]);

                if (savePosition) basePositions.Add(endPosition);
            }
        }
    }

    private Vector3Int RandomWalk(Vector3Int currPosition)
    {
        int randomIndex = Random.Range(0, randomDirections.Length);
        Vector3Int direction = randomDirections[randomIndex] * currSideLength;
        Vector3Int endPosition = currPosition + direction;

        if (endPosition.x > transform.position.x + maxWidth / 2 || endPosition.x < transform.position.x -maxWidth / 2 || endPosition.y > transform.position.y || endPosition.y < transform.position.y -maxHeight) 
        {
            return currPosition;
        }

        currPosition += direction;

        return currPosition;
    }

    private Vector3Int RandomBasePosition(Vector3Int currPosition)
    {
        int randomIndex = Random.Range(0, basePositions.Count);
        return basePositions[randomIndex];
    }

    private Vector3Int BasePositionWalk(Vector3Int currPosition)
    {
        if (currentBasePositionIndex == basePositions.Count) return currPosition;
        return basePositions[currentBasePositionIndex++];
    }

    private void CreateObjects(in GameObject[] arrayToInstantiate)
    {
        int randomAmount = Random.Range(20, 25);
        for (int i = 0; i < randomAmount; i++)
        {
            int randomGameObjectIndex = Random.Range(0, arrayToInstantiate.Length);
            currentBasePositionIndex = Random.Range(0, basePositions.Count);

            if (MyInstantiate(arrayToInstantiate[randomGameObjectIndex], basePositions[currentBasePositionIndex])) debrisCount++;
            int clumpLength = Random.Range(1, 5);
            for (int j = 0; j < clumpLength; j++)
            {
                if(MyInstantiate(arrayToInstantiate[randomGameObjectIndex], (Vector3) BasePositionWalk(new Vector3Int(0,0,0)))) debrisCount++;
            }
        }
    }

    private bool MyInstantiate(GameObject objectToInstantiate, Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        if (colliders.Length == 0)
        {
            GameObject instantiatedObject = Instantiate(objectToInstantiate, position, Quaternion.identity);
            instantiatedObject.transform.SetParent(boardParent);
            return true;
        }

        return false;
    }

    private void SurroundMineWithColliders()
    {
        foreach(Vector3Int position in basePositions)
        {
            foreach(Vector3Int direction in randomDirections)
            {
                Vector3Int instantiatePosition = position + direction;
                if (baseTilemap.GetTile(instantiatePosition) == null)
                {
                    MyInstantiate(colliderObject, instantiatePosition);
                }
            }
        }
    }

    public void DestroyMines()
    {
        Destroy(boardParent.gameObject);

        Collider2D[] otherStuff = Physics2D.OverlapCircleAll(transform.position, 50);

        foreach(Collider2D c in otherStuff)
        {
            if (c.CompareTag("Untagged")) Destroy(c.gameObject);
        }

        basePositions = new List<Vector3Int>();

        baseTilemap.ClearAllTiles();
        variationTilemap.ClearAllTiles();
        pathTilemap.ClearAllTiles();

        boardParent = new GameObject("Board").transform;

    }

    public void MakeLadderAppear(Vector2 position)
    {
        debrisCount--;
        int percentage = Random.Range(0, debrisCount);

        if ((debrisCount == 0 || percentage < debrisCount / 10) && level < 99)
        {
            GameObject instantiatedLadderGameObject = Instantiate(ladderGameObject, position, Quaternion.identity);
            instantiatedLadderGameObject.transform.SetParent(boardParent);
        }
    }
}

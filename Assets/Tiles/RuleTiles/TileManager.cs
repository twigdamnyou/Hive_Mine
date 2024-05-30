using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [Header("TileMap that is being managed")]
    public Tilemap tileMap;
    public Tilemap damageMap;

    public Dictionary<Vector3Int, TileStatus> tilesStatusDict = new Dictionary<Vector3Int, TileStatus>();
    public static TileManager instance;

    [Header("Type of tile that represents 'Dirt'")]
    public TileBase dirtTile;

    [Header("Type of ore that will spawn inside the mine and the frequency of that ores spawn")]
    public List<TileChanceEntry> oreTiles = new List<TileChanceEntry>();
    public float chanceForOre = 0.5f;
    private float totalOreWeights;

    [Header("Does this mine have a secret/reward in it")]
    public bool secret = false;


    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
        instance = this;
        CalculateOreWeights();
    }

    private void Start()
    {
        SetInsideTileTypes();
        FindAllTiles();
    }

    public static void FindAllTiles()
    {
        instance.tilesStatusDict.Clear();

        //Finding all positons within the TileMap
        BoundsInt mapBounds = instance.tileMap.cellBounds;
        BoundsInt.PositionEnumerator allPos = mapBounds.allPositionsWithin;

        //for each position we found in the array we just created
        //We are adding that tiles position and TileStatus to a dictionary
        foreach (Vector3Int postion in allPos)
        {
            TileBase targetTile = instance.tileMap.GetTile(postion);
            if (targetTile is AdvancedRuleTile)
            {
                AdvancedRuleTile myTile = (AdvancedRuleTile)targetTile;
                //Debug.Log("Found an advance tile: " + myTile.type);
                instance.tilesStatusDict.Add(postion, new TileStatus(myTile, postion, instance.tileMap.CellToWorld(postion)));
            }
        }
    }

    public static float GetHealthAtPos(Vector3Int position)
    {
        //access the TileStatus based on a position given in our Dict
        if (instance.tilesStatusDict.TryGetValue(position, out TileStatus targetData))
        {
            return targetData.currentHealth;
        }
        else
        {
            Debug.LogError("could not find tile at pos " + position);
            return -1f;
        }
    }

    public static void ChangeHealthAtPos(Vector3Int position, float value)
    {
        //access the TileStatus based on a position given in our Dict
        if (instance.tilesStatusDict.TryGetValue(position, out TileStatus targetData))
        {
            targetData.AdjustHealth(value);
        }
        else
        {
            //Debug.LogError("could not find tile at pos " + position);
        }

    }

    public static void DestroyTileAtPosition(Vector3Int pos)
    {
        instance.tileMap.SetTile(pos, null);
        instance.tilesStatusDict.Remove(pos);
    }

    public void UpdateSpriteAtPos(Vector3Int position)
    {
        //not yet implemented
        //Debug.LogError("could not find tile at pos " + position);
    }

    public static void ApplyDamageAtPos(Vector3Int position, TileBase damageTile)
    {
        instance.damageMap.SetTile(position, damageTile);
    }

    private void SetInsideTileTypes()
    {
        BoundsInt mapBounds = tileMap.cellBounds;
        //Debug.Log("xmax Bounds = " + mapBounds.size.x);
        //Debug.Log("ymax Bounds = " + mapBounds.size.y);

        var allPos = mapBounds.allPositionsWithin;

        foreach (Vector3Int postion in allPos)
        {
            TileBase targetTile = tileMap.GetTile(postion);
            if (targetTile is AdvancedRuleTile)
            {
                AdvancedRuleTile myTile = (AdvancedRuleTile)targetTile;

                if (myTile.type != AdvancedRuleTile.TileType.Wall)
                {
                    tileMap.SetTile(postion, SelectRandomTile());

                    //TileBase chosenTile = RollTileChance();
                    //tileMap.SetTile(postion, chosenTile);
                }
            }
        }
    }

    public TileBase RollTileChance()
    {
        float chance = UnityEngine.Random.Range(0f, 1f);

        //float lowestChance = float.MaxValue;

        //TileBase tile = null;

        for (int i = 0; i < oreTiles.Count; i++)
        {
            if (chance <= oreTiles[i].dropChance)
            {
                Debug.Log(oreTiles[i].tile.name + " Is the winner");
                return oreTiles[i].tile;
            }

            //if (oreTiles[i].dropChance < lowestChance)
            //{
            //    lowestChance = oreTiles[i].dropChance;
            //    if (chance <= lowestChance)
            //    {
            //        //Debug.Log(oreTiles[i].tile.name + " Is the winner");
            //        tile = oreTiles[i].tile;
            //    }
            //}
        }
        //Debug.LogError("Forgot to put a tile of at least rarity 1 in the tile list");
        return dirtTile;
    }

    private TileBase SelectRandomTile()
    {
        TileChanceEntry randomTile = oreTiles[GetRandomoOreIndex()];
        Debug.Log(randomTile.tile.name + " is the winner");
        return randomTile.tile;
    }

    private int GetRandomoOreIndex()
    {
        float roll = UnityEngine.Random.Range(0f, 1f) * totalOreWeights;

        for (int i = 0; i < oreTiles.Count; i++)
        {
            if (oreTiles[i].Weight >= roll)
            {
                return i;
            }
        }

        return 0;
    }

    private void CalculateOreWeights()
    {
        totalOreWeights = 0f;
        foreach (TileChanceEntry entry in oreTiles)
        {
            totalOreWeights += entry.dropChance;
            entry.Weight = totalOreWeights;
        }
    }
}

[System.Serializable]
public class TileChanceEntry
{
    public TileBase tile;

    [Range(0f, 100f)] 
    public float dropChance;

    public float Weight { get; set; }
}
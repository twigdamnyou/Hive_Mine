using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    public TileBase oreTile;
    public float chanceForOre = 0.5f;

    [Header("Does this mine have a secret/reward in it")]
    public bool secret = false;


    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
        instance = this;
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
                    //Debug.Log(myTile.type + " is not a wall tile");
                    float chance = UnityEngine.Random.Range(0f, 1f);
                    if (chance >= chanceForOre)
                        tileMap.SetTile(postion, dirtTile);
                    else
                    {
                        //Debug.Log(myTile.lootDrop == null);
                        tileMap.SetTile(postion, oreTile);
                    }
                }
            }
        }
    }

}

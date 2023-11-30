using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;

public class TileIdentifier : MonoBehaviour
{
    [Header("TileMap this Identifier is attached to")]
    public Tilemap tileMap;

    [Header("Type of tile that represents 'Dirt'")]
    public TileBase dirtTile;

    [Header("Type of ore that will spawn inside the mine and the frequency of that ores spawn")]
    public TileBase oreTile;
    public float chanceForOre = 0.5f;

    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
        //gameObject.SetActive(false);
    }

    private void Start()
    {
        SetInsideTileTypes();
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

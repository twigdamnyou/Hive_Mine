using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Custom Tile")]
public class CustomTile : RuleTile
{
    public enum TileType
    {
        Wall,
        Door,
        Loot,
        Dirt
    }

    public TileType type;
    public GameObject lootDrop;
    public float health;


    public void SpawnLoot(Vector3Int location)
    {
        Instantiate(lootDrop, location, Quaternion.identity);
    }

}

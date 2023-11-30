using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(menuName = "Custom Tile Rules")]
public class AdvancedRuleTile : RuleTile<AdvancedRuleTile.Neighbor>
{
    public bool alwaysConnect;
    public TileBase[] tilesToConnect;
    public bool checkSelf;
    public List<TileDamageThreshold> damageThresholds = new List<TileDamageThreshold>();

    public enum TileType
    {
        Wall,
        Door,
        Loot,
        Dirt
    }

    public TileType type;
    public GameObject lootDrop;
    public float maxHealth;

    #region RuleTileStuff
    public class Neighbor : AdvancedRuleTile.TilingRule.Neighbor
    {
        public const int Any = 3;
        public const int Specified = 4;
        public const int Nothing = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
            case Neighbor.Any: return CheckAny(tile);
            case Neighbor.Specified: return CheckSpecified(tile);
            case Neighbor.Nothing: return CheckNothing(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckThis(TileBase tile)
    {
        if (!alwaysConnect)
        {
            return tile == this;
        }
        else return tilesToConnect.Contains(this) || tile == this;
    }

    private bool CheckNotThis(TileBase tile)
    {
        return tile != this;
    }

    private bool CheckAny(TileBase tile)
    {
        if (checkSelf)
        {
            return tile != null;
        }
        else return tile != null && tile != this;
    }

    private bool CheckSpecified(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }

    private bool CheckNothing(TileBase tile)
    {
        return tile == null;
    }
    #endregion

    public void SpawnLoot(Vector3 location)
    {
        if (lootDrop != null)
        {
            //Debug.Log("attempting to spawn: " + lootDrop.name);
            Instantiate(lootDrop, location, Quaternion.identity);
        }
        else
        {
            Debug.Log("no loot to drop");
        }
    }

    [System.Serializable]
    public class TileDamageThreshold
    {
        public float threshold;

        public TileBase damageTile;

    }

}
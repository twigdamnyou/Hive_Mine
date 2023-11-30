using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStatus 
{
    public AdvancedRuleTile TileData { get; private set; }
    public float currentHealth;
    public float MaxHealth { get { return TileData.maxHealth; } }
    public Vector3Int Pos { get; private set; }
    public Vector3 WorldPos { get; private set; }

    public TileStatus(AdvancedRuleTile tileData, Vector3Int pos, Vector3 worldPos)
    {
        TileData = tileData;
        this.currentHealth = MaxHealth;
        Pos = pos;
        WorldPos = worldPos;
        //Debug.Log(tileData.maxHealth + " " + pos);
        //Debug.Log(tileData.lootDrop == null);
    }

    public void AdjustHealth(float ammount)
    {
        currentHealth += ammount;
        //Debug.Log(Pos + " " + currentHealth);
        //TODO: tile look up for swapping sprite or removing tile
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void CheckDamageThreshold()
    {

    }

    private void Die()
    {
        TileData.SpawnLoot(WorldPos);
        //play vfx
        //Debug.Log(Pos + " is destroyed");
        
        TileManager.DestroyTileAtPosition(Pos);
        //destroy tile
        
    }
}

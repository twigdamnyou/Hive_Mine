using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimeTile
{
    public float currentHealth;
    public Vector2Int tileCoordinate;
    public AdvancedRuleTile advancedRuleTile;

    public RunTimeTile(Vector2Int tileCoordinate, AdvancedRuleTile advancedRuleTile)
    {
        this.tileCoordinate = tileCoordinate;
        this.advancedRuleTile = advancedRuleTile;
        this.currentHealth = advancedRuleTile.maxHealth;
    }

    public bool DamageTile(float damage)
    {
        currentHealth -= damage;
        return currentHealth <= 0;
    }

}

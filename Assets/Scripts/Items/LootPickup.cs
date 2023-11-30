using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPickup : MonoBehaviour
{
    public OreClass.OreType oreType;
    private SpriteRenderer icon;

    private void Awake()
    {
        icon = GetComponentInChildren<SpriteRenderer>();
        icon.sprite = InventoryManager.instance.GetOreSprite(oreType);
    }
}

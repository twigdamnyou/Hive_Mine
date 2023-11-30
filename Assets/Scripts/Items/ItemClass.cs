using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : ScriptableObject
{
    [Header("Item")]
    public string itemName;
    public Sprite itemIcon;


    public abstract ItemClass GetItem();
    public abstract OreClass GetOre();
    public abstract MiscClass GetMisc();
    public abstract UpgradeClass GetUpgrade();

}

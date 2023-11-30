using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TICOreCollector : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LootPickup pickup = collision.GetComponent<LootPickup>();
        Miner miner = collision.GetComponent<Miner>();

        if (pickup != null)
        {
            InventoryManager.instance.AddOre(pickup.oreType, 1);
            Destroy(collision.gameObject);
        }

        if (miner != null)
        {
            foreach (KeyValuePair<OreClass.OreType, int> oreType in InventoryManager.instance.minersOreDictionary)
            {
                int value = InventoryManager.instance.GetMinersOreAmmount(oreType.Key);
                InventoryManager.instance.AddOre(oreType.Key, value);
            }
            
            InventoryManager.instance.EmptyMinerBackpack();
            miner.minerHUD.ResetBackpackBar();
        }
    }
}

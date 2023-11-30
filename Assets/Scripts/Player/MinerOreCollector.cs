using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerOreCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LootPickup pickup = collision.GetComponent<LootPickup>();

        if (pickup != null)
        {
            if (InventoryManager.instance.currentBackPackAmmount < InventoryManager.instance.maxBackPackSize)
            {
                InventoryManager.instance.MinerAddOre(pickup.oreType, 1);
                InventoryManager.instance.currentBackPackAmmount += 1;
                Destroy(collision.gameObject);
            }
            return;
        }

        //TODO: have this send a signal back to the tractor beam that its hit an pickup so that the tractor beam ca run the appropriate scripts.
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeEntry : MonoBehaviour
{
    public List<Upgrade> upgrades = new List<Upgrade>();


    private void Awake()
    {
        GetAllUpgrades();
    }

    public void GetAllUpgrades()
    {
        upgrades.Add(GetComponentInChildren<UpgradeNodeEntry>().upgrade);
        Debug.Log("Found: " + upgrades.Count + " Upgrades");
    }
}

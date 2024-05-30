using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    WeaponDamage,
    WeaponCooldown,
    WeaponRamge,

}

[CreateAssetMenu(fileName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public UpgradeType upgradeType;
    public string upgradeName;
    public List<Upgrade> prerequisiteUpgrades = new List<Upgrade>();
    public List<UpgradeCostData> upgradeCostData = new List<UpgradeCostData>();

    public float weaponDamageModifier;
    public float weaponCooldownModifier;
    public float weaponRangeModifier;



    [System.Serializable]
    public class UpgradeCostData
    {
        public OreClass.OreType oreType;
        public float cost;
    }

}

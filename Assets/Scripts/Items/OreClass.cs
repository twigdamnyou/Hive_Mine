using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new Ore Class", menuName = "Item/Ore")]
public class OreClass : ItemClass
{
    [Header("Ore")]
    public OreType oreType;
    public GameObject prefab;

    public enum OreType
    {
        Ichor,
        Amber,
        CompositeWax,
        ChitenWeave,
        Sclerimite,
        Neurotite,
        Ambrosia,
        None
    }

    public override ItemClass GetItem() { return this; }
    public override OreClass GetOre() { return this; }
    public override MiscClass GetMisc() { return null; }
    public override UpgradeClass GetUpgrade() { return null; }
}

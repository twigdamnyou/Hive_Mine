using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class Equipment
{
    public string equipmentName;
    public string equipmentDescription;
    public float cooldownTime;
    public float activeTime;
    public Entity Owner { get; protected set; }
    public bool IsActive { get; protected set; }
    public Stats EquipmentStats { get; protected set; }

    public Equipment(string equipmentName, string equipmentDescription, Entity owner, StatDataGroup defaultStats)
    {
        this.equipmentName = equipmentName;
        this.equipmentDescription = equipmentDescription;
        Owner = owner;
        EquipmentStats = new Stats(defaultStats);
    }

    public virtual bool Activate()
    {
        IsActive = true;
        return true;
    }
    public virtual bool Deactivate()
    {
        IsActive = false;
        return true;
    }
}

public class EarthquakeMachine : Equipment
{
    public LayerMask diggingMask;

    public EarthquakeMachine(string equipmentName, string equipmentDescription, Entity owner, float earthquakeDamage, float earthquakeRadius, LayerMask diggingMask, StatDataGroup defaultStats) 
        : base(equipmentName, equipmentDescription, owner, defaultStats)
    {
        this.diggingMask = diggingMask;
    }

    public override bool Activate()
    {
        base.Activate();
        InventoryManager.instance.StartCoroutine(PerformEarthquake());
        return true;
    }

    public override bool Deactivate()
    {
        base.Deactivate();
        InventoryManager.instance.StopCoroutine(PerformEarthquake());
        return true;
    }

    private IEnumerator PerformEarthquake()
    {
        while (GameManager.ticDocked == true)
        {
            Dictionary<Vector3Int, TileBase> hits = CollectHits();

            foreach (var item in hits)
            {
                Dig(item.Key, item.Value);
            }

            Debug.Log("Pulsing Earthquake");

            yield return new WaitForSeconds(2f);
        }
    }

    private Dictionary<Vector3Int, TileBase> CollectHits()
    {
        Tilemap tilemap = GameManager.currentPlanet.currentMine.tileMap;
        Vector3Int playerPos = tilemap.WorldToCell(GameManager.GetTICPosition());
        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if ((playerPos - pos).sqrMagnitude < EquipmentStats[Stat.EarthquakeRadius] * EquipmentStats[Stat.EarthquakeRadius])
            {
                tiles.Add(pos, tilemap.GetTile(pos));
            }
        }

        return tiles;
    }

    private void Dig(Vector3Int pos, TileBase tile)
    {
            AdvancedRuleTile advanceTile = tile as AdvancedRuleTile;

            if (advanceTile != null)
            {
                Debug.Log("Hit an advanced tile");
                switch (advanceTile.type)
                {
                    case AdvancedRuleTile.TileType.Wall:
                        return;
                    case AdvancedRuleTile.TileType.Loot:

                    case AdvancedRuleTile.TileType.Dirt:
                    case AdvancedRuleTile.TileType.Door:
                        TileManager.ChangeHealthAtPos(pos, -EquipmentStats[Stat.EarthquakeDamage]);
                        //tilemap.SetTile(tilePos, null);
                        break;
                    default:
                        break;
                }
            }
    }
}

public class TICTractorBeam : Equipment
{

    public TICTractorBeam(string equipmentName, string equipmentDescription, Entity owner, StatDataGroup defaultStats) 
        : base(equipmentName, equipmentDescription, owner, defaultStats)
    {

    }

    public override bool Activate()
    {
        base.Activate();
        GameManager.instance.player.ToggleTractorBeam(true);
        return true;
    }

    public override bool Deactivate()
    {
        base.Deactivate();
        GameManager.instance.player.ToggleTractorBeam(false);
        return true;
    }


}

public class Shield : Equipment
{

    public Shield(string equipmentName, string equipmentDescription, Entity owner, StatDataGroup defaultStats) 
        : base(equipmentName, equipmentDescription, owner, defaultStats)
    {
        
    }

    public override bool Activate()
    {
        base.Activate();
        return true;
    }

    public override bool Deactivate()
    {
        base.Deactivate();
        return true;
    }

    public void AdjustShield(float value, string shieldType, Entity source)
    {
        EquipmentStats.AdjustStatFlatRange(Stat.Shield, value, Stat.MaxShield);
        StatManager.SendStatChangeEvent(Owner, source, Stat.Shield, value, EquipmentStats[Stat.Shield]);

        //Owner.SendStatChangeEvent(Owner, shieldType, value, EquipmentStats.GetStat(Stat.Shield));
        //Debug.Log("Current Shields are: " + CurrentShield + " Incoming Damage: " + value);
        if (EquipmentStats.GetStat(Stat.Shield) <= 0f)
        {
            Deactivate();
            //fire an event or do something
        }
    }

    public void AdjustMaxShields(float value, string shieldType)
    {
        EquipmentStats.AdjustStatFlat(Stat.MaxShield, value);

        //Owner.SendStatChangeEvent(Owner, shieldType, value, EquipmentStats.GetStat(Stat.MaxShield));
    }

    public void ResetShieldToMax(string shieldType)
    {
        //TODO: needs to account for if there is any current shield left, dont overfill
        float currentShield = EquipmentStats.GetStat(Stat.Shield);
        float maxShield = EquipmentStats.GetStat(Stat.MaxShield);
        float resetAmmount = maxShield - currentShield;

        AdjustShield(resetAmmount, shieldType, Owner);
        Debug.Log("firing Reset Shields to max: " + currentShield + "/" + maxShield);
    }
}

public class DomeShield : Shield
{
    public DomeShield(string equipmentName, string equipmentDescription, Entity owner, StatDataGroup defaultStats)
        : base(equipmentName, equipmentDescription, owner, defaultStats)
    {

    }

    public override bool Activate()
    {
        base.Activate();
        HUD hud = PannelManager.GetPanel("HUD") as HUD;
        hud.ToggleDomeshieldUI(true);
        Owner.Movement.CanMove = false;
        //TODO: activate shield VFX
        //TODO: Fix player drifting on activate
        return true;
    }

    public override bool Deactivate()
    {
        base.Deactivate();
        HUD hud = PannelManager.GetPanel("HUD") as HUD;
        hud.ToggleDomeshieldUI(false);
        Owner.Movement.CanMove = true;
        //TODO: de-activate shield VFX
        return true;
    }
}

public class Overload : Equipment
{
    public float overloadDamage = 10f;
    public float overloadRadius = 10;
    public LayerMask diggingMask;

    public Overload(string equipmentName, string equipmentDescription, Entity owner, float overloadDamage, float overloadRadius, LayerMask diggingMask, StatDataGroup defaultStats) 
        : base(equipmentName, equipmentDescription, owner, defaultStats)
    {

    }

    public override bool Activate()
    {
        base.Activate();
        InventoryManager.instance.StartCoroutine(PerformOverload());
        return true;
    }

    public override bool Deactivate()
    {
        base.Deactivate();
        InventoryManager.instance.StopCoroutine(PerformOverload());
        return true;
    }

    private IEnumerator PerformOverload()
    {
        while (GameManager.minerDocked == false)
        {
            Dictionary<Vector3Int, TileBase> hits = CollectHits();

            foreach (var item in hits)
            {
                Dig(item.Key, item.Value);
            }

            Debug.Log("Pulsing Earthquake");

            yield return new WaitForSeconds(2f);
        }
    }

    private Dictionary<Vector3Int, TileBase> CollectHits()
    {
        Tilemap tilemap = GameManager.currentPlanet.currentMine.tileMap;
        Vector3Int playerPos = tilemap.WorldToCell(GameManager.GetTICPosition());
        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if ((playerPos - pos).sqrMagnitude < overloadRadius * overloadRadius)
            {
                tiles.Add(pos, tilemap.GetTile(pos));
            }
        }

        return tiles;
    }

    private void Dig(Vector3Int pos, TileBase tile)
    {
        AdvancedRuleTile advanceTile = tile as AdvancedRuleTile;

        if (advanceTile != null)
        {
            Debug.Log("Hit an advanced tile");
            switch (advanceTile.type)
            {
                case AdvancedRuleTile.TileType.Wall:
                    return;
                case AdvancedRuleTile.TileType.Loot:

                case AdvancedRuleTile.TileType.Dirt:
                case AdvancedRuleTile.TileType.Door:
                    TileManager.ChangeHealthAtPos(pos, -overloadDamage);
                    //tilemap.SetTile(tilePos, null);
                    break;
                default:
                    break;
            }
        }
    }
}

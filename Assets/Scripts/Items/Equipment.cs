using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;


public abstract class Equipment
{
    public string equipmentName;
    public string equipmentDescription;
    public Entity Owner { get; protected set; }
    public bool IsActive { get; protected set; }

    public Equipment(string equipmentName, string equipmentDescription, Entity owner)
    {
        this.equipmentName = equipmentName;
        this.equipmentDescription = equipmentDescription;
        Owner = owner;
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
    public float earthquakeDamage = 2f;
    public float earthquakeRadius = 10;
    public LayerMask diggingMask;

    public EarthquakeMachine(string equipmentName, string equipmentDescription, Entity owner, float earthquakeDamage, float earthquakeRadius, LayerMask diggingMask) 
        : base(equipmentName, equipmentDescription, owner)
    {
        this.earthquakeDamage = earthquakeDamage;
        this.earthquakeRadius = earthquakeRadius;
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
        Vector3Int playerPos = tilemap.WorldToCell(GameManager.GetPlayerPosition());
        Dictionary<Vector3Int, TileBase> tiles = new Dictionary<Vector3Int, TileBase>();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if ((playerPos - pos).sqrMagnitude < earthquakeRadius * earthquakeRadius)
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
                        TileManager.ChangeHealthAtPos(pos, -earthquakeDamage);
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

    public TICTractorBeam(string equipmentName, string equipmentDescription, Entity owner) 
        : base(equipmentName, equipmentDescription, owner)
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
    public float MaxShield { get; private set; }
    public float CurrentShield { get; private set; }

    public Shield(string equipmentName, string equipmentDescription, Entity owner, float maxShield) 
        : base(equipmentName, equipmentDescription, owner)
    {
        this.MaxShield = maxShield;
    }

    public override bool Activate()
    {
        base.Activate();
        CurrentShield = MaxShield;
        return true;
    }

    public override bool Deactivate()
    {
        base.Deactivate();
        return true;
    }

    public void AdjustShield(float value, string shieldType)
    {
        CurrentShield += value;

        Owner.SendStatChangeEvent(Owner, shieldType, value, CurrentShield);
        //Debug.Log("Current Shields are: " + CurrentShield + " Incoming Damage: " + value);
        if (CurrentShield <= 0f)
        {
            Deactivate();
            //fire an event or do something
        }
    }

    public void AdjustMaxShields(float value)
    {

    }
}

public class DomeShield : Shield
{
    public DomeShield(string equipmentName, string equipmentDescription, Entity owner, float maxShield)
        : base(equipmentName, equipmentDescription, owner, maxShield)
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

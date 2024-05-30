using System.Collections.Generic;
using UnityEngine;

public class Stats 
{
    /// <summary>
    /// Stats are always multiplied by the base values and not the upgraded value
    /// </summary>
    private Dictionary<Stat, float> stats = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> baseStats = new Dictionary<Stat, float>();
    public float this[Stat statName] {get {return GetStat(statName);}}

    public Stats(StatDataGroup startingData)
    {
        for (int i = 0; i < startingData.data.Count; i++)
        {
            stats.Add(startingData.data[i].statName, startingData.data[i].startingValue);
            baseStats.Add(startingData.data[i].statName, startingData.data[i].startingValue);
        }
    }

    public Stats(Stat defaultStat, float startingValue)
    {
        stats.Add(defaultStat, startingValue);
        baseStats.Add(defaultStat, startingValue);
    }

    public float GetStat(Stat stat)
    {
        if ( stats.TryGetValue(stat, out float value) )
            return value;
        else
        {
            Debug.Log($"No stat value found for {stat} on {this}");
            return 0;
        }
    }

    public float GetBaseStat(Stat stat)
    {
        if (baseStats.TryGetValue(stat, out float value))
            return value;
        else
        {
            Debug.Log($"No stat value found for {stat} on {this}");
            return 0;
        }

    }

    public void AdjustStatMulti(Stat stat, float value)
    {
        float currentValue = GetBaseStat(stat);
        currentValue *= value + 1f;

        stats[stat] = currentValue;
    }

    public void AdjustStatFlat(Stat stat, float value)
    {
        float currentValue = GetBaseStat(stat);
        currentValue += value;

        stats[stat] = currentValue;
    }

    public void AdjustStatFlatRange(Stat stat, float value, Stat statCap)
    {
        float currentValue = GetBaseStat(stat);
        float maxValue = GetBaseStat(statCap);
        if (currentValue + value > maxValue)
        {
            currentValue = maxValue;
        }
        else
        {
            currentValue += value;
        }

        stats[stat] = currentValue;
    }

    public void AdjustStatMultiRange(Stat stat, float value, Stat statCap)
    {
        float currentValue = GetBaseStat(stat);
        float maxValue = GetBaseStat(statCap);
        if (currentValue * (value + 1f) > maxValue )
        {
            currentValue = maxValue;
        }
        else
        {
            currentValue *= value + 1f;
        }

        stats[stat] = currentValue;
    }

    public void SetStatToValue(Stat stat, float value)
    {
        float currentValue = GetBaseStat(stat);
        currentValue = value;
        stats[stat] = currentValue;
    }
}

public enum Stat
{
    None,
    Health,
    MaxHealth,
    Shield,
    MaxShield,
    Speed,
    VerticalSpeed,
    HorizontalSpeed,
    RotationSpeed,
    LifeTime,
    Cooldown,
    ActiveTime,
    Accuracy,
    Damage,
    ShotDelay,
    ShotCount,
    SpaceThrustSpeed,
    SpaceRotationSpeed,
    PlaneteryHorizontalSpeed,
    EscapePlanetLaunchSpeed,
    BasePlanetLaunchSpeed,
    BlowbackDamage,
    DetectionRange,
    AttackRange,
    ChaseDistance,
    FleeDistance,
    MaxLaserDistance,
    VacumForce,
    EarthquakeDamage,
    EarthquakeRadius,
    MaxChain,
    ChainRange,
    ActivePlanetLaunchSpeed,



}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatManager
{
    //joel thinks the multi bool is garbage maybe fix

    public static void AdjustStat(Entity target, Entity source, float value, Stat stat, bool multi)
    {
        if (multi == true)
        {
            target.MyStats.AdjustStatMulti(stat, value);
        }
        else
        {
            target.MyStats.AdjustStatFlat(stat, value);
        }

        float currentValue = target.MyStats.GetStat(stat);
        SendStatChangeEvent(target, source, stat, value, currentValue);
    }

    public static void AdjustStatRange(Entity target, Entity source, float value, Stat stat, Stat maxStat,bool multi)
    {
        if (multi == true)
        {
            target.MyStats.AdjustStatMultiRange(stat, value, maxStat);
        }
        else
        {
            target.MyStats.AdjustStatFlatRange(stat, value, maxStat);
        }

        float currentValue = target.MyStats.GetStat(stat);
        SendStatChangeEvent(target, source, stat, value, currentValue);
    }

    public static void DealDamage(Entity target, Entity source, float value)
    {
        if (value < 0f)
        {
            float damageAfterShields = target.HandleShields(value, source);
            
            target.MyStats.AdjustStatFlat(Stat.Health, damageAfterShields);

            float currentValue = target.MyStats.GetStat(Stat.Health);
            SendStatChangeEvent(target, source, Stat.Health, value, currentValue);
        }
        else
        {
            Debug.LogError($"NonNegative Damage Being dealt to: {target.gameObject.name} from: {source.gameObject.name}");
        }
    }

    public static void SetStatToValue(Entity target, Entity source, Stat stat, float value)
    {
        float previousValue = target.MyStats.GetStat(stat);
        target.MyStats.SetStatToValue(stat, value);
        float currentValue = target.MyStats.GetStat(stat);
        SendStatChangeEvent(target, source, stat, value - previousValue, currentValue);
    }

    public static void SendStatChangeEvent(Entity entity, Entity source, Stat stat, float valueChanged, float currentValue)
    {
        EventData eventData = new EventData();

        eventData.AddEntity("Target", entity);
        eventData.AddEntity("Source", source);
        eventData.AddFloat("ValueChanged", valueChanged);
        eventData.AddFloat("CurrentValue", currentValue);
        eventData.AddStat("StatType", stat);

        EventManager.SendEvent(EventManager.GameEvent.StatChanged, eventData);
    }



}

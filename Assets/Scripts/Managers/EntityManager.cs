using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    //public Entity playerPrefab;
    public Entity enemyPrefab;


    private Dictionary<Entity.EntityType, List<Entity>> activateEntities =
        new Dictionary<Entity.EntityType, List<Entity>>();

    public static EntityManager instatce;

    private void Awake()
    {
        if (instatce == null)
        {
            instatce = this;
        }
    }

    public void RegisterEntity(Entity target)
    {
        if (activateEntities.ContainsKey(target.entityType) == true)
        {
            activateEntities[target.entityType].Add(target);
        }
        else
        {
            List<Entity> newEntityList = new List<Entity>();
            newEntityList.Add(target);

            activateEntities.Add(target.entityType, newEntityList);
        }
    }

    public void RemoveEntity(Entity target)
    {
        if (activateEntities.TryGetValue(target.entityType, out List<Entity> resuslts) == true)
        {
            resuslts.Remove(target);
            if (GetActiveEnemyCount() == 0)
            {
                //query spawn manager and ask if its still spawning enemeis
                if (SpawnManager.instance.waveActivelySpawning == false)
                {
                    EventData eventData = new EventData();
                    eventData.AddBool("WaveStatus", false);
                    EventManager.SendEvent(EventManager.GameEvent.WaveEnded, eventData);
                    Debug.Log("WaveEnded Event Sent");
                }
            }
        }
    }

    public static int GetActiveEnemyCount()
    {
        if (instatce.activateEntities.ContainsKey(Entity.EntityType.Enemy) == false)
        {
            return 0;
        }
        return instatce.activateEntities[Entity.EntityType.Enemy].Count;
    }

    public static void KillAllEnemies()
    {
        if(instatce.activateEntities.ContainsKey(Entity.EntityType.Enemy) == false)
            return;
        for (int i = instatce.activateEntities[Entity.EntityType.Enemy].Count -1; i >= 0; i--)
        {
            instatce.activateEntities[Entity.EntityType.Enemy][i].ForceDie();
        }
    }
}

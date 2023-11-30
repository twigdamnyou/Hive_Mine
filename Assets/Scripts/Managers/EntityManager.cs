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
        }
    }
}

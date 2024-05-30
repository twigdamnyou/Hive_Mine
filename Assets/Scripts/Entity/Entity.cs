using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public abstract class Entity : MonoBehaviour
{
    public enum EntityType
    {
        Tick,
        Player,
        Projectile,
        Ore,
        Pickup,
        Enemy
    }

    public EntityType entityType;
    public EntityMovement Movement { get; private set; }
    public bool ignorePlanetGravity = false;
    public Stats MyStats { get; private set; }
    [Header("Stat Definitions")]
    public StatDataGroup startingStats;

    //public float armor = 5f;

    public bool debugMode;

    [Header("VFX")]
    public GameObject deathEffectPrefab;
    public GameObject spawnEffectPrefab;

    protected virtual void Awake()
    {
        MyStats = new Stats(startingStats);
        Movement = GetComponent<EntityMovement>();
        if (ignorePlanetGravity == true)
        {
            PlanetManager.RemoveGravityInteration(GetComponent<Collider2D>());
        }
    }

    protected virtual void Start()
    {
        if (MyStats.GetStat(Stat.Health) != MyStats.GetStat(Stat.MaxHealth))
        {
            MyStats.SetStatToValue(Stat.Health, MyStats.GetStat(Stat.MaxHealth));
        }
        


        if (entityType != EntityType.Projectile)
        {
            EntityManager.instatce.RegisterEntity(this);
        }
    }

    protected virtual void OnEnable()
    {
        SpawnEntranceVFX();
        EventManager.AddListener(EventManager.GameEvent.StatChanged, OnStatChanged);
    }

    private void OnDisable()
    {
        EventManager.RemoveMyListeners(this);
    }

    //public virtual void AdjustHealth(float value)
    //{
    //    currentHealth += value;

    //    SendStatChangeEvent(this, "Health", value, currentHealth);

    //    if (currentHealth <= 0f)
    //    {
    //        Die();
    //    }
    //}

    public virtual void OnStatChanged(EventData eventData)
    {
        //If I need more then just health stat listener then just add a switch for each different stat

        Entity target = eventData.GetEntity("Target");
        if (target != this)
            return;

        Stat stat = eventData.GetStat("StatType");

        switch (stat)
        {
            case Stat.None:
                break;
            case Stat.Health:      
                if (stat <= 0)
                    Die();
                break;
            case Stat.MaxHealth:
                break;
        }
    }

    public virtual float HandleShields(float incomingDamage, Entity source)
    {
        //TODO: put shield logic here

        return incomingDamage;
    }

    protected virtual void Die()
    {
        //Debug.Log(gameObject.name + "dying");
        if (debugMode == true)
        {
            Debug.Log("Debug mode enabled: I AM DEATH DESTROYER OF WORLDS");
            return;
        }
        Destroy(gameObject);
    }

    public void ForceDie()
    {
        Die();
    }

    #region Events


    #endregion

    #region VFX
    protected void SpawnDeathVfx()
    {
        if (deathEffectPrefab == null)
        {
            return;
        }
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }

    protected void SpawnEntranceVFX()
    {
        if (spawnEffectPrefab == null)
        {
            return;
        }
        Instantiate(spawnEffectPrefab, transform.position, Quaternion.identity);

    }
    #endregion
}

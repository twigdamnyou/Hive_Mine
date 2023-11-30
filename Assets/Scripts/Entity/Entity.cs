using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Stat Definitions")]
    public float maxHealth = 20f;
    public float currentHealth;

    public float armor = 5f;

    public bool debugMode;

    [Header("VFX")]
    public GameObject deathEffectPrefab;
    public GameObject spawnEffectPrefab;

    protected virtual void Awake()
    {
        Movement = GetComponent<EntityMovement>();
        if (ignorePlanetGravity == true)
        {
            PlanetManager.RemoveGravityInteration(GetComponent<Collider2D>());
        }
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;


        if (entityType != EntityType.Projectile)
        {
            EntityManager.instatce.RegisterEntity(this);
        }
    }

    protected virtual void OnEnable()
    {
        SpawnEntranceVFX();
    }

    public virtual void AdjustHealth(float value)
    {


        currentHealth += value;

        SendStatChangeEvent(this, "Health", value, currentHealth);
        
        if (currentHealth <= 0f)
        {
            Die();
        }       
    }

    
    protected virtual void Die()
    {
        //Debug.Log(gameObject.name + "dying");
        if (debugMode == true)
        {
            return;
        }
        Destroy(gameObject);
    }

    #region Events

    public virtual void SendStatChangeEvent(Entity entity, string stat, float valueChanged, float currentValue)
    {
        EventData eventData = new EventData();

        eventData.AddEntity("Target", entity);
        eventData.AddFloat("ChangeValue", valueChanged);
        eventData.AddFloat("CurrentTotal", currentValue);
        eventData.AddString("StatType", stat);

        EventManager.SendEvent(EventManager.GameEvent.StatChanged, eventData);
    }

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

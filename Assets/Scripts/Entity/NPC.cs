using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Entity
{
    public AIBrain Brain { get; protected set; }

    [Header("Stat Definitions")]
    public float detectionRange = 50f;
    public float attackRange = 10f;

    protected override void Awake()
    {
        base.Awake();
        Brain = GetComponent<AIBrain>();
    }

    protected override void Die()
    {
        //play death anim
        //drop loot
        //privide experience

        base.Die();

        EntityManager.instatce.RemoveEntity(this);
        SpawnDeathVfx();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Brain.alwaysOnBehaviors.Contains(AIBrain.Behavior.DeathOnContact) == false)
            return;

        if(collision.gameObject.layer == 10)
            Die();
    }
}

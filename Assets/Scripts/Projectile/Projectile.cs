using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : Entity
{
    public enum ProjectileProperty
    {
        Standard,
        Pierce,
        Seeking,
        Chaining,
        Spliting
    }

    [Header("Projectile Properties")]
    public float projectileSpeed = 20f;
    public float projectileLifeTime = 2f;
    public List<ProjectileProperty> projectileProperties = new List<ProjectileProperty>();

    [Header("Chaining Properties")]
    public int maxChain = 1;
    public int chainingRange = 10;
    private int currentChainCount;
    private List<Collider2D> previousChainHits = new List<Collider2D>();

    public bool projectileCanMove;
    private int sourceLayer;

    [Header("Projectile Components")]
    private Rigidbody2D myBody;
    private Collider2D myCollider;
    private Entity source;
    private Weapon parentWeapon;

    [Header("On Projectile Death")]
    private GameObject onDeathEffect;

    #region Built In Methods

    protected override void Awake()
    {
        base.Awake();
        myCollider = GetComponent<Collider2D>();
        myBody = GetComponent<Rigidbody2D>();


        StartCoroutine(KillAfterLifetime());
    }

    private void FixedUpdate()
    {
        if (projectileCanMove == true)
            Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentWeapon == null)
            Debug.LogError("Parent Weapon is null when checking layers on projectile collision");

        Entity otherEntity = other.gameObject.GetComponent<Entity>();

        if (otherEntity != null)
        {
            if (otherEntity.gameObject.layer == sourceLayer)
            {
                return;
            }
            DealDamage(otherEntity);
        }

        if (projectileProperties.Contains(ProjectileProperty.Chaining) && currentChainCount < maxChain)
        {
            PerformChain(other);
            return;
        }
        //Debug.Log("collided with: "+ other.gameObject.name);
        CleanUp();
    }

    #endregion

    #region Collision Ignore

    public void IgnoreCollision(Entity target)
    {
        SetUpCollisonIgnore(target.GetComponent<Collider2D>());
    }

    public void SetUpCollisonIgnore(Collider2D ownersCollider)
    {
        Physics2D.IgnoreCollision(ownersCollider, myCollider);
    }

    #endregion

    #region Custom Methods

    public void SetupProjectile(Entity source, Weapon parentWeapon)
    {
        this.source = source;
        this.parentWeapon = parentWeapon;
        sourceLayer = source.gameObject.layer;

        SetUpCollisonIgnore(source.GetComponent<Collider2D>());

        StartCoroutine(ChainOnDelay());
    }

    private IEnumerator ChainOnDelay()
    {
        yield return new WaitForEndOfFrame();
        if (projectileProperties.Contains(ProjectileProperty.Chaining) && currentChainCount < maxChain)
        {
            myBody.velocity = Vector2.zero;
            projectileCanMove = false;
            PerformChain(null);
        }
    }

    private void Move()
    {
        myBody.AddForce(transform.up * projectileSpeed * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void DealDamage(Entity target)
    {
        target.AdjustHealth(-parentWeapon.weaponData.projectileDamage);
    }

    private IEnumerator KillAfterLifetime()
    {
        WaitForSeconds waiter = new WaitForSeconds(projectileLifeTime);
        yield return waiter;

        CleanUp();
    }

    private void CleanUp()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Projectile Behaviors

    private void PerformChain(Collider2D mostRecentTarget)
    {
        currentChainCount++;
        List<Collider2D> listNearbyTargets = Physics2D.OverlapCircleAll(transform.position, chainingRange, parentWeapon.targetLayerMask).ToList();
        List<Collider2D> validTargets = new List<Collider2D>();
        for (int i = 0; i < listNearbyTargets.Count; i++)
        {
            if (listNearbyTargets[i] == mostRecentTarget)
            {
                continue;
            }
            if (previousChainHits.Contains(listNearbyTargets[i]))
            {
                continue;
            }

            validTargets.Add(listNearbyTargets[i]);
        }

        if (validTargets.Count == 0)
        {
            CleanUp();
            return;
        }

        Collider2D nearestTarget = TargetUtilities.FindNearestTarget(validTargets.ToArray(), transform);

        myBody.velocity = Vector2.zero;
        projectileCanMove = false;
        transform.position = nearestTarget.transform.position;
        previousChainHits.Add(nearestTarget);
    }

    #endregion
}

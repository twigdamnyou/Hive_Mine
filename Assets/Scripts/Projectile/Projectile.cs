using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : Entity
{
    public enum ProjectileProperty
    {
        Standard,
        Pierce,
        Seeking,
        Chaining,
        Spliting,
        Arcing
    }

    [Header("Projectile Properties")]
    public List<ProjectileProperty> projectileProperties = new List<ProjectileProperty>();

    [Header("Chaining Properties")]
    private int currentChainCount;
    private List<Collider2D> previousChainHits = new List<Collider2D>();

    [Header("Arcing Properties")]
    public float arcHeightY = 1;
    public float arcDuration = 1f;
    public AnimationCurve arcCurve;


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

        //if (projectileProperties.Contains(ProjectileProperty.Arcing))
        ////{
        ////    myBody.AddForce(transform.up * projectileSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        ////}
        StartCoroutine(KillAfterLifetime());
    }

    private void FixedUpdate()
    {
        if (projectileProperties.Contains(ProjectileProperty.Arcing))
            StartCoroutine(PerformArc(source.transform.position, parentWeapon.target.transform.position));
        else if (projectileCanMove == true)
            Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (parentWeapon == null)
            Debug.LogError("Parent Weapon is null when checking layers on projectile collision");

        if (other.transform.gameObject.layer == 18)
        {
            return;
        }

        Entity otherEntity = other.gameObject.GetComponent<Entity>();

        if (otherEntity != null)
        {
            if (otherEntity.gameObject.layer == sourceLayer)
            {
                return;
            }
            DealDamage(otherEntity);
        }

        if (projectileProperties.Contains(ProjectileProperty.Chaining) && currentChainCount < MyStats[Stat.MaxChain])
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
        if (projectileProperties.Contains(ProjectileProperty.Chaining) && currentChainCount < MyStats[Stat.MaxChain])
        {
            myBody.velocity = Vector2.zero;
            projectileCanMove = false;
            PerformChain(null);
        }
    }

    private void Move()
    {
        if (projectileProperties.Contains(ProjectileProperty.Arcing))
            return;

        if (parentWeapon.target != null && projectileProperties.Contains(ProjectileProperty.Seeking))
            TargetUtilities.RotateSmoothlyTowardTarget(parentWeapon.target.transform, transform, MyStats[Stat.RotationSpeed]);

        myBody.AddForce(transform.up * MyStats[Stat.Speed] * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void DealDamage(Entity target)
    {
        StatManager.DealDamage(target, source, -parentWeapon.GetModifiedWeaponDamage());
    }

    private IEnumerator KillAfterLifetime()
    {
        WaitForSeconds waiter = new WaitForSeconds(MyStats[Stat.LifeTime]);
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
        List<Collider2D> listNearbyTargets = Physics2D.OverlapCircleAll(transform.position, MyStats[Stat.ChainRange], parentWeapon.targetLayerMask).ToList();
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



    private IEnumerator PerformArc(Vector3 start, Vector3 finish)
    {
        var timePast = 0f;


        //temp vars
        while (timePast < arcDuration)
        {
            timePast += Time.deltaTime;

            var linearTime = timePast / arcDuration; //0 to 1 time
            var heightTime = arcCurve.Evaluate(linearTime); //value from curve

            var height = Mathf.Lerp(0f, arcHeightY, heightTime); //clamped between the max height and 0

            transform.position =
                Vector3.Lerp(start, finish, linearTime) + new Vector3(0f, height, 0f); //adding values on y axis

            yield return null;
        }
    }

    //private void PerformArc()
    //{
    //    Vector2 startPos = source.transform.position;
    //    Vector2 targetPos = parentWeapon.target.transform.position;

    //    float x0 = startPos.x;
    //    float x1 = targetPos.x;
    //    float dist = x1 - x0;
    //    float nextX = Mathf.MoveTowards(transform.position.x, x1, projectileSpeed * Time.deltaTime);
    //    float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
    //    float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
    //    Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);

    //    // Rotate to face the next position, and then move there
    //    transform.rotation = TargetUtilities.LookAt2D(nextPos - transform.position);
    //    transform.position = nextPos;
    //}

    //IEnumerator PerformArc()
    //{
    //    //TempVar
    //    float gravity = 25f;
    //    Vector3 startPos = source.transform.position;
    //    Vector3 endPos = parentWeapon.target.transform.position;

    //    // Short delay added before Projectile is thrown
    //    yield return new WaitForSeconds(0f);

    //    // Calculate distance to target
    //    float target_Distance = Vector3.Distance(startPos, endPos);

    //    // Calculate the velocity needed to throw the object to the target at specified angle.
    //    float projectile_Velocity = target_Distance / (Mathf.Sin(2 * privateRotation * Mathf.Deg2Rad) / gravity);

    //    // Extract the X  Y componenent of the velocity
    //    float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(privateRotation * Mathf.Deg2Rad);
    //    float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(privateRotation * Mathf.Deg2Rad);

    //    // Calculate flight time.
    //    float flightDuration = target_Distance / Vx;

    //    // Rotate projectile to face the target.
    //    transform.rotation = Quaternion.LookRotation(endPos - transform.position);

    //    float elapse_time = 0;

    //    while (elapse_time < flightDuration)
    //    {
    //        transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

    //        elapse_time += Time.deltaTime;

    //        yield return null;
    //    }
    //}

    #endregion

}


using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [Header("Projectile Field")]
    public Transform projectileSpawnLocation;

    public bool canAttack;

    [Header("Weapon Components")]
    private Entity owner;
    public WeaponData weaponData;

    private Timer weaponCooldownTimer;

    private LineOfSight lineOfSight;

    public LayerMask targetLayerMask;

    #region Built In Methods

    private void Update()
    {
        UpdateTimer(weaponCooldownTimer, canAttack == false && GameManager.minerDocked == true);
    }

    #endregion

    #region CustomMethods

    public void SetUp()
    {
        owner = GetComponentInParent<Entity>();

        lineOfSight = owner.GetComponentInChildren<LineOfSight>();

        if (weaponData == null)
        {
            Debug.LogError("A weapon: " + gameObject.name + " has null weapon data. You forgot to assigne it in the inspector");
            return;
        }

        SetUpTimer();
    }

    public IEnumerator FireWithDelay()
    {
        if (RangeCheck() == false)
            yield break;

        WaitForSeconds waiter = new WaitForSeconds(weaponData.shotDelay);
        for (int counter = 0; counter < weaponData.shotCount; counter++)
        {
            Fire();
            yield return waiter;
        }
    }

    private void Fire()
    {
        Projectile activeProjectile = Instantiate(weaponData.payload, projectileSpawnLocation.position, projectileSpawnLocation.rotation);
        //Debug.Log(gameObject.name + " is fireing " + activeProjectile.gameObject.name);

        activeProjectile.SetupProjectile(owner, this);
        float inaccuracy = (1f - weaponData.weaponAccuracy) * 360f;
        activeProjectile.transform.eulerAngles += new Vector3(0f, 0f, Random.Range(-inaccuracy, inaccuracy));

        //canAttack = false;
    }

    private void ResetCooldown()
    {
        canAttack = true;
    }

    private void UpdateTimer(Timer timer, bool shouldUpdate)
    {
        if (timer != null && shouldUpdate == true)
        {
            timer.UpdateClock();
        }
    }

    public void SetUpTimer()
    {
        weaponCooldownTimer = new Timer(weaponData.weaponCooldown, ResetCooldown, true);
    }

    #endregion

    #region Modify Weapon
    public void ModifyWeaponCooldown(float ammount)
    {
        weaponCooldownTimer.ModifyDuration(ammount);
    }

    private void OverrideCooldown()
    {
        canAttack = true;
        weaponCooldownTimer.ResetTimer();
    }

    #endregion

    #region NPC
    private bool RangeCheck()
    {
        if (lineOfSight != null)
        {
            if (lineOfSight.Hit == false)
            {
                //Debug.LogWarning(owner.gameObject.name + " cant fire");

                return false;
            }
        }
        else
        {
            return true;
        }

        float weaponRange = ((NPC)owner).attackRange;

        if (weaponRange <= 0)
            return true;

        if (owner is NPC)
        {
            NPC npc = (NPC)owner;

            Entity target = npc.Brain.Sensor.LatestTarget;

            if (target != null)
            {
                float distance = Vector2.Distance(owner.transform.position, target.transform.position);
                if (distance > weaponRange)
                    return false;
            }
        }

        //Debug.LogWarning(owner.gameObject.name + " is fireing");

        return true;
    }
    #endregion
}


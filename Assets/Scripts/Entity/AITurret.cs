using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AITurret : MonoBehaviour
{

    public AISensor sensor;
    public Weapon weapon;

    [Header("Traverse")]
    [Tooltip("Speed at which the turret can rotate left/right.")]
    public float traverseSpeed = 60f;

    private void FixedUpdate()
    {
        if (sensor.LatestTarget == null)
        {
            //Debug.Log("No Target");
            return;
        }

        if (weapon.weaponData.lobedWeapon == false)
        {
            TargetUtilities.RotateSmoothlyTowardTarget(sensor.LatestTarget.transform.position, transform, traverseSpeed);
        }
        else
        {
            float x = (sensor.LatestTarget.transform.position.x - transform.position.x);
            float y = (sensor.LatestTarget.transform.position.y - transform.position.y);
            float launchForce = weapon.weaponData.payload.MyStats[Stat.Speed];
            float arcAngleNeededToHit = TargetUtilities.CalculateAngleOfArc(x, y, 20f, launchForce, false);


            TargetUtilities.RotateSmoothlyTowardAngle(arcAngleNeededToHit, transform, traverseSpeed);
        }
        
    }

}

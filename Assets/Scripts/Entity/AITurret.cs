using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurret : MonoBehaviour
{

    public AISensor sensor;

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

        //Debug.Log("Rotating Turret");
        TargetUtilities.RotateSmoothlyTowardTarget(sensor.LatestTarget.transform.position, transform, traverseSpeed);
    }

}

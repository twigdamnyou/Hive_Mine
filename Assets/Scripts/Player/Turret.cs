using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Turret : MonoBehaviour
{
    [Header("Rotations")]
    public Transform turretPivot;

    [Header("Traverse")]
    [Tooltip("Speed at which the turret can rotate left/right.")]
    public float traverseSpeed = 60f;

    [Tooltip("When true, the turret can only rotate horizontally with the given limits.")]
    [SerializeField] private bool hasLimitedTraverse = false;
    [Range(0, 179)] public float LeftLimit = 120f;
    [Range(0, 179)] public float RightLimit = 120f;
    public Vector2 clampVector = new Vector2(-120f, 120f);


    [Header("Behavior")]
    [Tooltip("When idle, the turret does not aim at anything and simply points forwards.")]
    public bool isIdle = false;

    [Tooltip("Position the turret will aim at when not idle. Set this to whatever you want" +
        "the turret to actively aim at.")]
    public Vector3 AimPosition = Vector3.zero;

    [Tooltip("When the turret is within this many degrees of the target, it is considered aimed.")]
    //[SerializeField] private float aimedThreshold = 5f;
    private float limitedTraverseAngle = 0f;
    //private float angleToTarget = 0f;
    //private bool isAimed = false;
    private bool isBaseAtRest = false;

    private void Awake()
    {
        if (turretPivot == null)
            Debug.LogError(name + ": TurretAim requires an assigned TurretBase!");
    }

    private void Update()
    {
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        AimPosition = mousPos;

        SwitchToIdle();

        if (isIdle)
        {
            if (!isBaseAtRest)
                RotateTurretToIdle();
            //isAimed = false;
        }
        else
        {
            RotateTurretToFaceTarget(AimPosition);

            // Turret is considered "aimed" when it's pointed at the target.
            //angleToTarget = GetTurretAngleToTarget(AimPosition);

            // Turret is considered "aimed" when it's pointed at the target.
            //isAimed = angleToTarget < aimedThreshold;

            //isBaseAtRest = false;
        }
    }

    private float GetTurretAngleToTarget(Vector3 targetPosition)
    {
        float angle = 999f;

        Vector3 flattenedTarget = Vector3.ProjectOnPlane(targetPosition - turretPivot.position, turretPivot.up);

        angle = Vector3.Angle(flattenedTarget - turretPivot.position, turretPivot.forward);

        return angle;
    }

    private void RotateTurretToFaceTarget(Vector3 targetPosition)
    {
        //Vector2 turretUp = transform.up;

        //Vector2 vecToTarget = targetPosition - turretPivot.position;
        //Vector3 flattenedVecForBase = Vector3.ProjectOnPlane(vecToTarget, turretUp);

        if (hasLimitedTraverse == true)
        {
            //float mx = Input.GetAxis("Mouse X") * Time.deltaTime * traverseSpeed;
            //float my = Input.GetAxis("Mouse Y") * Time.deltaTime * traverseSpeed;

            //Vector3 rot = turretPivot.transform.rotation.eulerAngles + new Vector3(-my, mx, 0f);
            //rot.x = TargetUtilities.ClampAngle(rot.x, -60f, 60f);

            //turretPivot.transform.eulerAngles = rot;
            
            transform.rotation = TargetUtilities.GetClampedRotation(targetPosition, turretPivot.transform, traverseSpeed, clampVector);

            //Vector2 turretForward = transform.forward;
            //float targetTraverse = Vector2.SignedAngle(turretForward, turretUp);

            //targetTraverse = Mathf.Clamp(targetTraverse, -LeftLimit, RightLimit); 
            //limitedTraverseAngle = Mathf.MoveTowards(limitedTraverseAngle, targetTraverse, TraverseSpeed * Time.deltaTime);

            //if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon) 
            //    turretPivot.localEulerAngles = Vector2.up * limitedTraverseAngle;
        }
        else
        {
            //Debug.Log("should be rotating smoothly");
            TargetUtilities.RotateSmoothlyTowardsMouse(turretPivot, traverseSpeed);
        }
    }

    private void RotateTurretToIdle()
    {
        // Rotate the base to its default position.
        if (hasLimitedTraverse == true)
        {
            limitedTraverseAngle = Mathf.MoveTowards(limitedTraverseAngle, 0f, traverseSpeed * Time.deltaTime);

            if (Mathf.Abs(limitedTraverseAngle) > Mathf.Epsilon)
                turretPivot.localEulerAngles = Vector3.up * limitedTraverseAngle;
            else
                isBaseAtRest = true;
        }
        else
        {
            turretPivot.rotation = Quaternion.RotateTowards(turretPivot.rotation, transform.rotation, traverseSpeed * Time.deltaTime);

            isBaseAtRest = Mathf.Abs(turretPivot.localEulerAngles.y) < Mathf.Epsilon;
        }
    }

    private void SwitchToIdle()
    {
        isIdle = !isIdle;
        isIdle = !GameManager.minerDocked;
    }

}

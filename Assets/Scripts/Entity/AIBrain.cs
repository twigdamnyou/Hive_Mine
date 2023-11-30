using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public enum Behavior
    {
        MaintainDistance,
        MoveTowards,
        MoveAway,
        MaintainPosition,
        Attack
    }

    public AISensor Sensor { get; private set; }
    public NPCMovement Movement { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public NPC Owner { get; private set; }

    private Behavior currentBehavior;
    public Behavior defaultBehavior;

    public bool hasTarget;

    private void Awake()
    {
        Owner = GetComponent<NPC>();
        Movement = GetComponent<NPCMovement>();
        WeaponManager = GetComponent<WeaponManager>();
        Sensor = GetComponentInChildren<AISensor>();
    }

    private void Start()
    {
        Sensor.Initialize(Owner, this);
        currentBehavior = defaultBehavior;
    }

    private void Update()
    {
        hasTarget = GetLatestSensorTarget();
        DetermineBehavior();
        ExecuteCurrentBehavior();
    }

    public void FireWeapon()
    {
        WeaponManager.FireCurrentWeapon();
    }

    public bool GetLatestSensorTarget()
    {
        if (Sensor.LatestTarget == null)
        {
            Movement.SetTarget(null);
            return false;
        }

        Movement.SetTarget(Sensor.LatestTarget.transform);
        return true;
    }

    public void DetermineBehavior()
    {
        float targetDistance = Sensor.GetDistanceToTarget();

        if (targetDistance <= Owner.attackRange && Sensor.LatestTarget != null)
        {
            currentBehavior = Behavior.Attack;
        }
        else
        {
            currentBehavior = defaultBehavior;
        }
    }

    public void ExecuteCurrentBehavior()
    {
        float targetDistance = Sensor.GetDistanceToTarget();

        switch (currentBehavior)
        {
            case Behavior.MaintainDistance:                
                Movement.MaintainDistance(targetDistance);
                break;
            case Behavior.MoveTowards:
                Movement.ChangeMoveType(NPCMovement.MovementType.MoveTowardTarget);
                break;
            case Behavior.MoveAway:
                Movement.ChangeMoveType(NPCMovement.MovementType.MoveAwayFromTarget);
                break;
            case Behavior.MaintainPosition:
                Movement.ChangeMoveType(NPCMovement.MovementType.None);
                break;
            case Behavior.Attack:
                FireWeapon();
                if (Movement.chaseDistance < targetDistance)
                    Movement.ChangeMoveType(NPCMovement.MovementType.MoveTowardTarget);
                else
                    Movement.ChangeMoveType(NPCMovement.MovementType.RotateTowardsTargetOnly);               
                break;
        }
    }


}

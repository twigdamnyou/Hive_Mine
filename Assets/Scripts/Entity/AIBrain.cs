using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public enum Behavior
    {
        MaintainDistance,
        MoveTowards,
        MoveAway,
        MaintainPosition,
        Attack,
        MaintainTelepotDistance,
        DeathOnContact,
        Bullet,
    }
    public List<Behavior> currentBehaviors = new List<Behavior>();

    public AISensor Sensor { get; private set; }
    public NPCMovement Movement { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public NPC Owner { get; private set; }

    private Behavior currentBehavior;
    public Behavior defaultBehavior;
    public List<Behavior> alwaysOnBehaviors = new List<Behavior>();

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
        currentBehaviors.AddRange(alwaysOnBehaviors);
    }

    private void Update()
    {
        hasTarget = GetLatestSensorTarget();
        DetermineAttackOrDefault();
        ExecuteCurrentBehaviors();
    }

    public void FireWeapon()
    {
        WeaponManager.FireCurrentWeapon();

        if (currentBehaviors.Contains(Behavior.DeathOnContact))
            WeaponManager.DealBlowbackDamage();
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

    public void DetermineAttackOrDefault()
    {
        float targetDistance = Sensor.GetDistanceToTarget();

        if (targetDistance <= Owner.attackRange && Sensor.LatestTarget != null)
        {
            if(currentBehaviors.Contains(Behavior.Attack) == false)
                currentBehaviors.Add(Behavior.Attack);

            if(currentBehaviors.Contains(defaultBehavior) == true)
                currentBehaviors.Remove(defaultBehavior);
        }
        else
        {
            if (currentBehaviors.Contains(Behavior.Attack) == true)
                currentBehaviors.Remove(Behavior.Attack);

            if (currentBehaviors.Contains(defaultBehavior) == false)
                currentBehaviors.Add(defaultBehavior);
        }
    }

    public void ExecuteCurrentBehavior()
    {
        float targetDistance = Sensor.GetDistanceToTarget();

        switch (currentBehavior)
        {
            case Behavior.MaintainDistance:
                if (Owner.ignorePlanetGravity == true)
                    Movement.MaintainDistance(targetDistance);
                else                
                    Movement.MaintainDistancePlanet(targetDistance);               
                break;
            case Behavior.MoveTowards:
                if (Owner.ignorePlanetGravity == true)
                    Movement.ChangeMoveType(NPCMovement.MovementType.MoveTowardTarget);
                else
                    Movement.ChangeMoveType(NPCMovement.MovementType.PlanetMoveTowardsTarget);
                break;
            case Behavior.MoveAway:
                if (Owner.ignorePlanetGravity == true)
                    Movement.ChangeMoveType(NPCMovement.MovementType.MoveAwayFromTarget);
                else
                    Movement.ChangeMoveType(NPCMovement.MovementType.PlanetMoveAwayFromTarget);
                break;
            case Behavior.MaintainPosition:
                Movement.ChangeMoveType(NPCMovement.MovementType.None);
                break;
            case Behavior.Attack:
                FireWeapon();
                if (Movement.Owner.MyStats[Stat.ChaseDistance] < targetDistance)
                    Movement.ChangeMoveType(NPCMovement.MovementType.MoveTowardTarget);
                else
                    Movement.ChangeMoveType(NPCMovement.MovementType.RotateTowardsTargetOnly);               
                break;
            case Behavior.MaintainTelepotDistance:
                Movement.MaintainDistance(targetDistance);
                break;
            case Behavior.Bullet:
                Movement.ChangeMoveType(NPCMovement.MovementType.MoveTowardsTargetWithRotation);
                break;
        }
    }

    public void ExecuteBehavior(Behavior behavior)
    {
        float targetDistance = Sensor.GetDistanceToTarget();

        switch (behavior)
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
                if (Movement.Owner.MyStats[Stat.ChaseDistance] < targetDistance)
                    Movement.ChangeMoveType(NPCMovement.MovementType.MoveTowardTarget);
                else
                    Movement.ChangeMoveType(NPCMovement.MovementType.RotateTowardsTargetOnly);
                break;
            case Behavior.MaintainTelepotDistance:
                Movement.MaintainDistance(targetDistance);
                break;
           
        }
    }

    public void ExecuteCurrentBehaviors()
    {
        foreach (Behavior behavior in currentBehaviors)
        {
            ExecuteBehavior(behavior);
        }
    }


}

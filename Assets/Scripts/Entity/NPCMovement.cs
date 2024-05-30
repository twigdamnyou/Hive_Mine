using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : EntityMovement
{
    public enum MovementType
    {
        None,
        MoveTowardTarget,
        MoveAwayFromTarget,
        PlanetMoveTowardsTarget,
        PlanetMoveAwayFromTarget,
        RotateTowardsTargetOnly,
        MoveTowardsTargetWithRotation,
    }

    public MovementType moveType;
    private AIBrain brain;

    private Transform currentTarget;

    protected override void Awake()
    {
        base.Awake();
        brain = GetComponent<AIBrain>();
    }

    public void SetTarget(Transform target)
    {
        if (currentTarget == target)
            return;

        currentTarget = target;
    }

    protected override void Move()
    {
        switch (moveType)
        {
            case MovementType.None:
                break;
            case MovementType.MoveTowardTarget:
                MoveTowardTarget();
                ApplyHorzontalMovement();
                if (GameManager.currentPlanet != null)
                {
                    float distance = Vector2.Distance(MyBody.transform.position, GameManager.currentPlanet.transform.position);
                    if (distance <= 45f)
                    {
                        MoveAwayFromPoint(GameManager.currentPlanet.transform.position);
                    }
                }
                break;
            case MovementType.MoveAwayFromTarget:
                MoveAwayFromTarget();
                break;
            case MovementType.PlanetMoveTowardsTarget:
                ApplyHorzontalMovement();
                break;
            case MovementType.PlanetMoveAwayFromTarget:
                ApplyHorzontalMovement();
                break;
            case MovementType.RotateTowardsTargetOnly:
                RotateTowardTarget();
                break;
            case MovementType.MoveTowardsTargetWithRotation:
                ForwardThrust();
                RotateTowardTarget();
                break;
        }
    }

    public void ChangeMoveType(MovementType targetMovement)
    {
        moveType = targetMovement;
    }

    public void MoveTowardTarget()
    {
        if (currentTarget == null)
            return;
        if (brain == null)
            return;
        switch (brain.defaultBehavior)
        {
            case AIBrain.Behavior.MaintainTelepotDistance:
                TeleportToTarget();
                return;
        }


        MoveTowardPoint(currentTarget.transform.position);
        //ApplyHorzontalMovement();
    }

    public void MoveTowardPoint(Vector2 location)
    {
        ApplyMovement(location);
    }

    public void MoveAwayFromPoint(Vector2 location)
    {
        ApplyMovement(location, -1f);
    }

    public void MoveAwayFromTarget()
    {
        if (currentTarget == null)
            return;

        MoveAwayFromPoint(currentTarget.transform.position);
    }

    public void RotateTowardTarget()
    {
        if (currentTarget == null)
            return;

        RotateTowardPoint(currentTarget.transform.position);
    }

    public void RotateTowardPoint(Vector2 location)
    {
        //Debug.Log("rotating");
        TargetUtilities.RotateSmoothlyTowardTarget(location, Owner.transform, Owner.MyStats[Stat.RotationSpeed]);

    }

    private Vector2 BasicMovement(Vector2 location)
    {
        Vector2 direction = location - (Vector2)transform.position;
        Vector2 moveForce = direction.normalized * Owner.MyStats[Stat.VerticalSpeed] * Time.fixedDeltaTime;

        return moveForce;
    }

    private void ForwardThrust()
    {
        MyBody.AddForce(transform.up * Owner.MyStats[Stat.VerticalSpeed] * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    private void ApplyMovement(Vector2 location, float modifier = 1f)
    {
        //Debug.Log("Applying Movement");
        Vector2 desiredForce = BasicMovement(location) * modifier;
        MyBody.AddForce(desiredForce, ForceMode2D.Force);
    }

    public void ApplyHorzontalMovement()
    {
        //need to figure out whether to move left or right

        Vector2 directionToPlanet = (GameManager.currentPlanet.transform.position - transform.position).normalized;
        Vector2 directionToPlayer = (GameManager.instance.player.transform.position - transform.position).normalized;
        Vector2 perpendicularVector = Vector2.Perpendicular(directionToPlanet.normalized);

        Vector2 wantedPosition = Vector2.zero;

        if (directionToPlayer.y < 0f)
        {
            Debug.Log("Player to the left");
            wantedPosition = -perpendicularVector * Owner.MyStats[Stat.HorizontalSpeed] * Time.fixedDeltaTime;

        }
        else if (directionToPlayer.y > 0f)
        {
            Debug.Log("Player to the right");
            wantedPosition = perpendicularVector * Owner.MyStats[Stat.HorizontalSpeed] * Time.fixedDeltaTime;
        }
        else
        {
            Debug.Log("Player below");
        }



        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    bool IsLeft(Vector2 A, Vector2 B)
    {
        return -A.x * B.y + A.y * B.x < 0;
    }


    public void MaintainDistance(float targetDistance)
    {
        if (targetDistance > Owner.MyStats[Stat.ChaseDistance])
        {
            ChangeMoveType(MovementType.MoveTowardTarget);
        }
        else if (targetDistance < Owner.MyStats[Stat.FleeDistance])
        {
            ChangeMoveType(MovementType.MoveAwayFromTarget);
        }
        else if (targetDistance < Owner.MyStats[Stat.ChaseDistance] && targetDistance > Owner.MyStats[Stat.FleeDistance])
        {
            ChangeMoveType(MovementType.None);
        }
    }

    public void MaintainDistancePlanet(float targetDistance)
    {
        if (targetDistance > Owner.MyStats[Stat.ChaseDistance])
        {
            ChangeMoveType(MovementType.PlanetMoveTowardsTarget);
        }
        else if (targetDistance < Owner.MyStats[Stat.FleeDistance])
        {
            ChangeMoveType(MovementType.PlanetMoveAwayFromTarget);
        }
        else if (targetDistance < Owner.MyStats[Stat.ChaseDistance] && targetDistance > Owner.MyStats[Stat.FleeDistance])
        {
            ChangeMoveType(MovementType.None);
        }
    }

    public void TeleportToTarget()
    {
        transform.position = FindValidTeleportPoint();
        Debug.Log("attempting to Teleport");
    }

    public Vector2 FindValidTeleportPoint()
    {
        List<Vector2> onScreenSpawnPoints = new List<Vector2>();
        List<Vector2> validSpawnPoints = new List<Vector2>();

        Vector2 screenCenter = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));

        foreach (Transform spawnPoint in GameManager.currentPlanet.enemySpawnPoints)
        {
            bool onScreen = TargetUtilities.IsTransformOnScreen(spawnPoint);

            if (onScreen == true)
            {
                if (Vector2.Distance(spawnPoint.position, GameManager.GetTICPosition()) > 10f)
                {
                    onScreenSpawnPoints.Add(spawnPoint.position);
                }
            }
        }

        Vector2 closest = TargetUtilities.FindNearestVector(onScreenSpawnPoints, screenCenter);
        onScreenSpawnPoints.Remove(closest);
        validSpawnPoints.Add(closest);

        Vector2 secondClosest = TargetUtilities.FindNearestVector(onScreenSpawnPoints, screenCenter);
        validSpawnPoints.Add(secondClosest);

        int randomIndex = validSpawnPoints.GetRandomIndex();

        if (validSpawnPoints.Count == 0)
        {
            Debug.LogError("Attempted to find valid teleport points and failed");
            return transform.position;
        }
        return validSpawnPoints[randomIndex];
    }
}

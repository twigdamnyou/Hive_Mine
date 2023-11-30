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
    }

    public MovementType moveType;

    private Transform currentTarget;

    public float verticalForce = 200f;
    public float horizontalForce = 200f;
    public float rotationForce = 200f;

    public float chaseDistance = 6f;
    public float fleeDistance = 5f;

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
                //RotateTowardTarget();
                break;
            case MovementType.MoveAwayFromTarget:
                MoveAwayFromTarget();
                //RotateTowardTarget();
                break;
            case MovementType.PlanetMoveTowardsTarget:
                MoveTowardTarget();
                break;
            case MovementType.PlanetMoveAwayFromTarget:
                MoveTowardTarget();
                break;
            case MovementType.RotateTowardsTargetOnly:
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
        TargetUtilities.RotateSmoothlyTowardTarget(location, Owner.transform, rotationForce);

    }

    private Vector2 BasicMovement(Vector2 location)
    {
        Vector2 direction = location - (Vector2)transform.position;
        Vector2 moveForce = direction.normalized * verticalForce * Time.fixedDeltaTime;

        return moveForce;
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
       
        Vector2 wantedPosition = transform.right * 1f * horizontalForce * Time.fixedDeltaTime;
        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    public void MaintainDistance(float targetDistance)
    {
        if (targetDistance > chaseDistance)
        {
            ChangeMoveType(MovementType.MoveTowardTarget);
        }
        else if (targetDistance < fleeDistance)
        {
            ChangeMoveType(MovementType.MoveAwayFromTarget);
        }
        else if (targetDistance < chaseDistance && targetDistance > fleeDistance)
        {
            ChangeMoveType(MovementType.None);
        }
    }
}

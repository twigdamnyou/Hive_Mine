using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AISensor : MonoBehaviour
{
    //Owner - Who's sensor am I?
    //range - how big is my sensor area?
    //Current Target - what have I sensed?

    public Collider2D myCollider;
    //public Collider2D aggroCollider;
    public LayerMask detectionMask;


    public Entity LatestTarget { get; private set; }
    //public Entity PriorityTarget { get; private set; }


    private NPC owner;
    private List<Entity> targets = new List<Entity>();
    private AIBrain brain;

    public void Initialize(NPC owner, AIBrain brain)
    {
        this.owner = owner;
        this.brain = brain;

        if (myCollider is CircleCollider2D)
        {
            CircleCollider2D circleCollider2D = (CircleCollider2D)myCollider;
            circleCollider2D.radius = owner.detectionRange;
        }
        //myCollider.radius = owner.Stats[StatName.DetectionRange];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Entity detectedTarget = IsDetectionValid(other);

        if (detectedTarget == null)
            return;

        OnTargetDetected(detectedTarget);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Entity detectedTarget = IsDetectionValid(other);

        if (detectedTarget == null)
            return;

        OnDetectionLost(detectedTarget);
    }

    public float GetDistanceToTarget(Entity target = null)
    {
        if (target != null)
            return Vector2.Distance(owner.transform.position, target.transform.position);

        if (target == null && LatestTarget != null)
            return Vector2.Distance(owner.transform.position, LatestTarget.transform.position);

        return -1f;
    }

    private Entity IsDetectionValid(Collider2D other)
    {
        if (LayerTools.IsLayerInMask(detectionMask, other.gameObject.layer) == false)
            return null;

        Entity detectedTarget = other.gameObject.GetComponent<Entity>();

        return detectedTarget;
    }

    private void OnTargetDetected(Entity target)
    {
        LatestTarget = target;

        if (targets.Contains(target) == false)
            targets.Add(target);
        
        if (brain.WeaponManager.CurrentWeapon != null)
            brain.WeaponManager.CurrentWeapon.target = target;

        //Debug.Log("Target Detected: " + target.gameObject.name);
    }

    private void OnDetectionLost(Entity target)
    {
        if (targets.Contains(target) == true)
        {
            targets.Remove(target);
        }

        //Debug.Log("Target Lost: " + target.gameObject.name);

        if (LatestTarget == target)
        {
            if (targets.Count == 0)
            {
                LatestTarget = null;
                return;
            }

            LatestTarget = TargetUtilities.FindNearestTarget(targets, owner.transform);
        }
    }
}

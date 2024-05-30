using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TargetUtilities 
{
    public static Vector2 CreateRandomDirection(float min, float max)
    {
        float xNoise = Random.Range(min, max);
        float yNoise = Random.Range(min, max);

        Vector2 result = new Vector2(
            Mathf.Sin(2 * Mathf.PI * xNoise / 360),
            Mathf.Sin(2 * Mathf.PI * yNoise / 360));

        return result;
    }

    public static Quaternion GetRotationTowardTarget(Transform target, Transform myTransform)
    {
        return GetRotationTowardTarget(target.position, myTransform.position);
    }

    public static Quaternion GetRotationTowardTarget(Vector2 targetPostion, Vector2 myPosition)
    {
        Vector2 direction = targetPostion - myPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        return targetRotation;
    }

    public static Collider2D FindNearestTarget(Collider2D[] targets, Transform myTransform)
    {
        Collider2D result = null;

        Dictionary<Collider2D, float> distances = new Dictionary<Collider2D, float>();

        int count = targets.Length;
        for (int i = 0; i < count; i++)
        {
            float distance = Vector2.Distance(myTransform.position, targets[i].transform.position);
            distances.Add(targets[i], distance);
        }

        result = distances.OrderBy(d => d.Value).First().Key;

        return result;
    }

    public static Entity FindNearestTarget(List<Entity> targets, Transform myTransform)
    {
        Entity result = null;

        Dictionary<Entity, float> distances = new Dictionary<Entity, float>();

        int count = targets.Count;
        for (int i = 0; i < count; i++)
        {
            float distance = Vector2.Distance(myTransform.position, targets[i].transform.position);
            distances.Add(targets[i], distance);
        }

        result = distances.OrderBy(d => d.Value).First().Key;

        return result;
    }

    public static Vector2 FindNearestVector(List<Vector2> list, Vector2 origin)
    {
        if (list.Count == 0)
        {
            Debug.LogError("When trying to find nearest Vector there were no items in list");
            return origin;
        }
        Vector2 closestVector = Vector2.one * Mathf.Infinity;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < list.Count; i++)
        {
            float distance = Vector2.Distance(origin, list[i]);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestVector = list[i];
            }
        }

        return closestVector;
    }

    public static Entity FindNearestTarget(Collider2D initialTarget, Vector2 myPosition, float radius, LayerMask mask)
    {
        List<Collider2D> nearbyColliders = Physics2D.OverlapCircleAll(myPosition, radius, mask).ToList();

        if (nearbyColliders.Contains(initialTarget))
        {
            nearbyColliders.Remove(initialTarget);
        }

        Entity closest = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < nearbyColliders.Count; i++)
        {
            float distance = Vector2.Distance(myPosition, nearbyColliders[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = nearbyColliders[i].GetComponent<Entity>();
            }
        }

        return closest;
    }

    public static RaycastHit2D FindNearestTarget(RaycastHit2D[] targets, Transform myTransform)
    {
        RaycastHit2D result;

        Dictionary<RaycastHit2D, float> distances = new Dictionary<RaycastHit2D, float>();

        int count = targets.Length;
        for (int i = 0; i < count; i++)
        {
            float distance = Vector2.Distance(myTransform.position, targets[i].transform.position);
            distances.Add(targets[i], distance);
        }

        result = distances.OrderBy(d => d.Value).First().Key;

        return result;
    }

    public static void RotateSmoothlyTowardTarget(Transform target, Transform myTransform, float speed)
    {
        Quaternion targetRotation = GetRotationTowardTarget(target, myTransform);
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetRotation, speed * Time.fixedDeltaTime);
    }

    public static void RotateSnapTowardMouse(Transform transform)
    {
        Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.rotation = GetRotationTowardTarget(mousPos, transform.position);
    }

    public static void RotateSmoothlyTowardsMouse(Transform myTransform, float speed)
    {
        Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Quaternion targetRotation = GetRotationTowardTarget(mousPos, myTransform.position);

        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetRotation, speed * Time.fixedDeltaTime);
    }

    public static void RotateSmoothlyTowardTarget(Vector3 targetPos, Transform myTransform, float speed)
    {
        Quaternion targetRotation = GetRotationTowardTarget(targetPos, myTransform.position);
        myTransform.rotation = Quaternion.RotateTowards(myTransform.rotation, targetRotation, speed * Time.fixedDeltaTime);
    }

    public static void RotateSmoothlyTowardAngle(float angle, Transform myTransform, float speed)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        myTransform.rotation = targetRotation; //Quaternion.RotateTowards(myTransform.rotation, targetRotation, speed * Time.fixedDeltaTime);
    }

    public static Quaternion GetClampedRotation(Vector2 targetPosition, Transform myTransform, float speed, Vector2 clamp)
    {
        float rotZ = GetRotationAngleTowardTarget(targetPosition, myTransform.position);

        if (rotZ < -180f)
            rotZ = -rotZ;

        float clampedZ = Mathf.Clamp(rotZ, clamp.x, clamp.y);

        Quaternion toRotation = Quaternion.Euler(new Vector3(0f, 0f, clampedZ));

        return RotateToward(myTransform.rotation, toRotation, speed * Time.deltaTime);

    }

    public static Quaternion RotateToward(Quaternion from, Quaternion to, float maxDegreesDelta)
    {
        float num = Quaternion.Angle(from, to);

        if (num == 0f)
        {
            return to;
        }

        return Quaternion.SlerpUnclamped(from, to, Mathf.Min(1f, maxDegreesDelta / num));
    }

    public static void UnwindAngleDegrees(ref float angle)
    {
        if ((angle %= 360f) < 0f)
        {
            angle += 360f;
        }
    }

    public static float GetRotationAngleTowardTarget(Vector2 targetPosition, Vector2 myPosition)
    {
        Vector2 direction = targetPosition - myPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        return angle;
    }

    public static float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    public static bool IsTransformOnScreen(Transform transform)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        bool x = viewportPosition.x >= 0 && viewportPosition.x <= 1;
        bool y = viewportPosition.y >= 0 && viewportPosition.y <= 1;

        return x == true && y == true;
    }

    public static int GetRandomIndex(this List<Vector2> list)
    {
        return Random.Range(0, list.Count);
    }

    public static float CalculateAngleOfArc(float x, float y, float gravity, float force, bool preferSmallAng)
    {
        float innerSq = Mathf.Pow(force, 4) - gravity * (gravity * x * x + 2 * y * force * force);
        if (innerSq < 0)
        {
            return float.NaN;
        }
        float innerAtan;
        if (preferSmallAng)
        {
            innerAtan = (force * force - Mathf.Sqrt(innerSq)) / (gravity * x);
        }
        else
        {
            innerAtan = (force * force + Mathf.Sqrt(innerSq)) / (gravity * x);
        }
        float res = Mathf.Atan(innerAtan) * Mathf.Rad2Deg;
        return res;
    }


    /// This is a 2D version of Quaternion.LookAt; it returns a quaternion
	/// that makes the local +X axis point in the given forward direction.
	/// 
	/// forward direction
	/// Quaternion that rotates +X to align with forward
    public static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}

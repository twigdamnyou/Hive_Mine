using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public float range = -1f;
    public LayerMask mask;

    public bool Hit { get; private set; }
    public Transform origin;

    private void Awake()
    {
        if (origin == null)
        {
            Debug.LogError("No Origin set on Line of Sight Component for:" + this.tag + "Setting self as default");
            origin = transform;
        }
    }

    private void Update()
    {
        float actualRange = range < 0f ? Mathf.Infinity : range;

        RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.up, actualRange, mask);

        Hit = hit.collider != null;

        if (hit.collider != null)
        {
            //Debug.LogWarning(hit.collider.gameObject.name + " was hit");
        }
        else
        {
            //Debug.LogError("Hit nothing");
        }
    }
}

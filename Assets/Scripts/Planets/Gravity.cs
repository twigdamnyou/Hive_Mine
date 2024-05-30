using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public Collider2D gravityCollider;

    public List<Transform> targets = new List<Transform>();
    public Planet planet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Ground Enemy")
        {
            if (targets.Contains(collision.transform) == false)
                targets.Add(collision.transform);

            if (collision.tag == "Player")
            {
                GameManager.currentPlanet = planet;

                EventData eventData = new EventData();
                eventData.AddPlanet("Planet", planet);
                EventManager.SendEvent(EventManager.GameEvent.PlayerEnteredAtmosphere, eventData);
            }

            //Debug.Log("Detected Entering: " + collision.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Ground Enemy")
        {
            if (targets.Contains(collision.transform))
                targets.Remove(collision.transform);

            if (collision.tag == "Player")
            {
                GameManager.currentPlanet = null;

                EventData eventData = new EventData();
                eventData.AddPlanet("Planet", planet);
                EventManager.SendEvent(EventManager.GameEvent.PlayerLeftAtmosphere, eventData);
            }


            //Debug.Log("Detected Exiting: " + collision.gameObject.name);
        }

    }

    private void FixedUpdate()
    {
        if (targets.Count == 0)
            return;

        foreach (Transform currentTarget in targets)
        {
            RotateAwayFromCenter(currentTarget);
        }
    }

    private void RotateAwayFromCenter(Transform target)
    {
        //Debug.Log("Starting Rotation: " + target.gameObject.name);
        Vector2 dir = target.position - transform.position;

        //gives us the angle we want to rotate too
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        //rotate the specified angle aroun the specified axis
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        Quaternion newRotation = Quaternion.Slerp(target.rotation, rot, Time.fixedDeltaTime * rotationSpeed);
        target.rotation = newRotation;
    }

}

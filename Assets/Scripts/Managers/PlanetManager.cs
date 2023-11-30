using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    public static List<Planet> activePlanetList = new List<Planet>();

    public static void RegisterPlanet(Planet planet)
    {
        if (activePlanetList.Contains(planet) == false)
        {
            activePlanetList.Add(planet);
        }
    }

    public static void UnRegisterPlanet(Planet planet)
    {
        if (activePlanetList.Contains(planet) == true)
        {
            activePlanetList.Remove(planet);
        }
    }

    public static void RemoveGravityInteration(Collider2D target)
    {
        foreach (Planet planet in activePlanetList)
        {
            Physics2D.IgnoreCollision(planet.gravity.gravityCollider, target, true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlanetDatabase;

public class PlanetManager : MonoBehaviour
{
    public static List<Planet> activePlanetList = new List<Planet>();
    
    public int rejectionSamples = 30;
    public float bufferAroundPlanets = 10f;
    public int planetMinNum = 5;
    public LayerMask overlapMask;

    private static Dictionary<Vector2, float> validSpawnPoints = new Dictionary<Vector2, float>();

    public PlanetDatabase planetDatabase;
    public static PlanetManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PopulatePlanetList()
    {
        //clear Lists
        activePlanetList.Clear();


    }

    public void SpawnTest()
    {
        //Gizmos.DrawWireCube(GameManager.instance.regionSize / 2, GameManager.instance.regionSize);
        //Debug.Log("attempting to spawn planets");
        instance.CheckSpawnPoint();

        if (validSpawnPoints != null)
        {
            foreach (KeyValuePair<Vector2, float> validSpawns in validSpawnPoints)
            {
                Planet targetSpawn = planetDatabase.GetPlanetBySize(validSpawns.Value);
                Instantiate(targetSpawn, validSpawns.Key, Quaternion.identity);
            }
        }
    }

    public void CheckSpawnPoint()
    {
        //float radius = 80f;
        
        validSpawnPoints = PoissonDiscSampling.GeneratePoints(planetDatabase.GetAllPlanetSizes(), GameManager.instance.regionSize, rejectionSamples);
    }

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

    //TODO: see if joels spawn system is better or fix my own
    public void SpawnPlanets()
    {
        for (int i = 0; i < rejectionSamples; ++i)
        {
            PlanetData targetSpawn = planetDatabase.GetRandomPlanet();
            float spawnSize = targetSpawn.planet.GetAtmosphereSize();

            Planet randomStartPlanet = activePlanetList.Count == 0 ? null : activePlanetList[Random.Range(0, activePlanetList.Count)];
            Vector2 startingPosition = randomStartPlanet == null ? Vector2.zero : randomStartPlanet.transform.position;
            float randomBuffer = Random.Range(bufferAroundPlanets, bufferAroundPlanets * 2);

            Vector2 randomizedPos = startingPosition + Random.insideUnitCircle * randomBuffer;
            Debug.Log("Starting Pos is: " + startingPosition);
            Debug.Log("randomized Pos is: " + randomizedPos);
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(randomizedPos, spawnSize + bufferAroundPlanets/6, overlapMask);

            if (overlaps.Length > 0)
            {
                Debug.Log(overlaps[0].gameObject.name);
                Debug.Log("too close");
                continue;
            }

            Planet activeSpawn = Instantiate(targetSpawn.planet, randomizedPos, Quaternion.identity);
            activePlanetList.Add(activeSpawn);
        }

        //if (activePlanetList.Count < planetMinNum)
        //{
        //    SpawnPlanets();
        //}
    }
}

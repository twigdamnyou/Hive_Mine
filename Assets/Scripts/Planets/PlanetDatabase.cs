using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Planet Database")]
public class PlanetDatabase : ScriptableObject
{
    public enum PlanetSize
    {
        Small,
        Medium,
        Large,
    }

    public List<PlanetData> planetData = new List<PlanetData>();

    public List<PlanetData> GetPlanetsBySize(PlanetSize size)
    {
        List<PlanetData> validPlanets = new List<PlanetData>();

        for (int i = 0; i < planetData.Count; i++)
        {
            if (planetData[i].planetSize == size)
            {
                validPlanets.Add(planetData[i]);
            }
        }

        return validPlanets;
    }

    public Planet GetPlanetBySize(float size)
    {
        for (int i = 0; i < planetData.Count; i++)
        {
            float atmoSize = planetData[i].planet.GetAtmosphereSize();

            if (atmoSize == size)
            {
                return planetData[i].planet;
            }
        }

        Debug.Log("No Planets found that match size given");
        return null;
    }

    public List<PlanetData> GetPlanetsByMineCount(float mineCount)
    {
        if (mineCount < 1)
        {
            Debug.LogError("Tried to find a Mine count less then 1");
            return null;
        }

        List<PlanetData> validPlanets = new List<PlanetData>();

        for (int i = 0; i < planetData.Count; i++)
        {
            if (planetData[i].mineCount == mineCount)
            {
                validPlanets.Add(planetData[i]);
            }
        }

        if (validPlanets.Count == 0)
        {
            Debug.LogWarning("No Planets at Mine count: " + mineCount + " found, findning closest match");
            GetPlanetsByMineCount(mineCount - 1);
        }
        return validPlanets;
    }

    public List<PlanetData> GetPlanets(PlanetSize type, float mineCount = 1f)
    {
        List<PlanetData> validPlanets = new List<PlanetData>();

        for (int i = 0; i < planetData.Count; i++)
        {
            if (planetData[i].planetSize == type && planetData[i].mineCount == mineCount)
            {
                validPlanets.Add(planetData[i]);
            }
        }

        return validPlanets;
    }

    public List<float> GetAllPlanetSizes()
    {
        List<float> results = new List<float>();

        for (int i = 0; i < planetData.Count; i++)
        {
            float planetSize = planetData[i].planet.GetAtmosphereSize();
            results.Add(planetSize);
        }

        return results;
    }

    public List<float> GetAllPlanetRadii()
    {
        List<float> results = new List<float>();

        for (int i = 0; i < planetData.Count; i++)
        {
            float radii = planetData[i].planet.radius;
            results.Add(radii);
        }

        return results;
    }

    public PlanetData GetRandomPlanet()
    {
        int randomNum = Random.Range(0, planetData.Count);
        return planetData[randomNum];
    }

    [System.Serializable]
    public class PlanetData
    {
        public Planet planet;
        public PlanetSize planetSize;
        //public float radius;
        //public Vector2 pos;
        public float mineCount;
        //public float minimumSpawnDistance;
    }
}

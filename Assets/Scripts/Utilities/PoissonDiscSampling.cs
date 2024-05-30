using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PoissonDiscSampling
{
    public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
    {
        // cell size bounded by r/?2 so each grid cell will contain at most 1 sample,
        // thus the grid can be implemented as a simple 2D array of integers:
        //  -- the default ?1 indicates no sample,
        //  -- a non-negative integer gives the index of the sample located in a cell.
        float cellSize = radius / Mathf.Sqrt(2);

        // Step 0 :
        // Initialize an 2d background grid for storing
        // samples and accelerating spatial searches
        int[,] grid = new int[
            Mathf.CeilToInt(sampleRegionSize.x / cellSize), 
            Mathf.CeilToInt(sampleRegionSize.y / cellSize)
            ];

        List<Vector2> points = new List<Vector2>(); //point in grid
        List<Vector2> spawnPoints = new List<Vector2>();

        // Step 1:
        // Select the initial sample, x0, randomly chosen uniformly
        // from the domain. Insert it into the background grid, and initialize
        // the “active list” (an array of sample indices) with this index (zero).
        spawnPoints.Add(sampleRegionSize / 2);

        while (spawnPoints.Count > 0) //keep iterating over spawnPts
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            //check for spawn point
            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                //get a random vector from current to new point
                float angle = Random.value * Mathf.PI * 2;
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));

                Vector2 candidate = spawnCenter + direction * Random.Range(radius, 2 * radius);

                if (IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }

            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    public static Dictionary<Vector2, float> GeneratePoints(List<float> radii, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
    {
        List<Point> points = new List<Point>();

        float minRadius = radii.Min();
        float maxRadius = radii.Max();

        float cellSize = minRadius / Mathf.Sqrt(2);

        Dictionary<Vector2Int, Point> grid = new();
        List<Point> spawnPoints = new()
        {
            new() { pos = sampleRegionSize / 2f }
        };

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Point spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new(Mathf.Sin(angle), Mathf.Cos(angle));

                Point point = new()
                {
                    pos = spawnCentre.pos + dir * Random.Range(minRadius * 2, maxRadius * 4),
                    radius = radii[Random.Range(0, radii.Count)]
                };

                if (IsValid(point, sampleRegionSize, cellSize, maxRadius, grid))
                {
                    Vector2Int gridKey = new((int)(point.pos.x / cellSize), (int)(point.pos.y / cellSize));
                    grid[gridKey] = point;
                    points.Add(point);
                    spawnPoints.Add(point);
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            } 
        }

        //convert the points over to a vector2
        Dictionary<Vector2, float> varRadiiSpwnPnts = new Dictionary<Vector2, float>();
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 pointPos = points[i].pos;
            float radius = points[i].radius;
            varRadiiSpwnPnts.Add(pointPos, radius);
        }

        return varRadiiSpwnPnts;
    }

    /// <summary>
    /// Check if candidate is a valid pt
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="sampleRegionSize"></param>
    /// <param name="cellSize"></param>
    /// <param name="radius"></param>
    /// <param name="points"></param>
    /// <param name="grid"></param>
    /// <param name="circularRegion"></param>
    /// <returns></returns>
    private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int cellx = (int)(candidate.x / cellSize);
            int celly = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellx - 2);
            int searchEndX = Mathf.Min(cellx + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, celly - 2);
            int searchEndY = Mathf.Min(celly + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        return false;
    }

    private static bool IsValid(Point candidate, Vector2 sampleRegionSize, float cellSize, float maxRadius, Dictionary<Vector2Int, Point> grid)
    {
        if (candidate.pos.x >= 0 && candidate.pos.x < sampleRegionSize.x && candidate.pos.y >= 0 && candidate.pos.y < sampleRegionSize.y)
        {
            int cellX = (int)(candidate.pos.x / cellSize);
            int cellY = (int)(candidate.pos.y / cellSize);
            int maxDiameterCells = 2 * Mathf.CeilToInt(maxRadius / cellSize);
            int searchStartX = cellX - maxDiameterCells;
            int searchEndX = cellX + maxDiameterCells;
            int searchStartY = cellY - maxDiameterCells;
            int searchEndY = cellY + maxDiameterCells;

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    Vector2Int gridKey = new(x, y);
                    if (grid.TryGetValue(gridKey, out Point other))
                    {
                        if (Mathf.Pow(candidate.radius + other.radius, 2) > (candidate.pos - other.pos).sqrMagnitude)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

}

public struct Point
{
    public Vector2 pos;
    public float radius;
}


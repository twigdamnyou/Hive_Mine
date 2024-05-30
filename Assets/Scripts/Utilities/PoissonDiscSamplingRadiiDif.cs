using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscSamplingRadiiDif : MonoBehaviour {
    
    [SerializeField] private float minRadius = 1f;
    [SerializeField] private float maxRadius = 2f;
    [SerializeField] private Vector2 regionSize = Vector2.one;
    [SerializeField] private int rejectionSamples = 30;
    [SerializeField] private int seed = 30;

    private List<Points> points;


    //OnValidate only runs in editor and will not fire during runtime
    private void OnValidate()
    {
        if (minRadius <= 0.001f)
            minRadius = 0.001f;
        if (minRadius > maxRadius)
            maxRadius = minRadius + 0.001f;

        if (rejectionSamples > 50)
            rejectionSamples = 50;

        Random.InitState(seed);

        points = new();
        GeneratePoints(ref points, minRadius, maxRadius, regionSize, rejectionSamples);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(regionSize / 2f, regionSize);
        if (points != null)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Gizmos.DrawSphere(points[i].pos, points[i].radius);
            }
        }
    }

    public void GeneratePoints(ref List<Points> points, float minRadius, float maxRadius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30)
    {
        float cellSize = minRadius / Mathf.Sqrt(2);

        Dictionary<Vector2Int, Points> grid = new();
        List<Points> spawnPoints = new()
        {
            new() { pos = sampleRegionSize / 2f }
        };

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Points spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new(Mathf.Sin(angle), Mathf.Cos(angle));
                
                Points point = new()
                {
                    pos = spawnCentre.pos + dir * Random.Range(minRadius * 2, maxRadius * 4),
                    radius = Random.Range(minRadius, maxRadius)
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
    }

    bool IsValid(Points candidate, Vector2 sampleRegionSize, float cellSize, float maxRadius, Dictionary<Vector2Int, Points> grid)
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
                    if (grid.TryGetValue(gridKey, out Points other))
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

public struct Points
{
    public Vector2 pos;
    public float radius;
}


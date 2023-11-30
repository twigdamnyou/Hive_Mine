using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static HashSet<Vector2Int> currrentWallPos;

    public static void CreateWalls(HashSet<Vector2Int> floorPositions, DungeonGenerator tileMapVisualizer)
    {
        List<Vector2Int> directions = new List<Vector2Int>();
        directions.AddRange(Direction2D.cardinalDirectionsList);
        directions.AddRange(Direction2D.diagonalDirection);

        HashSet<Vector2Int> basicWallPosition = FindWallsInDirections(floorPositions, directions);
        currrentWallPos = basicWallPosition;
        
        foreach (var position in basicWallPosition)
        {
            tileMapVisualizer.PaintSingleBasicWall(position);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            foreach (Vector2Int direction in directionList)
            {
                Vector2Int neighborPosition = pos + direction;
                if (floorPositions.Contains(neighborPosition) == false)
                {
                    wallPositions.Add(neighborPosition);
                }
            }
        }

        return wallPositions;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    private Dictionary<Vector2, CustomTile> tiles;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity);
                //spawnedTile.name = $"Tile {x} {y}";

                //tiles[new Vector2 (x, y)] = spawnedTile;
            }
        }
    }

    public CustomTile GetTileAtPosition(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }


}

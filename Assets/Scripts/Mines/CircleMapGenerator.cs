using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircleMapGenerator : MonoBehaviour
{
    public TileBase coreTile;
    public TileBase deepTile;
    public TileBase middleTile;
    public TileBase upperTile;
    public TileBase surfaceTile;

    public int mapRadius = 10;
    public Tilemap tilemap;

    private void Start()
    {
        for (int x = -mapRadius; x <= mapRadius; x++)
        {
            for (int y = -mapRadius; y <= mapRadius; y++)
            {
                if (Mathf.Sqrt(x * x + y * y) <= mapRadius)
                {
                    // Generate a tile at this position.
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    TileBase tile = GenerateTile(tilePos);
                    tilemap.SetTile(tilePos, tile);
                }
            }
        }
    }

    private TileBase GenerateTile(Vector3Int position)
    {
        // Generate and return a TileBase object for this position.
        // This method should return a different tile based on the position.
        // For example, you could use a noise function to generate different terrain types.

        float distance = Mathf.Sqrt(position.x * position.x + position.y * position.y);



        if (distance < mapRadius * 0.4f && distance > mapRadius * 0.2f)
        {
            return middleTile;
        }

        if (distance < mapRadius * 0.2f && distance > mapRadius * 0.1f)
        {
            return deepTile;
        }

        if (distance <= mapRadius * 0.1f)
        {
            return coreTile;
        }

        return null;
    }
}

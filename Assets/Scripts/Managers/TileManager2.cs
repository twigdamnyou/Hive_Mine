using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TileManager2 : MonoBehaviour
{
    public Dictionary<Vector2Int, RunTimeTile> runtimeTileDic = new Dictionary<Vector2Int, RunTimeTile>();

    public static TileManager2 instance;

    private void Awake()
    {
        instance = this;
    }

    public RunTimeTile GetRuntimeTileAtLocation(Vector2Int location, AdvancedRuleTile tileData)
    {
        if (runtimeTileDic.TryGetValue(location, out RunTimeTile targetTile) == true)
        {
            return targetTile;
        }
        else
        {
            RunTimeTile newTile = new RunTimeTile(location, tileData);
            runtimeTileDic.Add(location, newTile);
            return newTile;
        }
    }


}

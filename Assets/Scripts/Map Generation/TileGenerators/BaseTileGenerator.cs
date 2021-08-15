using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileGenerator : MonoBehaviour {
    public TileType tileType;
    public bool IsEnabled = true;
    public int seed;

    public List<TileType> blockingTileTypes;

    public virtual bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        //Debug.Log($"TileGenerator for {tileType}");
        return new bool[tileTypeMap.size.x, tileTypeMap.size.y];
    }
}

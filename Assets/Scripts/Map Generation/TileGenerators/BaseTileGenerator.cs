using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTileGenerator : MonoBehaviour {
    public TileType tileType;
    public bool IsEnabled = true;
    public int seed;

    public List<TileType> blockingTileTypes;

    public abstract bool[,] GenerateTiles(TileTypeMap tileTypeMap);
}

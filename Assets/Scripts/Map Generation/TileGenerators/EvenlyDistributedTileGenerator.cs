using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses the randomized grid algorithm to place evently distributed tiles.
/// See for algorithm: https://github.com/JamesTheButler/PointDistribution/tree/master/src/Algorithms
/// </summary>
class EvenlyDistributedTileGenerator : BaseTileGenerator {
    public int tileCount;
    public bool useSegmentation;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = base.GenerateTiles(tileTypeMap);

        if (!IsEnabled) return thisLayer;

        GenerateTiles(tileTypeMap, tileCount, tileTypeMap.size, ref thisLayer);

        return thisLayer;
    }

    private bool[,] GenerateTiles(TileTypeMap tileTypeMap, int tileCount, Vector2Int tileMapSize, ref bool[,] thisLayer) {
        if (tileCount == 0) return thisLayer;

        List<Rect> quadrants = GetQuadrants(tileMapSize, tileCount);
        for (int i = 0; i < tileCount; i++) {
            Vector2Int tilePos;
            if (useSegmentation) {
                var quadrantMinPosition = quadrants[i].position;
                var quadrantMaxPosition = quadrants[i].position + quadrants[i].size;
                tilePos = new Vector2Int((int)Random.Range(quadrantMinPosition.x, quadrantMaxPosition.x),
                                         (int)Random.Range(quadrantMinPosition.y, quadrantMaxPosition.y));
            } else {
                tilePos = new Vector2Int(Random.Range(0, tileMapSize.x), Random.Range(0, tileMapSize.y));
            }

            // avoid positions with blocking tiles and decrement i 
            if (tileTypeMap.HasAnyTileOnLayers(tilePos.x, tilePos.y, blockingTileTypes)) {
                i--;
            } else {
                thisLayer[tilePos.x, tilePos.y] = true;
            }
        }
        return thisLayer;
    }

    private int FindBestDivider(int number) {
        List<int> dividers = new List<int>();
        for (int i = 1; i <= number; i++) {
            if (number % i == 0) {
                dividers.Add((int)(number / i));
            }
        }
        return dividers[dividers.Count / 2];
    }

    private List<Rect> GetQuadrants(Vector2Int dimensions, int townCount) {
        List<Rect> rects = new List<Rect>();
        int columnCount = FindBestDivider(townCount);
        int rowCount = townCount / columnCount;
        Vector2 rectSize = new Vector2(dimensions.x / columnCount, dimensions.y / rowCount);

        for (int i = 0; i < columnCount; i++) {
            for (int j = 0; j < rowCount; j++) {
                rects.Add(new Rect(i * rectSize[0], j * rectSize[1], rectSize[0], rectSize[1]));
            }
        }
        return rects;
    }
}

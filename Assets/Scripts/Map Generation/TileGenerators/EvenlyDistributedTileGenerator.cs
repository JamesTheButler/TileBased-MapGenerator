using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class EvenlyDistributedTileGenerator : BaseTileGenerator {
    public int tileCount;
    public bool useSegmentation;
    public int quadrantBlockedLimit;

    public override void GenerateTiles(Tilemap tilemap) {
        if (!IsEnabled)
            return;
        base.GenerateTiles(tilemap);

        GenerateTiles(tilemap, tileCount);
        tileIndexer.Index(tilemap, flagMap, layerHeight);
    }

    private void GenerateTiles(Tilemap tilemap, int townCount) {
        if (townCount == 0)
            return;
        flagMap = new bool[tileMapSize.x, tileMapSize.y];
        List<Rect> quadrants = GetQuadrants(tileMapSize, townCount);
        for (int i = 0; i < townCount; i++) {
            int isQuadrantBlockedCounter = 0;
            Vector2Int tilePos;
            if (useSegmentation) {
                tilePos = new Vector2Int(UnityEngine.Random.Range((int)quadrants[i].position.x, (int)quadrants[i].position.x + (int)quadrants[i].size.x),
                                         UnityEngine.Random.Range((int)quadrants[i].position.y, (int)quadrants[i].position.y + (int)quadrants[i].size.y));
            } else {
                tilePos = new Vector2Int(UnityEngine.Random.Range(0, tileMapSize.x), UnityEngine.Random.Range(0, tileMapSize.y));
            }

            // avoid positions with blocking tiles
            if (tilemap.HasAnyTileOnLayers(tilePos.x, tilePos.y, ignoredLayers)) {
                //skip quadrant if it is fully blocked
                if (isQuadrantBlockedCounter <= quadrantBlockedLimit) {
                    i--;
                    isQuadrantBlockedCounter++;
                }
            } else {
                flagMap[tilePos.x, tilePos.y] = true;
            }
        }
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

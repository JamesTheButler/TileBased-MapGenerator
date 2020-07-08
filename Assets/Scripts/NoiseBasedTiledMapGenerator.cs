using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/* **** THIS IS WHERE I LEFT OF YESTERDAY ****
 * RECENT CHANGES
 * - rivers are implemented and have semi-random flow. Needs some work for better curves
 * TODO:s
 * - adjust random curves of rivers
 * - features can get rendered below terrain (?)
 * - Rivers flowing next to each other have tiling issues (have list of rivers and tile them individually)
 * - need end and start tiles.
 */
public enum TileType {
    NONE,
    GRASS,
    FOREST,
    MOUNTAIN,
    WATER,
    TOWN,
    RIVER,
    ROAD,
}


public class NoiseBasedTiledMapGenerator : MonoBehaviour {
    public Tilemap map;

    public TileSet grassTiles;
    public TileSet mountainTiles;
    public TileSet waterTiles;
    public TileSet forestTiles;

    public TileSet roadTiles;
    public TileSet riverTiles;
    public TileSet townTiles;

    public float terrainNoiseScale;
    public float forestNoiseScale;
    public int cleanUpIterations;
    public int cleanUpCutOffNumber;

    public CameraMovement camMover;
    public Vector2Int dimensions;

    public int riverCount;
    public int townCount;

    public bool useSeed = false;
    public int seed = 0;
    public bool useSegmentation;

    public bool profile;

    private int baseDepth = 0;
    private int terrainDepth = 1;
    private int featureDepth = 20;

    private float[,] terrainNoise;
    private float[,] forestNoise;

    private Dictionary<Vector2Int, TileType> terrainTileTypeMap;
    private Dictionary<Vector2Int, TileType> featureTileTypeMap;

    private Dictionary<TileType, TileSet> tileSets;

    private List<Vector2Int> mountainTileList;
    private List<Vector2Int> waterTileList;
    private List<Vector2Int> riverTileList;

    public GameObject noiseQuad;
    public GameObject riverQuad;


    private void Awake() {
        terrainTileTypeMap = new Dictionary<Vector2Int, TileType>();
        featureTileTypeMap = new Dictionary<Vector2Int, TileType>();
        tileSets = new Dictionary<TileType, TileSet>();
        mountainTileList = new List<Vector2Int>();
        waterTileList = new List<Vector2Int>();
        riverTileList = new List<Vector2Int>();
    }

    private void Start() {
        P.setState(profile);

        // init
        InitTiles();
        // make noise
        if (!useSeed)
            GenerateSeed();
        UnityEngine.Random.InitState(seed);

        Debug.Log("Map Seed: " + seed);

        GenerateNoise(seed);

        RenderNoise(terrainNoise);

        GenerateMap(seed);
        // cleanup pass
        CleanUpTerrain(cleanUpIterations);
        // generate rivers
        GenerateRivers(seed, dimensions);
        RenderRivers(riverTileList);
        // generate town positions
        GenerateTowns(seed, dimensions, townCount/*, minTownDistance*/);
        // set tiles
        SetTiles();
        // set up cam 
        camMover.setMap(dimensions);
    }

    private void RenderNoise(float[,] noise) {
        Texture2D texture = new Texture2D(dimensions.x, dimensions.y, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                texture.SetPixel(x, y, ColorFromGreySacel(noise[x, y]));
            }
        }
        texture.Apply();
        noiseQuad.transform.position = new Vector3(dimensions.x / 2, dimensions.y / 2, -5);
        noiseQuad.transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
        noiseQuad.GetComponent<Renderer>().material.mainTexture = texture;
    }

    private void GenerateNoise(int seed) {
        terrainNoise = new float[dimensions.x, dimensions.y];
        forestNoise = new float[dimensions.x, dimensions.y];
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                terrainNoise[x, y] = Mathf.PerlinNoise(x / terrainNoiseScale + seed, y / terrainNoiseScale + seed);
                forestNoise[x, y] = Mathf.PerlinNoise(x / forestNoiseScale + seed * seed, y / forestNoiseScale + seed * seed);
            }
        }
    }

    private Color ColorFromGreySacel(float v) {
        return new Color(v, v, v, 1f);
    }


    private void GenerateSeed() {
        seed = UnityEngine.Random.Range(0, 10000);
    }

    protected void InitTiles() {
        // setup tile indexers
        grassTiles.init(new RandomTileIndexer());
        waterTiles.init(new TerrainTileIndexer());
        mountainTiles.init(new TerrainTileIndexer());
        forestTiles.init(new RandomTileIndexer());
        townTiles.init(new TileIndexer());
        roadTiles.init(new PathingTileIndexer());
        riverTiles.init(new PathingTileIndexer());
        //add sets to look up table
        tileSets.Add(TileType.GRASS, grassTiles);
        tileSets.Add(TileType.FOREST, forestTiles);
        tileSets.Add(TileType.MOUNTAIN, mountainTiles);
        tileSets.Add(TileType.WATER, waterTiles);
        tileSets.Add(TileType.TOWN, townTiles);
        tileSets.Add(TileType.RIVER, riverTiles);
        tileSets.Add(TileType.ROAD, roadTiles);
    }

    private void GenerateMap(int seed) {
        Debug.Log("Generate Noise.");
        P.start("Generate Noise");

        int forestOffset = seed;
        int terrainOffset = seed;
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                // terrainTileTypeMap.Add(key, TileType.GRASS);

                float terrain_noise = terrainNoise[x, y];
                if (terrain_noise < waterTiles.heightLevel) {
                    terrainTileTypeMap[pos] = TileType.WATER;
                    waterTileList.Add(pos);
                } else if (terrain_noise > mountainTiles.heightLevel) {
                    terrainTileTypeMap[pos] = TileType.MOUNTAIN;
                    mountainTileList.Add(pos);
                } else {
                    float forest_noise = forestNoise[x, y];
                    if (forest_noise > forestTiles.heightLevel)
                        terrainTileTypeMap[pos] = TileType.FOREST;
                }
            }
        }
        P.end();
    }

    private bool IsDiagonal(Vector2Int v) { return v.x != 0 && v.y != 0; }

    private void RenderRivers(List<Vector2Int> riverPoints) {
        Texture2D texture = new Texture2D(dimensions.x, dimensions.y, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        // create clear texture
        Color[] colors = new Color[dimensions.x * dimensions.y];
        for (int i = 0; i < colors.Length; i++) { colors[i] = Color.clear; }
        texture.SetPixels(0, 0, dimensions.x, dimensions.y, colors);
        //draw river pixels
        foreach (Vector2Int point in riverPoints) {
            texture.SetPixel(point.x, point.y, Color.red);
        }

        texture.Apply();
        riverQuad.transform.position = new Vector3(dimensions.x / 2, dimensions.y / 2, -6);
        riverQuad.transform.localScale = new Vector3(dimensions.x, dimensions.y, 1);
        riverQuad.GetComponent<Renderer>().material.mainTexture = texture;
    }


    private void GenerateRivers(int seed, Vector2Int dimensions) {
        P.start("generateRivers");
        Debug.Log("generateRivers");

        for (int i = 0; i < riverCount; i++) {
            Vector2Int pos = mountainTileList[UnityEngine.Random.Range(0, mountainTileList.Count)];
            Debug.Log("mountain tile: " + pos);
            riverTileList.Add(pos);

            bool hasReachedEnd = false;
            Vector2Int currentPos = pos;
            List<Vector2Int> visited = new List<Vector2Int>();
            float currentHeight = terrainNoise[pos.x, pos.y];

            // define positions the river checks
            List<Vector2Int> kernel = new List<Vector2Int>();
            kernel.Add(Vector2Int.up + Vector2Int.left);
            kernel.Add(Vector2Int.up);
            kernel.Add(Vector2Int.up + Vector2Int.right);
            kernel.Add(Vector2Int.left);
            kernel.Add(Vector2Int.right);
            kernel.Add(Vector2Int.down + Vector2Int.left);
            kernel.Add(Vector2Int.down);
            kernel.Add(Vector2Int.down + Vector2Int.right);

            while (!hasReachedEnd) {
                if (!IsOnMap(currentPos)) break;
                //end loop. loop is continued if at least one next step is viable
                hasReachedEnd = true;

                visited.Add(currentPos);
                riverTileList.Add(currentPos);

                float minHeight = terrainNoise[currentPos.x, currentPos.y];

                Debug.Log("curr pos " + currentPos + ", " + minHeight);
                Vector2Int minPos = new Vector2Int();
                Vector2Int nextStep = new Vector2Int(); ;

                foreach (Vector2Int direction in kernel) {
                    nextStep = currentPos + direction;
                    //skip positions the river was previously on
                    if (visited.Contains(nextStep)) continue;
                    //skip positions that are outside the map
                    if (!IsOnMap(nextStep)) continue;
                    // if height ofd next pos is lower than the current pos, set it as prelimiary minimum
                    if (terrainNoise[nextStep.x, nextStep.y] < minHeight) {
                        Debug.Log("possible steps.." + nextStep + ", " + terrainNoise[nextStep.x, nextStep.y]);
                        minHeight = terrainNoise[nextStep.x, nextStep.y];
                        minPos = nextStep;
                        hasReachedEnd = false;
                    }
                }
                if (!hasReachedEnd) {
                    currentPos = minPos;
                    Debug.Log("Next Step goes to " + currentPos);
                }
            }

            //  featureTileTypeMap[pos] = TileType.RIVER;
            //  bool hasReachedEnd = false;
            //  Vector2Int direction = new Vector2Int(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
            //  //Vector2Int direction = new Vector2Int(-1, 1);
            // // Debug.Log("general direction " + direction);
            //
            //  while (!hasReachedEnd) {
            //      Vector2Int tempDir = direction;
            //      if (IsDiagonal(tempDir)) {
            //         /* if (UnityEngine.Random.value > 0.5) {
            //              if (UnityEngine.Random.value > 0.5)
            //                  direction.x = 0;
            //          } else {
            //              if (UnityEngine.Random.value > 0.5)
            //                  direction.y = 0;
            //          }*/
            //      } else {
            //          tempDir.x = UnityEngine.Random.Range(-1, 1);
            //      }
            //    //  Debug.Log("tile direction " + tempDir);
            //      // direction.y = UnityEngine.Random.Range(-1, 1);
            //      pos += tempDir;
            //      //fix curves (and connect corners for proper tiling)
            //      if (IsDiagonal(tempDir)) { 
            //          if (UnityEngine.Random.value > 0.5)
            //              featureTileTypeMap[pos - new Vector2Int(tempDir.x, 0)] = TileType.RIVER;
            //          else
            //              featureTileTypeMap[pos - new Vector2Int(0, tempDir.y)] = TileType.RIVER;
            //      }
            //      featureTileTypeMap[pos] = TileType.RIVER;
            //      hasReachedEnd = IsRiverEnd(pos);
            //  }
        }
        P.end();
    }

    private bool IsRiverEnd(Vector2Int pos) {
        if (!IsOnMap(pos))
            return true;
        if (terrainTileTypeMap.ContainsKey(pos) && terrainTileTypeMap[pos] == TileType.WATER)
            return true;
        return false;

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

    //TODO: set size town tile count
    private void GenerateTowns(int seed, Vector2Int dimensions, int townCount/* int townSize*/) {
        if (townCount == 0)
            return;

        List<Rect> quadrants = GetQuadrants(dimensions, townCount);
        for (int i = 0; i < townCount; i++) {
            Vector2Int townPos;
            if (useSegmentation) {
                townPos = new Vector2Int(UnityEngine.Random.Range((int)quadrants[i].position.x, (int)quadrants[i].position.x + (int)quadrants[i].size.x),
                                         UnityEngine.Random.Range((int)quadrants[i].position.y, (int)quadrants[i].position.y + (int)quadrants[i].size.y));
            } else {
                townPos = new Vector2Int(UnityEngine.Random.Range(0, dimensions.x), UnityEngine.Random.Range(0, dimensions.y));
                Debug.Log(townPos);
            }
            // set tile
            if (featureTileTypeMap.ContainsKey(townPos))
                featureTileTypeMap[townPos] = TileType.TOWN;
            else
                featureTileTypeMap.Add(townPos, TileType.TOWN);
            /*Vector2Int hori = Vector2Int.right;
            Vector2Int vert = Vector2Int.down;

            // TODO: can be circumvented by only allowing positions of dimensions - town size 
            if (townPos.x == dimensions.x)
                hori = Vector2Int.left;
            if (townPos.y == dimensions.y)
                vert = Vector2Int.up;

            if (featureTileTypeMap.ContainsKey(townPos))
                featureTileTypeMap[townPos + hori] = TileType.TOWN;
            else
                featureTileTypeMap.Add(townPos + hori, TileType.TOWN);

            if (featureTileTypeMap.ContainsKey(townPos))
                featureTileTypeMap[townPos + vert] = TileType.TOWN;
            else
                featureTileTypeMap.Add(townPos + vert, TileType.TOWN);

            if (featureTileTypeMap.ContainsKey(townPos))
                featureTileTypeMap[townPos + hori + vert] = TileType.TOWN;
            else
                featureTileTypeMap.Add(townPos + hori + vert, TileType.TOWN);*/
        }
    }

    private void SetTiles() {
        Debug.Log("setTiles");
        P.start("Set Tiles");
        Vector2Int pos = new Vector2Int(0, 0);

        Vector2Int[,] deltas = new Vector2Int[3, 3];
        deltas[0, 0] = Vector2Int.up + Vector2Int.left;
        deltas[0, 1] = Vector2Int.up;
        deltas[0, 2] = Vector2Int.up + Vector2Int.right;
        deltas[1, 0] = Vector2Int.left;
        deltas[1, 1] = Vector2Int.zero;
        deltas[1, 2] = Vector2Int.right;
        deltas[2, 0] = Vector2Int.down + Vector2Int.left;
        deltas[2, 1] = Vector2Int.down;
        deltas[2, 2] = Vector2Int.down + Vector2Int.right;

        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                pos.x = x;
                pos.y = y;
                // Base tiles
                map.SetTile(new Vector3Int(pos.x, pos.y, baseDepth), tileSets[TileType.GRASS].getIndexedTile(0)); //grassTiles.getTile(0)

                //Terrain Tiles
                if (terrainTileTypeMap.ContainsKey(pos)) {
                    TileType terrainType = terrainTileTypeMap[pos];
                    // adujst which tiling to use for each terrain type
                    if (terrainType == TileType.WATER || terrainType == TileType.MOUNTAIN) {        //water is tiled depending on surrounding tiles
                        //collect surrounding tiles
                        int flags = 0;
                        for (int i = 0; i < 3; i++) {
                            for (int j = 0; j < 3; j++) {
                                Vector2Int surroundingPos = pos + deltas[i, j];
                                // treat ouside of map as same tile
                                if (!IsOnMap(surroundingPos)) {
                                    flags += 1 << i * 3 + j;
                                    // if terrain tile map has a tile and type == this type
                                } else if (terrainTileTypeMap.ContainsKey(surroundingPos)) {
                                    if (terrainTileTypeMap[surroundingPos] == terrainType) {
                                        flags += 1 << i * 3 + j;
                                    }
                                }
                            }
                        }
                        map.SetTile(new Vector3Int(pos.x, pos.y, terrainDepth), tileSets[terrainType].getIndexedTile(flags));
                    } else {
                        map.SetTile(new Vector3Int(pos.x, pos.y, terrainDepth), tileSets[terrainType].getIndexedTile(0));
                    }
                }
                // Feature Tiles
                if (featureTileTypeMap.ContainsKey(pos)) {
                    //collect surrounding tiles
                    TileType featureType = featureTileTypeMap[pos];
                    // adujst which tiling to use for each terrain type
                    if (featureType == TileType.RIVER || featureType == TileType.ROAD) {
                        int flags = 0;
                        for (int i = 0; i < 3; i++) {
                            for (int j = 0; j < 3; j++) {
                                Vector2Int surroundingPos = pos + deltas[i, j];
                                // treat ouside of map as same tile
                                if (!IsOnMap(surroundingPos)) {
                                    flags += 1 << i * 3 + j;
                                    // if terrain tile map has a tile and type == this type
                                } else if (featureTileTypeMap.ContainsKey(surroundingPos)) {
                                    if (featureTileTypeMap[surroundingPos] == featureType) {
                                        flags += 1 << i * 3 + j;
                                    }
                                }
                            }
                        }

                        map.SetTile(new Vector3Int(pos.x, pos.y, featureDepth), tileSets[featureType].getIndexedTile(flags));
                    }
                    if (featureType == TileType.TOWN) {
                        map.SetTile(new Vector3Int(pos.x, pos.y, featureDepth), tileSets[TileType.TOWN].getTile(0));
                    }
                }
            }
            P.end();
        }
    }

    private bool IsOnMap(Vector2Int pos) {
        return (pos.x >= 0) && (pos.x < dimensions.x) && (pos.y >= 0) && (pos.y < dimensions.y);
    }

    private void CleanUpTerrain(int iterationCount) {
        Vector2Int[,] deltas = new Vector2Int[3, 3];
        deltas[0, 0] = Vector2Int.up + Vector2Int.left;
        deltas[0, 1] = Vector2Int.up;
        deltas[0, 2] = Vector2Int.up + Vector2Int.right;
        deltas[1, 0] = Vector2Int.left;
        deltas[1, 1] = Vector2Int.zero;
        deltas[1, 2] = Vector2Int.right;
        deltas[2, 0] = Vector2Int.down + Vector2Int.left;
        deltas[2, 1] = Vector2Int.down;
        deltas[2, 2] = Vector2Int.down + Vector2Int.right;

        for (int iterations = 0; iterations < iterationCount; iterations++) {
            for (int x = 0; x < dimensions.x; x++) {
                for (int y = 0; y < dimensions.y; y++) {
                    Vector2Int pos = new Vector2Int(x, y);

                    // do only if terrain tile exists here
                    if (terrainTileTypeMap.ContainsKey(pos)) {

                        TileType type = terrainTileTypeMap[pos];

                        //check surrounding tiles for equality
                        int differentTilesAround = 0;
                        for (int i = 0; i < 3; i++) {
                            for (int j = 0; j < 3; j++) {
                                Vector2Int surroundingPos = pos + deltas[i, j];
                                // treat ouside of map as same tile
                                if (IsOnMap(surroundingPos)) {
                                    if (!(terrainTileTypeMap.ContainsKey(surroundingPos)) || !(terrainTileTypeMap[surroundingPos] == terrainTileTypeMap[pos])) {
                                        differentTilesAround++;
                                    }
                                }
                            }
                        }
                        Debug.Log("same tile count at " + pos + "(" + terrainTileTypeMap[pos] + ")" + ": " + differentTilesAround);
                        if (differentTilesAround >= cleanUpCutOffNumber)
                            terrainTileTypeMap.Remove(pos);
                    }
                }
            }
        }
    }


    private void Log2DArray<T>(T[,] array) {
        if (array.Rank != 2)
            Debug.LogWarning("Can only log 2D arrays.");
        string output = "[";
        for (int i = 0; i < array.GetLength(0); i++) {
            output += "[";
            for (int j = 0; j < array.GetLength(1); j++) {
                output += array[i, j] + " ";
            }
            output += "]";
        }
        Debug.Log(output + "]");
    }

    protected bool TileExists(Vector2Int position) {
        return terrainTileTypeMap.ContainsKey(position);
    }

    private Tile GetTile(int tileIndex, TileType tileType) {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        switch (tileType) {
            case TileType.GRASS:
                tile = grassTiles.getTile(tileIndex); break;
            case TileType.FOREST:
                tile = forestTiles.getTile(tileIndex); break;
            case TileType.WATER:
                tile = waterTiles.getTile(tileIndex); break;
            case TileType.MOUNTAIN:
                tile = mountainTiles.getTile(tileIndex); break;
            case TileType.TOWN:
                tile = townTiles.getTile(tileIndex); break;
            case TileType.RIVER:
                tile = riverTiles.getTile(tileIndex); break;
            case TileType.ROAD:
                tile = roadTiles.getTile(tileIndex); break;
        }
        return tile;
    }
}

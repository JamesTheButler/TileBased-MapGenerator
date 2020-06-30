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


    private Dictionary<Vector2Int, TileType> terrainTileTypeMap;
    private Dictionary<Vector2Int, TileType> featureTileTypeMap;

    private Dictionary<TileType, TileSet> tileSets;

    private List<Vector2Int> mountainTileList;
    private List<Vector2Int> waterTileList;

    private void Start() {
        P.setState(profile);

        // init
        initTiles();
        // make noise
        if (!useSeed)
            generateSeed();
        UnityEngine.Random.InitState(seed);

        Debug.Log("Map Seed: " + seed);
        // generate 
        generateMap(seed);
        // cleanup pass
        cleanUpTerrain(cleanUpIterations);
        // generate rivers
        generateRivers(seed, dimensions);
        // generate town positions
        generateTowns(seed, dimensions, townCount/*, minTownDistance*/);
        // set tiles
        setTiles();
        // set up cam 
        camMover.setMap(dimensions);
    }


    private void Awake() {
        terrainTileTypeMap = new Dictionary<Vector2Int, TileType>();
        featureTileTypeMap = new Dictionary<Vector2Int, TileType>();
        tileSets = new Dictionary<TileType, TileSet>();
        mountainTileList = new List<Vector2Int>();
        waterTileList = new List<Vector2Int>();
    }

    private void generateSeed() {
        seed = UnityEngine.Random.Range(0, 10000);
    }

    protected void initTiles() {
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

    private void generateMap(int seed) {
        Debug.Log("Generate Noise.");
        P.start("Generate Noise");

        int forestOffset = seed;
        int terrainOffset = seed;
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                Vector2Int pos = new Vector2Int(x, y);
                // terrainTileTypeMap.Add(key, TileType.GRASS);

                float terrain_noise = Mathf.PerlinNoise(x / terrainNoiseScale + terrainOffset, y / terrainNoiseScale + terrainOffset);
                if (terrain_noise < waterTiles.heightLevel) {
                    terrainTileTypeMap[pos] = TileType.WATER;
                    waterTileList.Add(pos);
                } else if (terrain_noise > mountainTiles.heightLevel) {
                    terrainTileTypeMap[pos] = TileType.MOUNTAIN;
                    mountainTileList.Add(pos);
                } else {
                    float red_noise = Mathf.PerlinNoise(x / forestNoiseScale + forestOffset, y / forestNoiseScale + forestOffset);
                    if (red_noise > forestTiles.heightLevel)
                        terrainTileTypeMap[pos] = TileType.FOREST;
                }
            }
        }
        P.end();
    }

    private bool isDiagonal(Vector2Int v) { return v.x != 0 && v.y != 0; }

    private void generateRivers(int seed, Vector2Int dimensions) {
        P.start("generateRiver");
        Debug.Log("generateRiver");
        for (int i = 0; i < riverCount; i++) {
            Vector2Int pos = mountainTileList[UnityEngine.Random.Range(0, mountainTileList.Count)];

            featureTileTypeMap[pos] = TileType.RIVER;
            bool hasReachedEnd = false;
            Vector2Int direction = new Vector2Int(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1));
            //Vector2Int direction = new Vector2Int(-1, 1);
            Debug.Log("general direction " + direction);

            while (!hasReachedEnd) {
                Vector2Int tempDir = direction;
                if (isDiagonal(tempDir)) {
                   /* if (UnityEngine.Random.value > 0.5) {
                        if (UnityEngine.Random.value > 0.5)
                            direction.x = 0;
                    } else {
                        if (UnityEngine.Random.value > 0.5)
                            direction.y = 0;
                    }*/
                } else {
                    tempDir.x = UnityEngine.Random.Range(-1, 1);
                }
                Debug.Log("tile direction " + tempDir);
                // direction.y = UnityEngine.Random.Range(-1, 1);
                pos += tempDir;
                //fix curves (and connect corners for proper tiling)
                if (isDiagonal(tempDir)) { 
                    if (UnityEngine.Random.value > 0.5)
                        featureTileTypeMap[pos - new Vector2Int(tempDir.x, 0)] = TileType.RIVER;
                    else
                        featureTileTypeMap[pos - new Vector2Int(0, tempDir.y)] = TileType.RIVER;
                }
                featureTileTypeMap[pos] = TileType.RIVER;
                hasReachedEnd = isRiverEnd(pos);
            }
        }
        Debug.Log(featureTileTypeMap.Count);
        P.end();
    }

    private bool isRiverEnd(Vector2Int pos) {
        if (!isOnMap(pos))
            return true;
        if (terrainTileTypeMap.ContainsKey(pos) && terrainTileTypeMap[pos] == TileType.WATER)
            return true;
        return false;

    }

    private int findBestDivider(int number) {
        List<int> dividers = new List<int>();
        for (int i = 1; i <= number; i++) {
            if (number % i == 0) {
                dividers.Add((int)(number / i));
            }
        }
        return dividers[dividers.Count / 2];
    }

    private List<Rect> getQuadrants(Vector2Int dimensions, int townCount) {
        List<Rect> rects = new List<Rect>();
        int columnCount = findBestDivider(townCount);
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
    private void generateTowns(int seed, Vector2Int dimensions, int townCount/* int townSize*/) {
        if (townCount == 0)
            return;

        List<Rect> quadrants = getQuadrants(dimensions, townCount);
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
            featureTileTypeMap.Add(townPos, TileType.TOWN);
            Vector2Int hori = Vector2Int.right;
            Vector2Int vert = Vector2Int.down;

            // TODO: can be circumvented by only allowing positions of dimensions - town size 
            if (townPos.x == dimensions.x)
                hori = Vector2Int.left;
            if (townPos.y == dimensions.y)
                vert = Vector2Int.up;

            featureTileTypeMap.Add(townPos + hori, TileType.TOWN);
            featureTileTypeMap.Add(townPos + vert, TileType.TOWN);
            featureTileTypeMap.Add(townPos + hori + vert, TileType.TOWN);
        }
    }

    private void setTiles() {
        Debug.Log("set tiles");
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
                                if (!isOnMap(surroundingPos)) {
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
                    if (featureTileTypeMap[pos] == featureType) {
                        int flags = 0;
                        for (int i = 0; i < 3; i++) {
                            for (int j = 0; j < 3; j++) {
                                Vector2Int surroundingPos = pos + deltas[i, j];
                                // treat ouside of map as same tile
                                if (!isOnMap(surroundingPos)) {
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
                }
            }
            P.end();
        }
    }

    private bool isOnMap(Vector2Int pos) {
        return (pos.x >= 0) && (pos.x < dimensions.x) && (pos.y >= 0) && (pos.y < dimensions.y);
    }

    private void cleanUpTerrain(int iterationCount) {
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
                                if (isOnMap(surroundingPos)) {
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


    private void log2DArray<T>(T[,] array) {
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

    protected bool tileExists(Vector2Int position) {
        return terrainTileTypeMap.ContainsKey(position);
    }

    private Tile getTile(int tileIndex, TileType tileType) {
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

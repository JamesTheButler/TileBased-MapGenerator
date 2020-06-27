using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/* **** THIS IS WHERE I LEFT OF YESTERDAY ****
 * - tile indexer used correctly
 * - tile indices are now determined before being drawn
 * - rivers and road have different rules for vicinity flags
 */

public class NoiseBasedTiledMapGenerator : MonoBehaviour {
    public Tilemap map;
    //public List<TileSet> tileSets;

    public TileSet grassTiles;
    public TileSet mountainTiles;
    public TileSet waterTiles;
    public TileSet forestTiles;

    public TileSet roadTiles;
    public TileSet riverTiles;
    public TileSet townTiles;

    public float terrainNoiseScale;
    public float forestNoiseScale;

    public CameraMovement camMover;
        
    public Vector2Int dimensions;
    private int baseDepth = 0;
    private int terrainDepth = 1;
    private int featureDepth = 2;

    public int minRivers;
    public int maxRivers;

    public int townCount;
    
    public bool useSeed = false;
    public int seed = 0;

    public bool useSegmentation;

    public bool profile;

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

    private Dictionary<Vector2Int, TileType> terrainTileTypeMap;
    private Dictionary<Vector2Int, TileType> featureTileTypeMap;

    private Dictionary<TileType, TileSet> tileSets;

    private void Start() {
        P.setState(profile);

        // init
        initTiles();
        // make noise
        if (!useSeed)
            generateSeed();
        Random.InitState(seed);

        Debug.Log("Map Seed: " + seed);
        //generate 
        generateMap(seed);
        generateRivers(seed, dimensions);
        //generate town positions
        generateTowns(seed, dimensions, townCount/*, minTownDistance*/);
        // set tiles
        setTiles();
        //set up cam 
        camMover.setMap(dimensions);
    }

    private void Awake() {
        terrainTileTypeMap = new Dictionary<Vector2Int, TileType>();
        featureTileTypeMap = new Dictionary<Vector2Int, TileType>();
        tileSets = new Dictionary<TileType, TileSet>();
    }

    private void generateSeed() {
        seed = UnityEngine.Random.Range(0, 10000);
    }

    protected void initTiles() {
        // setup tile indexers
        TileIndexer t = new TileIndexer();
        PathingTileIndexer pt = new PathingTileIndexer();
        TerrainTileIndexer tt = new TerrainTileIndexer();
        grassTiles.init(t);
        waterTiles.init(tt);
        mountainTiles.init(tt);
        forestTiles.init(tt);
        townTiles.init(t);
        roadTiles.init(pt);
        riverTiles.init(pt);
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
                Vector2Int key = new Vector2Int(x, y);
                terrainTileTypeMap.Add(key, TileType.GRASS);

                float terrain_noise = Mathf.PerlinNoise(x / terrainNoiseScale + terrainOffset, y / terrainNoiseScale + terrainOffset);
                if (terrain_noise < waterTiles.heightLevel) { 
                    terrainTileTypeMap[key] = TileType.WATER;
                }else if (terrain_noise > mountainTiles.heightLevel) { 
                    terrainTileTypeMap[key] = TileType.MOUNTAIN;
                } else { 
                    float red_noise = Mathf.PerlinNoise(x / forestNoiseScale + forestOffset, y / forestNoiseScale + forestOffset);
                    if (red_noise > forestTiles.heightLevel)
                        terrainTileTypeMap[key] = TileType.FOREST;
                }
            }
        }
        P.end();
    }

    private void generateRivers(int seed, Vector2Int dimensions) {
        int riverCount = Random.Range(minRivers, maxRivers);
    }

    private int findBestDivider(int number) {
        List<int> dividers = new List<int>();
        for(int i=1; i<=number; i++) {
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
        Vector2 rectSize = new Vector2(dimensions.x/columnCount, dimensions.y/rowCount);

        for (int i = 0; i < columnCount; i++) {
            for (int j = 0; j < rowCount; j++) {
                rects.Add(new Rect(i * rectSize[0], j * rectSize[1], rectSize[0], rectSize[1]));
            }
        }
        return rects;
    }

    //TODO: set size town tile count
    private void generateTowns(int seed, Vector2Int dimensions, int townCount/* int townSize*/) {
          List<Rect> quadrants = getQuadrants(dimensions, townCount);
        for (int i = 0; i < townCount; i++) {
            Vector2Int townPos;
            if (useSegmentation) { 
                townPos = new Vector2Int(Random.Range((int) quadrants[i].position.x, (int) quadrants[i].position.x + (int) quadrants[i].size.x),
                                         Random.Range((int) quadrants[i].position.y, (int) quadrants[i].position.y + (int) quadrants[i].size.y));
            } else {
                townPos = new Vector2Int(Random.Range(0, dimensions.x), Random.Range(0, dimensions.y));
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

            featureTileTypeMap.Add(townPos + hori,          TileType.TOWN);
            featureTileTypeMap.Add(townPos + vert,          TileType.TOWN);
            featureTileTypeMap.Add(townPos + hori + vert,   TileType.TOWN);
        }
    }

    private void setTiles() {
        Debug.Log("set tiles");
        P.start("Set Tiles");
        Vector2Int pos = new Vector2Int(0, 0);
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                pos.x = x;
                pos.y = y;
                map.SetTile(new Vector3Int(pos.x, pos.y, baseDepth), grassTiles.getTile(0));

                Tile terrainTile = tileSets[terrainTileTypeMap[pos]].getIndexedTile(getSurroundingFlags(pos));
                map.SetTile(new Vector3Int(pos.x, pos.y, terrainDepth), terrainTile);

                if (featureTileTypeMap.ContainsKey(pos)) {
                    map.SetTile(new Vector3Int(pos.x, pos.y, featureDepth), townTiles.getTile(0));
                }
            }
        }
        P.end();
    }

    protected bool tileExists(Vector2Int position) {
        return terrainTileTypeMap.ContainsKey(position);
    }

    private Tile getTile(int tileIndex, TileType tileType){
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
                tile = townTiles.getTile(tileIndex); break;
            case TileType.ROAD:
                tile = townTiles.getTile(tileIndex); break;
        }
        return tile;
    }

    private Tile getTile(Dictionary<Vector2Int, int> tileIndexMap, Dictionary<Vector2Int, TileType> tileTypeMap, Vector2Int pos) {
        int id = tileIndexMap[pos];
        Tile tile = grassTiles.getTile(id);
        switch (tileTypeMap[pos]) {
            case TileType.FOREST:
                tile = forestTiles.getTile(id); break;
            case TileType.WATER:
                tile = waterTiles.getTile(id); break;
            case TileType.MOUNTAIN:
                tile = mountainTiles.getTile(id); break;
            case TileType.TOWN:
                Debug.Log("town " + id);
                tile = townTiles.getTile(id); break;
            case TileType.RIVER:
                Debug.Log("river " + id);
                tile = townTiles.getTile(id); break;
            case TileType.ROAD:
                Debug.Log("road " + id);
                tile = townTiles.getTile(id); break;
        }
        return tile;
    }

    protected int getSurroundingFlags(Vector2Int pos) {
        // edge Flags:
        // ___|__1___|___
        // _8_|_tile_|_2_
        // ___|__4___|___

        // corner Flags:
        // _1_|______|_2_
        // ___|_tile_|___
        // _8_|______|_4_

        TileType thisType = terrainTileTypeMap[pos];
        int edgeTileFlags = 0;
        int cornerTileFlags = 0;

        if (terrainTileTypeMap[pos] == TileType.GRASS)
            return 4;       //TODO: HACK!!!!
        if (terrainTileTypeMap[pos] == TileType.TOWN) {
            Debug.Log("get flags town");
            return 6;       //TODO: HACK!!!!
        }

        Vector2Int top = pos + new Vector2Int(0, 1);
        Vector2Int right = pos + new Vector2Int(1, 0);
        Vector2Int bottom = pos + new Vector2Int(0, -1);
        Vector2Int left = pos + new Vector2Int(-1, 0);

        // surroundingTileFlags += (tileExists(top))  && tileTypeMap[top] == thisType) ? 1 : 0;
        // surroundingTileFlags += (tileExists(right))  && tileTypeMap[right] == thisType) ? 2 : 0;
        // surroundingTileFlags += (tileExists(bottom)) && tileTypeMap[bottom] == thisType) ? 4 : 0;
        // surroundingTileFlags += (tileExists(left)) && tileTypeMap[left] == thisType) ? 8 : 0;

        edgeTileFlags += (!tileExists(top)  || ((tileExists(top)  && terrainTileTypeMap[top]  == thisType))) ? 1 : 0;
        edgeTileFlags += (!tileExists(right)  || ((tileExists(right)  && terrainTileTypeMap[right]  == thisType))) ? 2 : 0;
        edgeTileFlags += (!tileExists(bottom) || ((tileExists(bottom) && terrainTileTypeMap[bottom] == thisType))) ? 4 : 0;
        edgeTileFlags += (!tileExists(left) || ((tileExists(left) && terrainTileTypeMap[left] == thisType))) ? 8 : 0;

        /*
        cornerTileFlags += (!tileExists(top+left) || ((tileExists(top) && terrainTileTypeMap[top] == thisType))) ? 1 : 0;
        cornerTileFlags += (!tileExists(right) || ((tileExists(right) && terrainTileTypeMap[right] == thisType))) ? 2 : 0;
        cornerTileFlags += (!tileExists(bottom) || ((tileExists(bottom) && terrainTileTypeMap[bottom] == thisType))) ? 4 : 0;
        cornerTileFlags += (!tileExists(left) || ((tileExists(left) && terrainTileTypeMap[left] == thisType))) ? 8 : 0;
        */
        return edgeTileFlags;
    }

    protected int getNewTileIndex(int vicinityFlag)
    {
        // Flags:
        // _._|__1___|_._
        // _8_|_tile_|_2_
        // _._|__4___|_._

        int newTileId = 0;
        switch (vicinityFlag) {
            //case 0: break;
            //case 1: break;
            //case 2: break;
            case 3: newTileId = 6; break;
            //case 4: break;
            //case 5: break;
            case 6: newTileId = 0; break;
            case 7: newTileId = 3; break;
            //case 8: break;
            case 9: newTileId = 8; break;
            //case 10: break;
            case 11: newTileId = 7; break;
            case 12: newTileId = 2; break;
            case 13: newTileId = 5; break;
            case 14: newTileId = 1; break;
            case 15: newTileId = 4; break;
            default: newTileId = 4; break;
        }
        return newTileId;
    }
}

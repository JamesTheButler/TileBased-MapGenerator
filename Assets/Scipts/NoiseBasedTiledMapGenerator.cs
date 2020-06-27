using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


/* **** THIS IS WHERE I LEFT OF YESTERDAY ****
 * - added tile indexer to generalize finding tile id based on vicinity flags
 * - have to add it to updateMap
 * - rivers and road have different rules for vicinity flags
 * */

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
    private Dictionary<Vector2Int, int> terrainTileIndexMap;
    private Dictionary<Vector2Int, TileType> featureTileTypeMap;
    private Dictionary<Vector2Int, int> featureTileIndexMap;

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
        // generate tile ids
        updateMap();
        // set tiles
        setTiles();
        //set up cam 
        camMover.setMap(dimensions);
    }

    private void Awake() {
        terrainTileTypeMap = new Dictionary<Vector2Int, TileType>();
        terrainTileIndexMap = new Dictionary<Vector2Int, int>();
        featureTileTypeMap = new Dictionary<Vector2Int, TileType>();
        featureTileIndexMap = new Dictionary<Vector2Int, int>();
    }

    private void generateSeed() {
        seed = UnityEngine.Random.Range(0, 10000);
    }

    protected void initTiles() {
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
                //set index of def
                terrainTileIndexMap[new Vector2Int(x,y)] = 4;

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
            featureTileIndexMap.Add(townPos, 0);
            Vector2Int hori = Vector2Int.right;
            Vector2Int vert = Vector2Int.down;

            // TODO: can be circumvented by only allowing positions of dimensions - town size 

            if (townPos.x == dimensions.x)
                hori = Vector2Int.left;
            if (townPos.y == dimensions.y)
                vert = Vector2Int.up;

            featureTileTypeMap.Add(townPos + hori,          TileType.TOWN);
            featureTileIndexMap.Add(townPos + hori, 0);
            featureTileTypeMap.Add(townPos + vert,          TileType.TOWN);
            featureTileIndexMap.Add(townPos + vert, 0);
            featureTileTypeMap.Add(townPos + hori + vert,   TileType.TOWN);
            featureTileIndexMap.Add(townPos + hori + vert, 0);
        }
    }

    public void updateMap() {
        Debug.Log("updateMap");
        P.start("Update Map");
        for (int x = 0; x < dimensions.x; x++) {
            Vector2Int position = new Vector2Int(0, 0);
            for (int y = 0; y < dimensions.y; y++) {
                position.x = x;
                position.y = y;
                terrainTileIndexMap[position] = getNewTileIndex(getSurroundingFlags(position));
            }
        }
        P.end();
    }

    private void setTiles() {
        Debug.Log("set tiles");
        P.start("Set Tiles");
        Vector2Int pos = new Vector2Int(0, 0);
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                pos.x = x;
                pos.y = y;
                map.SetTile(new Vector3Int(pos.x, pos.y, baseDepth), grassTiles.getTile(4));
                map.SetTile(new Vector3Int(pos.x, pos.y, terrainDepth), getTile(terrainTileIndexMap, terrainTileTypeMap, pos));
                if (featureTileTypeMap.ContainsKey(pos)) {
                 //   Debug.Log("feature found");
                 //   Debug.Log(featureTileIndexMap[pos]);
                 //   Debug.Log(featureTileTypeMap[pos]);
                    map.SetTile(new Vector3Int(pos.x, pos.y, featureDepth), getTile(featureTileIndexMap[pos], featureTileTypeMap[pos]));
                }
            }
        }
        P.end();
    }

    protected bool tileExists(Vector2Int position) {
        return terrainTileTypeMap.ContainsKey(position);
    }

    private Tile getTile(int  tileIndex, TileType tileType){
        Tile tile = new Tile() ;
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
        TileType thisType = terrainTileTypeMap[pos];
        int surroundingTileFlags = 0;

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

        surroundingTileFlags += (!tileExists(top)  || ((tileExists(top)  && terrainTileTypeMap[top]  == thisType))) ? 1 : 0;
        surroundingTileFlags += (!tileExists(right)  || ((tileExists(right)  && terrainTileTypeMap[right]  == thisType))) ? 2 : 0;
        surroundingTileFlags += (!tileExists(bottom) || ((tileExists(bottom) && terrainTileTypeMap[bottom] == thisType))) ? 4 : 0;
        surroundingTileFlags += (!tileExists(left) || ((tileExists(left) && terrainTileTypeMap[left] == thisType))) ? 8 : 0;

        return surroundingTileFlags;
    }

    protected int getNewTileIndex(int vicinityFlag) {
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

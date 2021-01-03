using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainAutoTileIndexer : BaseTerrainTileIndexer {
    public Texture2D tileTexture;
    private List<Tile> tiles;

    void Start() {
        string spriteSheet = AssetDatabase.GetAssetPath(tileTexture);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet).OfType<Sprite>().ToArray();
        tiles = new List<Tile>();
        foreach (var sprite in sprites) {
            Tile t = ScriptableObject.CreateInstance<Tile>();
            t.sprite = sprite;
            tiles.Add(t);
        }
    }

    public override Tile Index(int vicinityFlag) {
        return tiles[GetTileIndex(vicinityFlag)];
    }
}

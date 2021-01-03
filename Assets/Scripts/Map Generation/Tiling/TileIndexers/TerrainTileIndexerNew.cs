using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Map_Generation.Tiling.TileIndexers {
    public class TerrainTileIndexerNew : BaseTerrainTileIndexer {
        // Sprites
        public Sprite DefaultSprite;
        public Sprite CenterSprite;
        public Sprite OuterCornerTopRightSprite;
        public Sprite OuterCornerTopLeftSprite;
        public Sprite OuterCornerBottomRightSprite;
        public Sprite OuterCornerBottomLeftSprite;
        public Sprite EdgeTopSprite;
        public Sprite EdgeLeftSprite;
        public Sprite EdgeRightSprite;
        public Sprite EdgeBottomSprite;
        public Sprite InnerCornerTopRightSprite;
        public Sprite InnerCornerTopLeftSprite;
        public Sprite InnerCornerBottomRightSprite;
        public Sprite InnerCornerBottomLeftSprite;

        private Dictionary<int, Tile> indexToTileMap;
        private Tile defaultTile;

        private Tile SpriteToTile(Sprite sprite) {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            return tile;
        }

        void Awake() {
            defaultTile = SpriteToTile(DefaultSprite);
            indexToTileMap = new Dictionary<int, Tile>();
            indexToTileMap.Add(0, SpriteToTile(OuterCornerTopLeftSprite));
            indexToTileMap.Add(1, SpriteToTile(EdgeTopSprite));
            indexToTileMap.Add(2, SpriteToTile(OuterCornerTopRightSprite));
            indexToTileMap.Add(3, SpriteToTile(EdgeLeftSprite));
            indexToTileMap.Add(4, SpriteToTile(CenterSprite));
            indexToTileMap.Add(5, SpriteToTile(EdgeRightSprite));
            indexToTileMap.Add(6, SpriteToTile(OuterCornerBottomLeftSprite));
            indexToTileMap.Add(7, SpriteToTile(EdgeBottomSprite));
            indexToTileMap.Add(8, SpriteToTile(OuterCornerBottomRightSprite));
            indexToTileMap.Add(9, SpriteToTile(InnerCornerTopLeftSprite));
            indexToTileMap.Add(10, SpriteToTile(InnerCornerTopRightSprite));
            indexToTileMap.Add(11, SpriteToTile(InnerCornerBottomLeftSprite));
            indexToTileMap.Add(12, SpriteToTile(InnerCornerBottomRightSprite));
        }

        public override Tile Index(int vicinityFlag) {
            Tile tile;
            if (!indexToTileMap.TryGetValue(GetTileIndex(vicinityFlag), out tile))
                tile = defaultTile;
            return tile;
        }
    }
}

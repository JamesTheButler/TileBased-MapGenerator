using UnityEngine;
using UnityEngine.Tilemaps;

    public static class TileUtility {

        public static Tile SpriteToTile(Sprite sprite) {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            return tile;
        }

}

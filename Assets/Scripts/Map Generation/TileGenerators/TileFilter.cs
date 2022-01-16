using System.Collections.Generic;
using System.Linq;
using Convenience.Collections.Arrays;

/// <summary>
/// Removes tiles of type <c>tileType</c> from the tile type map, if there is any tile of type <c>filteredTileTypes</c> in the same position.
/// </summary>
public class TileFilter : BaseTileGenerator {

    public List<TileType> filters;

    public override bool[,] GenerateTiles(TileTypeMap tileTypeMap) {
        var thisLayer = tileTypeMap.GetLayer(tileType);

        if (!IsEnabled) return thisLayer;
        if (filters.Count <= 0) return thisLayer;

        var filterMap = buildFilterMap(tileTypeMap);
        if (filterMap == null) return thisLayer;

        for (int x = 0; x < tileTypeMap.size.x; x++) {
            for (int y = 0; y < tileTypeMap.size.y; y++) {
                if (!thisLayer[x, y]) continue;

                if (filterMap[x, y]) {
                    thisLayer[x, y] = false;
                }
            }
        }
        return thisLayer;
    }

    private bool[,] buildFilterMap(TileTypeMap tileTypeMap) {
        var activeFilters = new List<TileType>();
        foreach (var filter in filters) {
            if (tileTypeMap.HasLayer(filter)) {
                activeFilters.Add(filter);
            }
        }
        var activeFilterLayers = activeFilters.Select(filter => tileTypeMap.GetLayer(filter));
        return Array2DUtility.MergeArrays(activeFilterLayers, (x, y) => x || y);
    }
}

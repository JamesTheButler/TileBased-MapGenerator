using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour {
    [SerializeField] GameObject townInfoPanelPrefab;

    private readonly List<Town> towns = new List<Town>();
    private readonly Dictionary<Vector2Int, Town> townPositions = new Dictionary<Vector2Int, Town>();

    // TODO: Move these to new component "TownDataGenerator"
    readonly CommodityDict defaultConsumption = new CommodityDict() {
            { Commodity.FISH, 0.2 },
            { Commodity.WINE, 0.2 },
            { Commodity.WOOD, 0.2 }
        };
    readonly CommodityDict defaultStock = new CommodityDict {
            { Commodity.FISH, 20 },
            { Commodity.WINE, 20 },
            { Commodity.WOOD, 20 }
        };
    readonly CommodityDict defaultProduction = new CommodityDict {
            { Commodity.WOOD, 0.5 }
        };

    private void Awake() {
        MapGenerator.OnMapGenerationFinished += GenerateTowns;
        MapGenerator.OnNeighborsGenerated += SetNeighbors;
    }

    void Start() {
        /*
        towns.Add(new Town("Ebersbach", 2000, defaultStock, defaultConsumption, new CommodityDict(Commodity.FISH, 0.5)));
        towns.Add(new Town("Lauterbach", 200, defaultStock, defaultConsumption, new CommodityDict(Commodity.WOOD, 0.5)));
        towns.Add(new Town("Kalkreuth", 1000, defaultStock, defaultConsumption, new CommodityDict(Commodity.WINE, 0.5)));

        foreach (var town in towns) {
            var infoPanel = Instantiate(townInfoPanelPrefab, transform).GetComponent<TownInfoPanel>();
            infoPanel.ConnectTown(town);
            town.TriggerAllEvents();
        }
        */
    }

    public Town GetTown(Vector2Int position) {
        if (townPositions.ContainsKey(position)) {
            return townPositions[position];
        }
        return null;
    }

    private Dictionary<Vector2Int, List<Vector2Int>> neighbors = new Dictionary<Vector2Int, List<Vector2Int>>();

    public void SetNeighbors(Dictionary<Vector2Int, List<Vector2Int>> neighbors) {
        Debug.Log($"TownManager.setNeighbors -- received neighbors for {neighbors.Count} towns.");
        this.neighbors = neighbors;
    }

    // TODO: Move this to new component "TownDataGenerator"
    private void GenerateTowns(TileTypeMap tileTypeMap) {
        Debug.Log($"TownManager.GenerateTowns -- ");
        var townTiles = tileTypeMap.GetTiles(TileType.TOWN);
        var names = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < Mathf.Min(names.Length, townTiles.Count); i++) {
            var town = new Town(names[i] + "", 500, townTiles[i], defaultStock, defaultProduction, defaultConsumption);
            towns.Add(town);
            townPositions.Add(townTiles[i], town);
        }

        foreach (var entry in neighbors) {
            var output = $"{GetTown(entry.Key).Name} -";
            foreach (var val in entry.Value) {
                output += $" {GetTown(val).Name},";
            }
            Debug.Log(output);
        }
    }
}

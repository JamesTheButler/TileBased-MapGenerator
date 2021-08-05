using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour {
    [SerializeField] GameObject townInfoPanelPrefab;

    private List<Town> towns = new List<Town>();

    private Dictionary<Vector2Int, Town> townPositions = new Dictionary<Vector2Int, Town>();

    //public delegate void Tick();
    //public static event Tick OnTownTick;

    //[SerializeField] float townStockUpdateInterval = 1f;
    //[SerializeField] float townPopulationGrowthInterval = 5f;
    //private float lastUpdateTime = 0;

    CommodityDict defaultConsumption = new CommodityDict() {
            { Commodity.FISH, 0.2 },
            { Commodity.WINE, 0.2 },
            { Commodity.WOOD, 0.2 }
        };

    CommodityDict defaultStock = new CommodityDict {
            { Commodity.FISH, 20 },
            { Commodity.WINE, 20 },
            { Commodity.WOOD, 20 }
        };

    CommodityDict defaultProduction = new CommodityDict {
            { Commodity.WOOD, 0.5 }
        };


    void Start() {
        //towns.Add(new Town("Ebersbach", 2000, defaultStock, defaultConsumption, new CommodityDict(Commodity.FISH, 0.5)));
        //towns.Add(new Town("Lauterbach", 200, defaultStock, defaultConsumption, new CommodityDict(Commodity.WOOD, 0.5)));
        //towns.Add(new Town("Kalkreuth", 1000, defaultStock, defaultConsumption, new CommodityDict(Commodity.WINE, 0.5)));

        foreach (var town in towns) {
            //var infoPanel = Instantiate(townInfoPanelPrefab, transform).GetComponent<TownInfoPanel>();
            //infoPanel.ConnectTown(town);
            //town.TriggerAllEvents();
        }
        MapGenerator.OnMapGenerationFinished += GenerateTowns;
    }

    public Town GetTown(Vector2Int position) {
        if (townPositions.ContainsKey(position)) {
            return townPositions[position];
        }
        return null;
    }

    private Dictionary<Vector2Int, List<Vector2Int>> neighbors = new Dictionary<Vector2Int, List<Vector2Int>>();

    public void SetNeighbors(Dictionary<Vector2Int, List<Vector2Int>> neighbors) {
        this.neighbors = neighbors;
    }


    private void GenerateTowns(TileTypeMap tileTypeMap) {
        var townTiles = tileTypeMap.GetTiles(TileType.TOWN);
        var names = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < Mathf.Min(names.Length, townTiles.Count); i++) {
            var town = new Town(names[i] + "", 500, defaultStock, defaultProduction, defaultConsumption);
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

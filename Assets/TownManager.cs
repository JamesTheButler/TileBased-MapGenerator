using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour {
    [SerializeField] GameObject townInfoPanelPrefab;

    private List<Town> towns = new List<Town>();

    public delegate void Tick();
    public static event Tick OnTownTick;

    [SerializeField] float townStockUpdateInterval = 1f;
    [SerializeField] float townPopulationGrowthInterval = 5f;
    private float lastUpdateTime = 0;

    void Start() {
        var defaultConsumption = new CommodityDict() {
            { Commodity.FISH, 0.2 },
            { Commodity.WINE, 0.2 },
            { Commodity.WOOD, 0.2 }
        };

        var defaultStock = new CommodityDict {
            { Commodity.FISH, 20 },
            { Commodity.WINE, 20 },
            { Commodity.WOOD, 20 }
        };

        towns.Add(new Town("Ebersbach", 2000, defaultStock, defaultConsumption, new CommodityDict(Commodity.FISH, 0.5)));
        towns.Add(new Town("Lauterbach", 200, defaultStock, defaultConsumption, new CommodityDict(Commodity.WOOD, 0.5)));
        towns.Add(new Town("Kalkreuth", 1000, defaultStock, defaultConsumption, new CommodityDict(Commodity.WINE, 0.5)));

        foreach (var town in towns) {
            var infoPanel = Instantiate(townInfoPanelPrefab, transform).GetComponent<TownInfoPanel>();
            infoPanel.ConnectTown(town);
            town.TriggerAllEvents();
        }
    }

    private void Update() {
        if (Time.time - lastUpdateTime > townStockUpdateInterval) {
            OnTownTick?.Invoke();
            lastUpdateTime = Time.time;
        }
    }
}

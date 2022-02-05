using System.Collections.Generic;
using UnityEngine;

public class TownInfoPanel : MonoBehaviour {
    [SerializeField] GameObject nameLabel;
    [SerializeField] GameObject populationLabel;
    [SerializeField] GameObject stockListGO;
    [SerializeField] GameObject stockListItemPrefab;

    Dictionary<Commodity, TownInfoStockListItem> listDict = new Dictionary<Commodity, TownInfoStockListItem>();

    private CommodityDict stock = new CommodityDict();
    private CommodityDict consumption = new CommodityDict();
    private CommodityDict production = new CommodityDict();

    public void ConnectTown(Town town) {
        UiUtility.SetUiText(nameLabel, town.Name);
        UiUtility.SetUiText(populationLabel, $"Population: {town.Population}");

        town.OnPopulationChanged += Town_OnPopulationChanged;
        town.OnStockChanged += Town_OnStockChanged;
        town.OnProductionChanged += Town_OnProductionChanged;
        town.OnConsumptionChanged += Town_OnConsumptionChanged;
    }

    private void Town_OnConsumptionChanged(CommodityDict consumption) {
        this.consumption = consumption;
        UpdateStockList();
    }

    private void Town_OnProductionChanged(CommodityDict production) {
        this.production = production;
        UpdateStockList();
    }

    private void Town_OnStockChanged(CommodityDict stock) {
        this.stock = stock;
        UpdateStockList();
    }

    private void UpdateStockList() {
        var commodityList = new List<Commodity>();
        // Collect commodites from different lists
        foreach (var item in stock.Keys) {
            if (!commodityList.Contains(item)) commodityList.Add(item);
        }
        foreach (var item in consumption.Keys) {
            if (!commodityList.Contains(item)) commodityList.Add(item);
        }
        foreach (var item in production.Keys) {
            if (!commodityList.Contains(item)) commodityList.Add(item);
        }

        // update list view
        foreach (var commodity in commodityList) {
            var amount = stock.ContainsKey(commodity) ? stock[commodity] : 0.0f;
            var consumption = this.consumption.ContainsKey(commodity) ? this.consumption[commodity] : 0.0f;
            var production = this.production.ContainsKey(commodity) ? this.production[commodity] : 0.0f;

            if (!listDict.ContainsKey(commodity)) {
                var newItem = Instantiate(stockListItemPrefab, stockListGO.transform);
                listDict.Add(commodity, newItem.GetComponent<TownInfoStockListItem>());
            }
            listDict[commodity].UpdateListItem(commodity.ToString(), amount, production, consumption, production - consumption, 999.9f);
        }
    }

    private void Town_OnPopulationChanged(int newValue) {
        UiUtility.SetUiText(populationLabel, $"Population: {newValue}");
    }
}

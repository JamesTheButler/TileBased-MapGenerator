using UnityEngine;

public class Town{
    public string Name { get; set; }
    public Vector2Int Coordinates { get; set; }

    int _population;
    public int Population {
        get {
            return _population;
        }
        set {
            if (_population != value) {
                _population = value;
                OnPopulationChanged?.Invoke(value);
            }
        }
    }

    CommodityDict _stock;
    public CommodityDict Stock {
        get {
            return _stock;
        }
        set {
            if (_stock != value) {
                _stock = value;
                OnStockChanged?.Invoke(value);
            }
        }
    }

    CommodityDict _consumption;
    public CommodityDict Consumption {
        get {
            return _consumption;
        }
        set {
            if (_consumption != value) {
                _consumption = value;
                OnConsumptionChanged?.Invoke(value);
            }
        }
    }

    CommodityDict _production;
    public CommodityDict Production {
        get {
            return _production;
        }
        set {
            if (_production != value) {
                _production = value;
                OnProductionChanged?.Invoke(value);
            }
        }
    }

    public delegate void TownPropertyChanged<T>(T newValue);
    public event TownPropertyChanged<int> OnPopulationChanged;
    public event TownPropertyChanged<CommodityDict> OnStockChanged;
    public event TownPropertyChanged<CommodityDict> OnConsumptionChanged;
    public event TownPropertyChanged<CommodityDict> OnProductionChanged;

    public Town(string name,
                int population,
                Vector2Int coordinates,
                CommodityDict stock,
                CommodityDict consumption, CommodityDict production) {
        Name = name;
        Population = population;
        Coordinates = coordinates;
        Stock = stock;
        Consumption = consumption;
        Production = production;
        UpdateEventManager.OnTownStockUpdate += UpdateStock;
        UpdateEventManager.OnTownPopulationUpdate += UpdatePopulation;
    }

    private void UpdateStock() {
        var tempStock = new CommodityDict(Stock);
        //TODO: add and remove items from stock if they have been produced/consumed

        foreach (var comm in Production.Keys) {
            tempStock[comm] += Production[comm];
        }

        foreach (var comm in Consumption.Keys) {
            tempStock[comm] -= Consumption[comm];
        }

        Stock = tempStock;
    }

    private void UpdatePopulation() {
        Population += (int)(Population * 0.08f);
    }


    public void TriggerAllEvents() {
        OnPopulationChanged?.Invoke(Population);
        OnStockChanged?.Invoke(Stock);
        OnConsumptionChanged?.Invoke(Consumption);
        OnProductionChanged?.Invoke(Production);
    }

    public void OnInterval() {
        throw new System.NotImplementedException();
    }
}

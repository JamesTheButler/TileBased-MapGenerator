using System;
using UnityEngine;
public enum UpdateType {
    BASE_UPDATE,
    TOWN_STOCK,
    TOWN_POPULATION,
    PRICE_UPDATE,
}

public class UpdateEventManager : MonoBehaviour {
    public delegate void UpdateEvent();
    public static event UpdateEvent OnUpdate;
    public static event UpdateEvent OnTownPopulationUpdate;
    public static event UpdateEvent OnTownStockUpdate;
    public static event UpdateEvent OnPriceUpdate;

    [SerializeField] SerializableDict_UpdateType_float updateEventIntervalTable = new SerializableDict_UpdateType_float();
    SerializableDict_UpdateType_float updatEventLastUpdateTable = new SerializableDict_UpdateType_float();

    private void Invoke(UpdateType type) {
        switch (type) {
            case UpdateType.TOWN_POPULATION: OnTownPopulationUpdate?.Invoke(); break;
            case UpdateType.TOWN_STOCK: OnTownStockUpdate?.Invoke(); break;
            case UpdateType.PRICE_UPDATE: OnPriceUpdate?.Invoke(); break;
        }
    }

    private void Update() {
        OnUpdate?.Invoke();
        var now = Time.time;

        foreach (var updateEventType in updateEventIntervalTable.Keys) {
            if (!updatEventLastUpdateTable.ContainsKey(updateEventType)) {
                updatEventLastUpdateTable.Add(updateEventType, 0);
            }

            var lastUpdate = updatEventLastUpdateTable[updateEventType];
            var intervalTime = updateEventIntervalTable[updateEventType];
            if (now - lastUpdate > intervalTime) {
                updatEventLastUpdateTable[updateEventType] = now;
                Invoke(updateEventType);
            }
        }
    }
}

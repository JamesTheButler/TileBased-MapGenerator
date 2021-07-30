using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class UpdateEventManager : MonoBehaviour {
    public delegate void UpdateEvent();
    public static event UpdateEvent OnUpdate;
    public static event UpdateEvent OnTownPopulationUpdate;
    public static event UpdateEvent OnTownStockUpdate;
    public static event UpdateEvent OnPriceUpdate;

    [Serializable] public class UpdateTypeToFloatDictionary : SerializableDictionaryBase<UpdateType, float> { }
    [SerializeField] UpdateTypeToFloatDictionary updateEventIntervalTable = new UpdateTypeToFloatDictionary();
    UpdateTypeToFloatDictionary updatEventLastUpdateTable = new UpdateTypeToFloatDictionary();
    public enum UpdateType {
        BASE_UPDATE,
        TOWN_STOCK,
        TOWN_POPULATION,
        PRICE_UPDATE,
    }

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

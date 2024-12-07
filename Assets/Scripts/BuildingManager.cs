using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    private List<Building> buildings = new List<Building>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBuilding(Building building)
    {
        buildings.Add(building);
    }

    public void UnregisterBuilding(Building building)
    {
        buildings.Remove(building);
    }

    public float CalculateTotalPollution()
    {
        float totalPollution = 0f;
        foreach (var building in buildings)
        {
            totalPollution += building.GetPollutionCount();
        }
        return totalPollution;
    }

    // Список всех зданий
    public List<Building> GetAllBuildings()
    {
        return new List<Building>(buildings);
    }
}

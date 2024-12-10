using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public enum ResourceType
    {
        None,
        Honey,
        Stone,
        Iron,
        Copper,
        OrganicFood,
        CookedFood, // Добавляем готовую еду
        Fuel,
        Oil,
        Uranium,
        Plastic,
        IronPlate,
        CopperWire,
        Concrete
    }

    public static ResourceManager Instance { get; private set; }
    public ResourceUIManager resourceUIManager;

    // Публичные поля для стартового количества ресурсов
    public int initialHoney = 0;
    public int initialStone = 0;
    public int initialIron = 0;
    public int initialCopper = 0;
    public int initialOrganicFood = 0;
    public int initialCookedFood = 0;
    public int initialFuel = 0;
    public int initialOil = 0;
    public int initialUranium = 0;
    public int initialPlastic = 0;
    public int initialIronPlate = 0;
    public int initialCopperWire = 0;
    public int initialConcrete = 0;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, int> resourceLimits = new Dictionary<ResourceType, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources[resourceType] = 0;
            resourceLimits[resourceType] = 300; // Базовый лимит
        }

        // Устанавливаем начальные значения ресурсов
        resources[ResourceType.Honey] = initialHoney;
        resources[ResourceType.Stone] = initialStone;
        resources[ResourceType.Iron] = initialIron;
        resources[ResourceType.Copper] = initialCopper;
        resources[ResourceType.OrganicFood] = initialOrganicFood;
        resources[ResourceType.CookedFood] = initialCookedFood;
        resources[ResourceType.Fuel] = initialFuel;
        resources[ResourceType.Oil] = initialOil;
        resources[ResourceType.Uranium] = initialUranium;
        resources[ResourceType.Plastic] = initialPlastic;
        resources[ResourceType.IronPlate] = initialIronPlate;
        resources[ResourceType.CopperWire] = initialCopperWire;
        resources[ResourceType.Concrete] = initialConcrete;
    }

    public int GetResource(ResourceType resourceType)
    {
        return resources.TryGetValue(resourceType, out var amount) ? amount : 0;
    }

    public void AddResource(ResourceType resourceType, int amount)
    {
        if (!resources.ContainsKey(resourceType) || resourceType == ResourceType.None) return;

        resources[resourceType] = Mathf.Min(resources[resourceType] + amount, resourceLimits[resourceType]);
    }

    public bool RemoveResource(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.None) return false;

        if (resources.TryGetValue(resourceType, out var currentAmount) && currentAmount >= amount)
        {
            resources[resourceType] -= amount;
            return true;
        }
        return false;
    }

    public void SetResourceLimit(ResourceType resourceType, int limit)
    {
        if (resourceLimits.ContainsKey(resourceType) && resourceType != ResourceType.None)
        {
            resourceLimits[resourceType] = limit;
            resources[resourceType] = Mathf.Min(resources[resourceType], limit);
        }
    }
}

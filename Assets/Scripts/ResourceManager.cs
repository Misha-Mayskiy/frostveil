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
    }

    public int GetResource(ResourceType resourceType)
    {
        return resources.TryGetValue(resourceType, out var amount) ? amount : 0;
    }

    public void AddResource(ResourceType resourceType, int amount)
    {
        if (!resources.ContainsKey(resourceType) || resourceType == ResourceType.None) return;

        resources[resourceType] = Mathf.Min(resources[resourceType] + amount, resourceLimits[resourceType]);
        // Debug.Log($"Successful plused: {resourceType} = {resources[resourceType]}");
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

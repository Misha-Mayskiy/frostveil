using UnityEngine;

public class ThermalDrill : Building
{
    public enum ResourceType { None, Copper, Iron, Stone, Honey }
    public ResourceType resourceType;

    private float extractionRate = 1f;
    private float extractionTimer = 0f;
    private float pollutionCount = 100f;
    public float resourceDetectionRadius = 1f; // Радиус проверки ресурсов

    public float ExtractionRate => extractionRate; // Публичное свойство

    protected override void Start()
    {
        base.Start();
        category = "Extraction";
        workersRequired = 5;

        // Проверка ресурса при старте
        CheckResourceZone();
    }

    protected override void Update()
    {
        base.Update();
        if (IsOperational && (resourceType != ResourceType.None) && (currentWorkers >= workersRequired))
        {
            ExtractResources();
        }
    }

    private void ExtractResources()
    {
        extractionTimer += Time.deltaTime;

        if (extractionTimer >= 4f)
        {
            extractionTimer = 0f;
            ResourceManager.Instance.AddResource(ConvertResourceType(resourceType), Mathf.RoundToInt(extractionRate));
        }
    }

    public void CheckResourceZone()
    {
        resourceType = ResourceType.None; // Сбрасываем ресурс по умолчанию
        Collider[] colliders = Physics.OverlapSphere(transform.position, resourceDetectionRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Copper"))
            {
                resourceType = ResourceType.Copper;
                break;
            }
            else if (collider.CompareTag("Iron"))
            {
                resourceType = ResourceType.Iron;
                break;
            }
            else if (collider.CompareTag("Stone"))
            {
                resourceType = ResourceType.Stone;
                break;
            }
            else if (collider.CompareTag("Honey"))
            {
                resourceType = ResourceType.Honey;
                break;
            }
        }

        if (resourceType == ResourceType.None)
        {
            isOperational = false; // Отключаем бур, если ресурс не найден
            Debug.LogWarning($"ThermalDrill {gameObject.name} установлен не на ресурсе!");
        }
        else
        {
            isOperational = true; // Включаем бур, если ресурс найден
        }
    }

    private ResourceManager.ResourceType ConvertResourceType(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Copper:
                return ResourceManager.ResourceType.Copper;
            case ResourceType.Iron:
                return ResourceManager.ResourceType.Iron;
            case ResourceType.Stone:
                return ResourceManager.ResourceType.Stone;
            case ResourceType.Honey:
                return ResourceManager.ResourceType.Honey;
            default:
                return ResourceManager.ResourceType.None;
        }
    }

    public override float GetPollutionCount()
    {
        return pollutionCount * GetPollutionRate();
    }

    protected override void OnOperationalStateChanged()
    {
        CheckResourceZone(); // Проверить наличие ресурсов заново
        isOperational = (currentWorkers >= workersRequired) && isOperational;
    }
}

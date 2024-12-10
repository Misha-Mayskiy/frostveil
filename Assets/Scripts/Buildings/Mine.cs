using UnityEngine;

public class Mine : Building
{
    public enum ResourceType { None, Oil, Uranium }
    public ResourceType resourceType;

    private float extractionRate = 1f; // Скорость добычи ресурсов
    private float extractionTimer = 0f; // Таймер добычи
    public float resourceDetectionRadius = 1f; // Радиус проверки ресурсов
    public float ExtractionRate => extractionRate; // Публичное свойство для скорости добычи

    protected override void Start()
    {
        base.Start();
        category = "Extraction";
        workersRequired = 10;

        // Проверяем наличие ресурсов на старте
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

        if (extractionTimer >= 8f) // Интервал добычи
        {
            extractionTimer = 0f;
            ResourceManager.Instance.AddResource(ConvertResourceType(resourceType), Mathf.RoundToInt(extractionRate));
        }
    }

    private void CheckResourceZone()
    {
        resourceType = ResourceType.None; // Сбрасываем ресурс по умолчанию
        Collider[] colliders = Physics.OverlapSphere(transform.position, resourceDetectionRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Oil"))
            {
                resourceType = ResourceType.Oil;
                break;
            }
            else if (collider.CompareTag("Uranium"))
            {
                resourceType = ResourceType.Uranium;
                break;
            }
        }

        if (resourceType == ResourceType.None)
        {
            isOperational = false;
            Debug.LogWarning($"Mine {gameObject.name} установлен не на ресурсе!");
        }
        else
        {
            isOperational = true;
        }
    }

    private ResourceManager.ResourceType ConvertResourceType(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Oil:
                return ResourceManager.ResourceType.Oil;
            case ResourceType.Uranium:
                return ResourceManager.ResourceType.Uranium;
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
        CheckResourceZone(); // Повторная проверка ресурсов при изменении состояния
        isOperational = (currentWorkers >= workersRequired) && isOperational;
    }
}

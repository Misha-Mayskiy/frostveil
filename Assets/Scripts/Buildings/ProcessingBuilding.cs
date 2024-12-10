using UnityEngine;

public class ProcessingBuilding : Building
{
    public enum RecipeType { None, CopperWire, IronPlate, Concrete, Fuel }
    public RecipeType CurrentRecipe { get; private set; } = RecipeType.None;

    private float productionTimer = 0f;
    private float productionInterval = 8f; // Интервал производства

    protected override void Start()
    {
        base.Start();
        category = "Processing";
        workersRequired = 5;
    }

    protected override void Update()
    {
        base.Update();

        // Производство ресурсов только если здание включено и рецепт выбран
        if (IsOperational && CurrentRecipe != RecipeType.None)
        {
            ProduceResources();
        }
    }

    private void ProduceResources()
    {
        productionTimer += Time.deltaTime;

        if (productionTimer >= productionInterval)
        {
            productionTimer = 0f;

            // Получаем типы ресурсов для текущего рецепта
            ResourceManager.ResourceType producedResource = GetProducedResourceType(CurrentRecipe);
            ResourceManager.ResourceType requiredResource = GetRequiredResourceType(producedResource);

            // Проверяем наличие входного ресурса и производим выходной
            if (ResourceManager.Instance.RemoveResource(requiredResource, 1))
            {
                ResourceManager.Instance.AddResource(producedResource, 1);
                Debug.Log($"Произведён ресурс: {producedResource}");
            }
            else
            {
                Debug.LogWarning($"Недостаточно ресурса: {requiredResource} для производства {producedResource}");
            }
        }
    }

    private ResourceManager.ResourceType GetProducedResourceType(RecipeType recipe)
    {
        switch (recipe)
        {
            case RecipeType.CopperWire: return ResourceManager.ResourceType.CopperWire;
            case RecipeType.IronPlate: return ResourceManager.ResourceType.IronPlate;
            case RecipeType.Concrete: return ResourceManager.ResourceType.Concrete;
            case RecipeType.Fuel: return ResourceManager.ResourceType.Fuel;
            default: return ResourceManager.ResourceType.None;
        }
    }

    private ResourceManager.ResourceType GetRequiredResourceType(ResourceManager.ResourceType producedResource)
    {
        switch (producedResource)
        {
            case ResourceManager.ResourceType.CopperWire: return ResourceManager.ResourceType.Copper;
            case ResourceManager.ResourceType.IronPlate: return ResourceManager.ResourceType.Iron;
            case ResourceManager.ResourceType.Concrete: return ResourceManager.ResourceType.Stone;
            case ResourceManager.ResourceType.Fuel: return ResourceManager.ResourceType.Honey;
            default: return ResourceManager.ResourceType.None;
        }
    }

    public void SelectRecipe(RecipeType recipe)
    {
        CurrentRecipe = recipe;
        Debug.Log($"Выбран рецепт: {recipe}");
    }

    protected override void OnOperationalStateChanged()
    {
        isOperational = (currentWorkers >= workersRequired) && CurrentRecipe != RecipeType.None;
        Debug.Log($"Операционность перерабатывающего здания изменена: {isOperational}");
    }

    // Метод для отображения информации на панели управления
    public override string GetBuildingInfo()
    {
        return $"Перерабатывающее здание\n" +
               $"Текущий рецепт: {CurrentRecipe}\n" +
               $"Рабочие: {currentWorkers}/{workersRequired}\n" +
               $"Загрязнение: {pollutionCount * GetPollutionRate():F1}\n" +
               $"Интервал производства: {productionInterval} сек";
    }

    // Метод для настройки производственного интервала (опционально)
    public void SetProductionInterval(float interval)
    {
        productionInterval = interval;
        Debug.Log($"Интервал производства изменён на {productionInterval} секунд");
    }
}

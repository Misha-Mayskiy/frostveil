using UnityEngine;

public class CafeBuilding : Building
{
    public int foodProductionRate = 4; // Сколько порций готовится за цикл
    public int productionTimeSeconds = 10; // Время цикла производства
    private float productionTimer = 0f;

    protected override void Update()
    {
        base.Update(); // Вызов базового Update для проверки состояния здания

        if (isOperational)
        {
            productionTimer += Time.deltaTime;

            if (productionTimer >= productionTimeSeconds)
            {
                ProduceFood();
                productionTimer = 0f;
            }
        }
    }

    // Логика производства пищи
    private void ProduceFood()
    {
        int requiredHoney = 1;
        int requiredOrganicFood = 2;

        // Проверяем, хватает ли ресурсов для производства
        if (ResourceManager.Instance.RemoveResource(ResourceManager.ResourceType.Honey, requiredHoney) &&
            ResourceManager.Instance.RemoveResource(ResourceManager.ResourceType.OrganicFood, requiredOrganicFood))
        {
            ResourceManager.Instance.AddResource(ResourceManager.ResourceType.CookedFood, foodProductionRate);
        }
    }

    // Переопределение метода GetBuildingInfo для отображения информации о кафе
    public override string GetBuildingInfo()
    {
        return base.GetBuildingInfo() + $"\nПроизводит {foodProductionRate} порций пищи каждые {productionTimeSeconds} секунд.";
    }
}

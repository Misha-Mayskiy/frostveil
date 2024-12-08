using UnityEngine;

public class Greenhouse : Building
{
    public float productionInterval = 4f; // Интервал производства пищи (4 секунды)
    private float timeSinceLastProduction = 0f;

    // Этот метод вызывается в Update для проверки времени
    protected override void Update()
    {
        base.Update(); // Сначала вызываем базовую логику (если нужно)

        if (isOperational)
        {
            timeSinceLastProduction += Time.deltaTime;

            // Если время прошло, производим еду
            if (timeSinceLastProduction >= productionInterval)
            {
                ProduceOrganicFood();
                timeSinceLastProduction = 0f; // Сбрасываем таймер
            }
        }
    }

    // Метод для производства органической пищи
    private void ProduceOrganicFood()
    {
        ResourceManager.Instance.AddResource(ResourceManager.ResourceType.OrganicFood, 1); // Производим 1 единицу пищи
        Debug.Log($"Производство органической пищи: 1 единица.");
    }

    public override string GetBuildingInfo()
    {
        return base.GetBuildingInfo() + $"\nИнтервал производства пищи: {productionInterval} сек.";
    }
}

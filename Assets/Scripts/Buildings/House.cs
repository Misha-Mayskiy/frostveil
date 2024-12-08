using UnityEngine;

public class House : Building
{
    public int capacity = 10; // Вместимость здания

    // Переопределяем метод InitializeBuilding для учета вместимости
    public override void InitializeBuilding()
    {
        base.InitializeBuilding(); // Вызов базовой инициализации
        if (isOperational)
        {
            OnBuilt(); // Если здание активно, обновляем количество жильцов
        }
    }

    protected override void OnOperationalStateChanged()
    {
        base.OnOperationalStateChanged(); // Вызов базового метода при изменении состояния
        if (isOperational)
        {
            OnBuilt(); // При включении здания обновляем количество жильцов
        }
        else
        {
            OnDestroyed(); // При выключении здания уменьшаем количество жильцов
        }
    }

    // Метод, вызываемый при постройке здания
    private void OnBuilt()
    {
        ResidentManager.Instance.UpdateHousing(ResidentManager.Instance.HousedResidents + capacity);
    }

    // Метод, вызываемый при разрушении здания
    public void OnDestroyed()
    {
        ResidentManager.Instance.UpdateHousing(ResidentManager.Instance.HousedResidents - capacity);
    }

    // Переопределение метода GetBuildingInfo для отображения информации о доме
    public override string GetBuildingInfo()
    {
        return base.GetBuildingInfo() + $"\nВместимость: {capacity} жильцов.";
    }
}

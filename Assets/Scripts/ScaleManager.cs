using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    // Значения шкал
    public float pollution = 0f;
    public float tension = 0f;

    // Максимальные значения шкал
    private const float MaxPollution = 3000f;
    private const float MaxTension = 100f;

    // Скорости увеличения напряженности при разных уровнях загрязнения
    private const float LowPollutionThreshold = 0.3f;  // 30%
    private const float MediumPollutionThreshold = 0.6f; // 60%
    private const float HighPollutionThreshold = 0.9f; // 90%

    private float tensionIncreaseRate = 0f;

    private void Update()
    {
        // Обновляем загрязнение и напряженность
        UpdatePollution();
        UpdateTension();

        // Ограничиваем значения шкал
        pollution = Mathf.Clamp(pollution, 0, MaxPollution);
        tension = Mathf.Clamp(tension, 0, MaxTension);
    }

    private void UpdatePollution()
    {
        if (BuildingManager.Instance == null) return;

        // Получаем общее загрязнение от всех зданий
        pollution = BuildingManager.Instance.CalculateTotalPollution();
    }

    private void UpdateTension()
    {
        float pollutionPercentage = pollution / MaxPollution;

        // Определяем скорость увеличения напряженности
        if (pollutionPercentage > HighPollutionThreshold)
        {
            tensionIncreaseRate = 3f; // +3 каждые 20 секунд
        }
        else if (pollutionPercentage > MediumPollutionThreshold)
        {
            tensionIncreaseRate = 2f; // +2 каждые 20 секунд
        }
        else if (pollutionPercentage > LowPollutionThreshold)
        {
            tensionIncreaseRate = 1f; // +1 каждые 20 секунд
        }
        else
        {
            tensionIncreaseRate = 0f; // Нет роста напряженности
        }

        // Увеличиваем напряженность
        tension += tensionIncreaseRate * Time.deltaTime / 20f; // Разделение на 20 секунд
    }

    // Получение текущего уровня загрязнения
    public float GetPollution()
    {
        return pollution;
    }

    // Получение текущего уровня напряженности
    public float GetTension()
    {
        return tension;
    }
}

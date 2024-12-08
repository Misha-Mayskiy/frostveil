using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public event Action<int, int, int> OnDayUpdated; // Событие обновления дня (часы, минуты, дни)

    private float timeScale = 1f; // Скорость времени
    private float timeElapsed = 0f; // Прошедшее время в секундах
    private int currentHour = 0;
    private int currentMinute = 0;
    private int currentDay = 1; // Счетчик дней

    private const int DayDurationSeconds = 720; // 12 минут = 720 секунд

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime * timeScale;

        if (timeElapsed >= DayDurationSeconds)
        {
            timeElapsed -= DayDurationSeconds;
            currentDay++;
            OnDayUpdated?.Invoke(currentHour, currentMinute, currentDay);
        }

        UpdateTime();
    }

    private void UpdateTime()
    {
        int previousHour = currentHour;
        int previousMinute = currentMinute;

        float normalizedTime = timeElapsed / DayDurationSeconds;
        currentHour = Mathf.FloorToInt(normalizedTime * 24);
        currentMinute = Mathf.FloorToInt((normalizedTime * 24 - currentHour) * 60);

        if (currentHour != previousHour || currentMinute != previousMinute)
        {
            OnDayUpdated?.Invoke(currentHour, currentMinute, currentDay);

            // Событие кормления
            if (currentHour == 13 && currentMinute == 0)
            {
                ResidentManager.Instance.UpdateFeeding(FeedResidents());
            }
            else if (currentHour == 20 && currentMinute == 0)
            {
                ResidentManager.Instance.UpdateFeeding(FeedExtraResidents());
            }
        }
    }

    // Метод кормления
    private int FeedResidents()
    {
        // Получаем количество доступных порций еды
        int availableFood = ResourceManager.Instance.GetResource(ResourceManager.ResourceType.CookedFood);
        // Считаем, сколько жителей нужно накормить
        int residentsToFeed = Mathf.Min(availableFood, ResidentManager.Instance.TotalResidents);

        // Уменьшаем запасы еды
        ResourceManager.Instance.RemoveResource(ResourceManager.ResourceType.CookedFood, residentsToFeed);

        return residentsToFeed;
    }

    private int FeedExtraResidents()
    {
        // Вычисляем количество недокормленных жителей
        int unfedResidents = ResidentManager.Instance.TotalResidents - ResidentManager.Instance.FedResidents;
        // Получаем доступное количество еды
        int availableFood = ResourceManager.Instance.GetResource(ResourceManager.ResourceType.CookedFood);
        // Считаем, сколько можно дополнительно накормить
        int extraToFeed = Mathf.Min(availableFood, unfedResidents);

        // Уменьшаем запасы еды
        ResourceManager.Instance.RemoveResource(ResourceManager.ResourceType.CookedFood, extraToFeed);

        return ResidentManager.Instance.FedResidents + extraToFeed;
    }

    public void SetTimeScale(float scale)
    {
        timeScale = scale;
        Time.timeScale = scale;
        Debug.Log($"Time scale set to: {timeScale}");
    }

    public string GetFormattedTime()
    {
        return $"{currentHour:00}:{currentMinute:00}";
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }
}

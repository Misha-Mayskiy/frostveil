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
        
        // Debug.Log($"{normalizedTime}, {currentHour}, {currentMinute}");
        
        // Только если время изменилось, вызываем событие
        if (currentHour != previousHour || currentMinute != previousMinute)
        {
            OnDayUpdated?.Invoke(currentHour, currentMinute, currentDay);
        }

        // Добавить вызовы событий
        // if (currentHour == 13 && currentMinute == 0)
        // {
        //     OnFeedingTime?.Invoke(); // Кормление жителей
        // }
        // else if (currentHour == 20 && currentMinute == 0)
        // {
        //     OnExtraFeedingTime?.Invoke(); // Дополнительное кормление
        // }
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

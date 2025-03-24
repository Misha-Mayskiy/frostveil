using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Lighting References")]
    public Light directionalLight; // Основной источник света (солнце/луна)
    
    [Header("Day/Night Cycle Settings")]
    public Color dayColor = Color.white; // Цвет освещения днем
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f); // Цвет освещения ночью
    public AnimationCurve lightIntensityCurve; // Кривая интенсивности света в зависимости от времени суток
    
    [Header("Sun Settings")]
    public int sunriseHour = 6; // Час восхода солнца
    public int sunsetHour = 18; // Час заката солнца
    
    [Header("Sky Settings")]
    public Material skyboxMaterial; // Материал skybox для изменения цвета неба
    public Gradient skyColorGradient; // Градиент цвета неба в зависимости от времени
    
    [Header("Smooth Transition")]
    [Range(0.1f, 10f)]
    public float transitionSpeed = 1f; // Скорость плавного перехода (выше = быстрее)
    
    private TimeManager timeManager;
    private int currentHour;
    private int currentMinute;
    
    // Целевые значения для плавного перехода
    private Color targetLightColor;
    private float targetLightIntensity;
    private Quaternion targetLightRotation;
    private Color targetSkyColor;
    
    // Текущие значения для плавного перехода
    private Color currentLightColor;
    private float currentLightIntensity;
    private Quaternion currentLightRotation;
    private Color currentSkyColor;
    
    private void Start()
    {
        // Получаем ссылку на TimeManager
        timeManager = TimeManager.Instance;
        if (timeManager == null)
        {
            Debug.LogError("TimeManager не найден! Убедитесь, что TimeManager существует на сцене.");
            enabled = false;
            return;
        }
        
        // Подписываемся на событие обновления дня
        timeManager.OnDayUpdated += OnTimeUpdated;
        
        // Убедитесь, что у нас есть ссылка на направленный свет, если не установлен через инспектор
        if (directionalLight == null)
            directionalLight = FindMainDirectionalLight();
            
        if (skyboxMaterial == null && RenderSettings.skybox != null)
            skyboxMaterial = RenderSettings.skybox;
            
        // Инициализируем начальное состояние освещения
        UpdateCurrentTime();
        
        // Инициализируем текущие значения
        float normalizedTime = NormalizeTime(currentHour, currentMinute);
        CalculateTargetLightingValues(normalizedTime);
        
        // Устанавливаем начальные значения равными целевым
        currentLightColor = targetLightColor;
        currentLightIntensity = targetLightIntensity;
        currentLightRotation = targetLightRotation;
        currentSkyColor = targetSkyColor;
        
        // Применяем начальные настройки освещения
        ApplyLightingValues();
    }
    
    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (timeManager != null)
            timeManager.OnDayUpdated -= OnTimeUpdated;
    }
    
    private void Update()
    {
        // Плавно переходим к целевым значениям
        float t = Time.deltaTime * transitionSpeed;
        
        currentLightColor = Color.Lerp(currentLightColor, targetLightColor, t);
        currentLightIntensity = Mathf.Lerp(currentLightIntensity, targetLightIntensity, t);
        currentLightRotation = Quaternion.Slerp(currentLightRotation, targetLightRotation, t);
        currentSkyColor = Color.Lerp(currentSkyColor, targetSkyColor, t);
        
        // Применяем текущие значения
        ApplyLightingValues();
    }
    
    private void OnTimeUpdated(int hour, int minute, int day)
    {
        currentHour = hour;
        currentMinute = minute;
        
        // Вычисляем новые целевые значения
        float normalizedTime = NormalizeTime(hour, minute);
        CalculateTargetLightingValues(normalizedTime);
    }
    
    private void UpdateCurrentTime()
    {
        string timeString = timeManager.GetFormattedTime();
        string[] timeParts = timeString.Split(':');
        
        if (timeParts.Length == 2 && int.TryParse(timeParts[0], out currentHour) && int.TryParse(timeParts[1], out currentMinute))
        {
            // Время успешно получено
        }
        else
        {
            Debug.LogWarning("Не удалось получить текущее время из TimeManager.");
        }
    }
    
    private float NormalizeTime(int hour, int minute)
    {
        // Преобразуем текущее время в нормализованное (0-1)
        return (hour + minute / 60f) / 24f;
    }
    
    private void CalculateTargetLightingValues(float normalizedTime)
    {
        // Настраиваем направление света (солнце/луна)
        float lightRotation = normalizedTime * 360.0f; // Полный оборот за 24 часа
        targetLightRotation = Quaternion.Euler(new Vector3(lightRotation - 90.0f, 170.0f, 0));
        
        // Настраиваем интенсивность света по кривой
        targetLightIntensity = lightIntensityCurve.Evaluate(normalizedTime);
        
        // Настраиваем цвет света в зависимости от времени суток
        bool isDay = currentHour >= sunriseHour && currentHour < sunsetHour;
        
        // Плавный переход для рассвета и заката
        float sunriseTransitionRange = 1.0f; // Диапазон перехода в часах
        float sunsetTransitionRange = 1.0f;
        float sunriseProgress = 0;
        float sunsetProgress = 0;
        
        float currentTimeFloat = currentHour + currentMinute / 60f;
        
        // Расчет прогресса восхода
        if (currentTimeFloat >= (sunriseHour - sunriseTransitionRange) && 
            currentTimeFloat <= (sunriseHour + sunriseTransitionRange))
        {
            sunriseProgress = Mathf.InverseLerp(sunriseHour - sunriseTransitionRange, 
                                                sunriseHour + sunriseTransitionRange, 
                                                currentTimeFloat);
        }
        
        // Расчет прогресса заката
        if (currentTimeFloat >= (sunsetHour - sunsetTransitionRange) && 
            currentTimeFloat <= (sunsetHour + sunsetTransitionRange))
        {
            sunsetProgress = Mathf.InverseLerp(sunsetHour - sunsetTransitionRange, 
                                              sunsetHour + sunsetTransitionRange, 
                                              currentTimeFloat);
            sunsetProgress = 1.0f - sunsetProgress; // Инвертируем для правильного перехода
        }
        
        // Определяем цвет солнца/луны
        if (isDay)
        {
            if (sunriseProgress > 0)
                targetLightColor = Color.Lerp(nightColor, dayColor, sunriseProgress);
            else if (sunsetProgress > 0)
                targetLightColor = Color.Lerp(nightColor, dayColor, sunsetProgress);
            else
                targetLightColor = dayColor;
        }
        else
        {
            targetLightColor = nightColor;
        }
        
        // Обновляем цвет неба
        targetSkyColor = skyColorGradient.Evaluate(normalizedTime);
    }
    
    private void ApplyLightingValues()
    {
        // Применяем текущие значения к объектам сцены
        directionalLight.color = currentLightColor;
        directionalLight.intensity = currentLightIntensity;
        directionalLight.transform.rotation = currentLightRotation;
        
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetColor("_Tint", currentSkyColor);
            // При использовании стандартного skybox от Unity
            RenderSettings.ambientLight = currentSkyColor;
        }
    }
    
    private Light FindMainDirectionalLight()
    {
        // Поиск основного directional light в сцене (обычно представляет солнце)
        Light[] lights = FindObjectsOfType<Light>();
        
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
                return light;
        }
        
        // Если не найден, создаем новый
        GameObject lightObj = new GameObject("Directional Light");
        Light newLight = lightObj.AddComponent<Light>();
        newLight.type = LightType.Directional;
        
        return newLight;
    }
    
    /// <summary>
    /// Проверка, является ли текущее время днем
    /// </summary>
    public bool IsDaytime()
    {
        return currentHour >= sunriseHour && currentHour < sunsetHour;
    }
}
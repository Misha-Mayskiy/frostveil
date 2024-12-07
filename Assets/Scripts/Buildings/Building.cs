using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("Общие параметры здания")]
    public int workersRequired = 10; // Количество рабочих, необходимых для здания
    public int currentWorkers = 0;   // Текущее количество рабочих
    public string category;          // Категория здания
    public string buildingName;      // Название здания

    [Header("Параметры загрязнения")]
    private float pollutionRate = 1f;    // Уровень загрязнения
    private float pollutionCount = 0f;  // Количество загрязнения

    [Header("Состояние здания")]
    protected bool isOperational = false; // Работает ли здание

    // Публичное свойство для доступа к isOperational
    public bool IsOperational
    {
        get => isOperational;
        set
        {
            isOperational = value;
            OnOperationalStateChanged(); // Вызов события при изменении статуса
        }
    }

    protected virtual void Start()
    {
        InitializeBuilding(); // Базовая инициализация
    }

    protected virtual void Update()
    {
        if (isOperational)
        {
            Operate(); // Если здание активно, выполняем его работу
        }
    }

    public virtual void InitializeBuilding()
    {
        // Проверяем, достаточно ли рабочих для работы здания
        isOperational = currentWorkers >= workersRequired;
    }

    protected virtual void Operate()
    {
        // Логика работы здания (перекрывается в наследниках)
    }

    public virtual float GetPollutionCount()
    {
        return pollutionCount * pollutionRate;
    }

    public float GetPollutionRate()
    {
        return pollutionRate;
    }

    public void ReducePollutionRate(float rate)
    {
        pollutionRate = Mathf.Max(pollutionRate - rate, 0f); // Снижаем уровень загрязнения, не уходя в отрицательное значение
    }

    public void SetWorkers(int workers)
    {
        currentWorkers = Mathf.Clamp(workers, 0, workersRequired); // Устанавливаем количество рабочих в пределах допустимого
        InitializeBuilding(); // Проверяем состояние здания после изменения
    }

    public virtual string GetBuildingInfo()
    {
        return $"{category} Здание\nРаботники: {currentWorkers}/{workersRequired}\nАктивно: {isOperational}";
    }

    protected virtual void OnOperationalStateChanged()
    {
        // Метод, вызываемый при изменении состояния работы здания
        // Может быть перекрыт в наследниках для особого поведения
        Debug.Log($"{name}: Состояние работы изменено на {(isOperational ? "Включено" : "Выключено")}");
    }

    public void ToggleOperational(bool isOn)
    {
        IsOperational = isOn;
    }

    public void AdjustWorkers(int amount)
    {
        SetWorkers(currentWorkers + amount); // Увеличиваем или уменьшаем число рабочих
    }
}

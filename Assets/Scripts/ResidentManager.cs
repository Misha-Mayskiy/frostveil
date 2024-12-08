using UnityEngine;
using System;

public class ResidentManager : MonoBehaviour
{
    public static ResidentManager Instance;

    public int TotalResidents = 100; // Начальное количество жителей
    public int HousedResidents = 0; // Количество жителей с жильем
    public int FedResidents = 0; // Количество накормленных жителей

    public int WorkersAvailable { get; private set; } // Количество доступных рабочих
    public int WorkersAssigned { get; private set; } // Количество рабочих, уже занятых в зданиях

    private int currentDay = 0; // Текущий день

    public event Action<int, int, int> OnResidentStatusUpdated;

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

    private void Start()
    {
        UpdateWorkers(); // Инициализация рабочих на старте
    }

    public void UpdateHousing(int housed)
    {
        HousedResidents = housed;
        UpdateWorkers();
    }

    public void UpdateFeeding(int fed)
    {
        FedResidents = fed;
        UpdateWorkers();
    }

    public void UpdateDay(int day)
    {
        currentDay = day;
        UpdateWorkers();
    }

    private void UpdateWorkers()
    {
        if (currentDay <= 3)
        {
            // Первые три дня все жители могут работать
            WorkersAvailable = TotalResidents - WorkersAssigned;
        }
        else
        {
            // После третьего дня вычисляем доступных рабочих
            int homelessResidents = Mathf.Max(0, TotalResidents - HousedResidents);
            int unfedResidents = Mathf.Max(0, TotalResidents - FedResidents);

            int potentialWorkers = TotalResidents - Mathf.Max(homelessResidents, unfedResidents);
            int minimumWorkers = Mathf.CeilToInt(TotalResidents * 0.25f); // Минимум 25%

            WorkersAvailable = Mathf.Max(potentialWorkers - WorkersAssigned, minimumWorkers - WorkersAssigned);
        }

        // Обновляем статус жителей
        UpdateResidentStatus();
    }

    public bool AssignWorkers(int amount)
    {
        // Проверяем, достаточно ли доступных рабочих
        if (WorkersAvailable >= amount)
        {
            WorkersAssigned += amount;
            WorkersAvailable -= amount;
            UpdateWorkers();
            return true;
        }
        else
        {
            Debug.LogWarning("Недостаточно рабочих для выполнения задачи!");
            return false;
        }
    }

    public void UnassignWorkers(int amount)
    {
        // Уменьшаем число занятых рабочих
        WorkersAssigned = Mathf.Max(0, WorkersAssigned - amount);
        WorkersAvailable = Mathf.Min(TotalResidents, WorkersAvailable + amount);
        UpdateWorkers();
    }

    private void UpdateResidentStatus()
    {
        int homelessResidents = Mathf.Max(0, TotalResidents - HousedResidents);
        int unfedResidents = Mathf.Max(0, TotalResidents - FedResidents);

        OnResidentStatusUpdated?.Invoke(homelessResidents, unfedResidents, TotalResidents);
    }

    public int GetFreeWorkers()
    {
        return WorkersAvailable; // Свободные рабочие
    }

    public int GetAssignedWorkers()
    {
        return WorkersAssigned; // Количество занятых рабочих
    }
}

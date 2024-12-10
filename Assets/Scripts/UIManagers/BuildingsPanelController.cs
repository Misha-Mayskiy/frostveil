using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingsPanelController : MonoBehaviour
{
    [Header("UI References")]
    public Transform buildingsContainer; 
    public GameObject buildingButtonPrefab; 
    public TMP_Text descriptionText; 

    [Header("Category Buttons")]
    public Button housingFoodButton;
    public Button specializedButton;
    public Button processingButton;
    public Button extractionButton;

    [Header("Data")]
    public List<BuildingData> housingFoodBuildings;
    public List<BuildingData> specializedBuildings;
    public List<BuildingData> processingBuildings;
    public List<BuildingData> extractionBuildings;

    [Header("Placement Controller")]
    public BuildingPlacer buildingPlacer; // Ссылка на BuildingPlacer

    private void Start()
    {
        housingFoodButton.onClick.AddListener(() => PopulateBuildings(housingFoodBuildings));
        specializedButton.onClick.AddListener(() => PopulateBuildings(specializedBuildings));
        processingButton.onClick.AddListener(() => PopulateBuildings(processingBuildings));
        extractionButton.onClick.AddListener(() => PopulateBuildings(extractionBuildings));

        PopulateBuildings(housingFoodBuildings);
    }

    private void PopulateBuildings(List<BuildingData> buildings)
    {
        // Очистка предыдущих кнопок
        foreach (Transform child in buildingsContainer)
        {
            Destroy(child.gameObject);
        }

        // Создание кнопок для текущей категории
        foreach (var building in buildings)
        {
            GameObject buildingButtonObj = Instantiate(buildingButtonPrefab, buildingsContainer);
            Button buildingButton = buildingButtonObj.GetComponent<Button>();
            TMP_Text buildingText = buildingButtonObj.GetComponentInChildren<TMP_Text>();
            Image buildingImage = buildingButtonObj.GetComponentInChildren<Image>();

            buildingText.text = building.name;
            buildingImage.sprite = building.icon;

            buildingButton.onClick.AddListener(() =>
            {
                UpdateDescription(building);
                StartPlacingBuilding(building);
            });
        }
    }

    private void StartPlacingBuilding(BuildingData building)
    {
        if (building.prefab == null)
        {
            Debug.LogError($"Building '{building.name}' is missing a prefab!");
            return;
        }

        // Убедимся, что процесс размещения не активен
        if (buildingPlacer.HasActivePlacement)
        {
            Debug.LogWarning("Already placing a building! Finish or cancel the current placement first.");
            return;
        }

        // Проверка ресурсов
        if (!HasEnoughResources(building))
        {
            Debug.LogWarning($"Not enough resources to build {building.name}!");
            return;
        }

        // Передаём префаб в BuildingPlacer
        buildingPlacer.StartPlacingBuilding(building.prefab);
    }

    private bool HasEnoughResources(BuildingData building)
    {
        foreach (var cost in building.costs)
        {
            int currentAmount = ResourceManager.Instance.GetResource(cost.resourceType);
            if (currentAmount < cost.amount)
            {
                return false; // Недостаточно ресурса
            }
        }
        return true;
    }

    private void UpdateDescription(BuildingData building)
    {
        string resourceCosts = "Стоимость:\n";
        foreach (var cost in building.costs)
        {
            resourceCosts += $"{cost.resourceType}: {cost.amount}; ";
        }

        descriptionText.text = $"{resourceCosts}\n{building.description}";
    }
}


[System.Serializable]
public class BuildingData
{
    public string name;           // Название здания
    public Sprite icon;           // Иконка здания
    public string description;    // Описание здания
    public GameObject prefab;     // Префаб здания

    [Header("Стоимость постройки")]
    public List<ResourceRequirement> costs; // Список затрат ресурсов
}

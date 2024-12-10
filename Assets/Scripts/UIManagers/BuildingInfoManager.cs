using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class BuildingInfoManager : MonoBehaviour
{
    public TMP_Text buildingNameText;
    public Image buildingImage;
    public Toggle operationalToggle;
    public Button destroyButton;
    public TMP_Text workersText;
    public Button decreaseWorkersButton;
    public Button increaseWorkersButton;
    public TMP_Text resourceProductionText;
    public Image resourceIcon;
    public TMP_Text prodTitle;
    public GameObject resourcePanel; // Панель для ресурсов
    public GameObject recipeScrollPanel; // Панель для рецептов (скроллинг)

    private Building currentBuilding;

    // Словарь для иконок ресурсов
    [SerializeField] private Sprite honeySprite;
    [SerializeField] private Sprite stoneSprite;
    [SerializeField] private Sprite ironSprite;
    [SerializeField] private Sprite copperSprite;
    [SerializeField] private Sprite oilSprite;
    [SerializeField] private Sprite uraniumSprite;

    [SerializeField] private Dictionary<ThermalDrill.ResourceType, Sprite> resourceIconsDrill;
    [SerializeField] private Dictionary<Mine.ResourceType, Sprite> resourceIconsMine;
    [SerializeField] private List<BuildingIcon> buildingIcons;

    private void Awake()
    {
        Debug.Log("Initializing resourceIconsDrill and resourceIconsMine");
        resourceIconsDrill = new Dictionary<ThermalDrill.ResourceType, Sprite>
        {
            { ThermalDrill.ResourceType.Honey, honeySprite },
            { ThermalDrill.ResourceType.Stone, stoneSprite },
            { ThermalDrill.ResourceType.Iron, ironSprite },
            { ThermalDrill.ResourceType.Copper, copperSprite }
        };

        resourceIconsMine = new Dictionary<Mine.ResourceType, Sprite>
        {
            { Mine.ResourceType.Oil, oilSprite },
            { Mine.ResourceType.Uranium, uraniumSprite }
        };
        gameObject.SetActive(false);
    }

    public void SetBuilding(Building building)
    {
        Debug.Log("RAYCASTED SUCCESSFUL");
        currentBuilding = building;
        gameObject.SetActive(true);
        UpdatePanel();
    }

    private void UpdatePanel()
    {
        if (currentBuilding == null) return;

        // Установить имя здания
        buildingNameText.text = currentBuilding.buildingName;

        // Устанавливаем изображение здания из словаря
        buildingImage.sprite = GetBuildingIcon(currentBuilding.buildingName);

        // Настроить переключатель работы здания
        operationalToggle.isOn = currentBuilding.IsOperational;
        operationalToggle.onValueChanged.AddListener(ToggleBuildingState);

        // Установить текст рабочих
        workersText.text = $"{currentBuilding.currentWorkers} / {currentBuilding.workersRequired}";

        // Установить обработчики кнопок изменения рабочих
        decreaseWorkersButton.onClick.RemoveAllListeners();
        increaseWorkersButton.onClick.RemoveAllListeners();
        destroyButton.onClick.RemoveAllListeners();
        decreaseWorkersButton.onClick.AddListener(() => ChangeWorkers(-1));
        increaseWorkersButton.onClick.AddListener(() => ChangeWorkers(1));
        destroyButton.onClick.AddListener(() => DestroyBuilding());

        // Логика для добывающих зданий
        if (currentBuilding is ThermalDrill drill)
        {
            prodTitle.text = "Ресурсы";
            SetupResourcePanel(drill);
        }
        else if (currentBuilding is Mine mine)
        {
            prodTitle.text = "Ресурсы";
            SetupResourcePanel(mine); // Шахта использует ту же логику, что и бур
        }
        // Логика для производственных зданий
        else if (currentBuilding is ProcessingBuilding processor)
        {
            prodTitle.text = "Производство";
            SetupRecipePanel(processor);
        }
        else
        {
            prodTitle.text = "";
            resourcePanel.SetActive(false);
            recipeScrollPanel.SetActive(false);
        }
    }

    private void SetupResourcePanel(ThermalDrill drill)
    {
        resourcePanel.SetActive(true);
        recipeScrollPanel.SetActive(false);

        // Отображение добычи
        if (drill.IsOperational) {
            resourceProductionText.text = $"{drill.ExtractionRate} / 4s";
            SetResourceIcon(drill.resourceType);
        }
    }

    private void SetupResourcePanel(Mine mine)
    {
        resourcePanel.SetActive(true);
        recipeScrollPanel.SetActive(false);

        // Отображение добычи
        if (mine.IsOperational) {
            resourceProductionText.text = $"{mine.ExtractionRate} / 4s";
            SetResourceIcon(mine.resourceType);
        }
    }

    private void ToggleBuildingState(bool isOn)
    {
        if ((currentBuilding != null) && (currentBuilding is not House house))
        {
            currentBuilding.ToggleOperational(isOn);
        }
    }

    private void ChangeWorkers(int amount)
    {
        if ((currentBuilding != null) && (currentBuilding is not House house))
        {
            if (amount > 0)
            {
                // Проверяем, достаточно ли рабочих
                if ((currentBuilding.currentWorkers < currentBuilding.workersRequired) && ResidentManager.Instance.AssignWorkers(amount))
                {
                    currentBuilding.AdjustWorkers(amount);
                    workersText.text = $"{currentBuilding.currentWorkers} / {currentBuilding.workersRequired}";
                }
                else
                {
                    Debug.LogWarning("Недостаточно своб.рабочих либо макс.рабочих в здании!");
                }
            }
            else if (amount < 0)
            {
                // Освобождаем рабочих при уменьшении
                if (currentBuilding.currentWorkers > 0)
                {
                    int actualAmount = Mathf.Abs(amount);
                    currentBuilding.AdjustWorkers(-actualAmount);
                    ResidentManager.Instance.UnassignWorkers(actualAmount);
                    workersText.text = $"{currentBuilding.currentWorkers} / {currentBuilding.workersRequired}";
                }
            }
        }
    }

    private void SetupRecipePanel(ProcessingBuilding processor)
    {
        resourcePanel.SetActive(false);
        recipeScrollPanel.SetActive(true);

        // Удаляем предыдущие кнопки
        foreach (Transform child in recipeScrollPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Добавляем кнопку "None"
        GameObject noneButton = CreateRecipeButton("None", () => processor.SelectRecipe(ProcessingBuilding.RecipeType.None));
        noneButton.transform.SetParent(recipeScrollPanel.transform, false);

        // Добавляем кнопки для всех доступных рецептов
        foreach (ProcessingBuilding.RecipeType recipe in System.Enum.GetValues(typeof(ProcessingBuilding.RecipeType)))
        {
            if (recipe != ProcessingBuilding.RecipeType.None)
            {
                GameObject recipeButton = CreateRecipeButton(recipe.ToString(), () => processor.SelectRecipe(recipe));
                recipeButton.transform.SetParent(recipeScrollPanel.transform, false);
            }
        }
    }

    private GameObject CreateRecipeButton(string recipeName, System.Action onClick)
    {
        GameObject buttonObject = new GameObject(recipeName + "Button");
        
        // Добавляем компонент кнопки
        Button button = buttonObject.AddComponent<Button>();
        
        // Добавляем фон кнопки (например, Image)
        Image image = buttonObject.AddComponent<Image>();
        image.color = Color.gray; // Базовый цвет кнопки (измените при необходимости)

        // Добавляем текст кнопки
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        TMP_Text buttonText = textObject.AddComponent<TMP_Text>();
        buttonText.text = recipeName;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontSize = 24;

        // Настройка размеров текста
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Настройка размеров кнопки
        RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(200, 50); // Размер кнопки (измените при необходимости)

        // Привязываем действие к кнопке
        button.onClick.AddListener(() => onClick.Invoke());

        return buttonObject;
    }

    public void SetResourceIcon(ThermalDrill.ResourceType resourceType)
    {
        if (resourceIconsDrill.TryGetValue(resourceType, out Sprite icon))
        {
            resourceIcon.sprite = icon;
        }
        else
        {
            Debug.LogWarning($"Иконка для ресурса {resourceType} не найдена.");
        }
    }

        public void SetResourceIcon(Mine.ResourceType resourceType)
    {
        if (resourceIconsMine.TryGetValue(resourceType, out Sprite icon))
        {
            resourceIcon.sprite = icon;
        }
        else
        {
            Debug.LogWarning($"Иконка для ресурса {resourceType} не найдена.");
        }
    }

    private void DestroyBuilding()
    {
        if (currentBuilding != null)
        {
            // Добавление 50% стоимости здания в ресурсы
            // int refundAmount = Mathf.FloorToInt(currentBuilding.BuildCost * 0.5f);
            // ResourceManager.Instance.AddResources(refundAmount);

            ResidentManager.Instance.UnassignWorkers(currentBuilding.currentWorkers);
            if (currentBuilding is House house) {
                house.OnDestroyed();
            }

            FindObjectOfType<BuildingManager>().UnregisterBuilding(currentBuilding);

            Destroy(currentBuilding.gameObject);
            currentBuilding = null;

            gameObject.SetActive(false);
        }
    }

    private Sprite GetBuildingIcon(string buildingName)
    {
        foreach (var icon in buildingIcons)
        {
            if (icon.buildingName == buildingName)
            {
                return icon.buildingSprite;
            }
        }
        Debug.LogWarning($"Иконка для здания {buildingName} не найдена.");
        return null; // Возвращаем null, если иконка не найдена
    }
    
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        currentBuilding = null;
    }
}

[System.Serializable]
public class BuildingIcon
{
    public string buildingName; // Название здания
    public Sprite buildingSprite; // Иконка здания
}


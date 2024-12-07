using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUIManager : MonoBehaviour
{
    public GameObject resourceItemPrefab;   // Префаб элемента ресурса
    public Transform resourcePanel;         // Панель для размещения элементов
    private RectTransform panelRect;        // RectTransform панели ресурсов
    private Dictionary<ResourceManager.ResourceType, TextMeshProUGUI> resourceTexts = new Dictionary<ResourceManager.ResourceType, TextMeshProUGUI>();
    private Dictionary<ResourceManager.ResourceType, Image> resourceIcons = new Dictionary<ResourceManager.ResourceType, Image>(); // Для иконок
    public ResourceManager resourceManager;
    public Sprite[] resourceIconsArray;     // Массив иконок для ресурсов
    
    private void Start()
    {
        panelRect = resourcePanel.GetComponent<RectTransform>();

        // Проверка наличия ResourceManager
        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager is not assigned!");
            return;
        }

        // Создаем элементы для каждого ресурса
        foreach (ResourceManager.ResourceType resourceType in System.Enum.GetValues(typeof(ResourceManager.ResourceType)))
        {
            if (resourceType != ResourceManager.ResourceType.None)
            {
                CreateResourceItem(resourceType);
            }
        }

        // Обновляем интерфейс каждые 0.5 секунды
        InvokeRepeating(nameof(UpdateUI), 0, 0.5f);
        Debug.Log("InvokeRepeating for UpdateUI started.");
    }

    private void CreateResourceItem(ResourceManager.ResourceType resourceType)
    {
        // Создаем новый элемент из префаба
        GameObject resourceItem = Instantiate(resourceItemPrefab, resourcePanel);

        // Настроим иконку
        Image icon = resourceItem.GetComponentInChildren<Image>();
        if (icon != null)
        {
            icon.sprite = GetResourceIcon(resourceType); // Устанавливаем иконку для ресурса
        }
        else
        {
            Debug.LogError("No Image component found in resource item prefab!");
        }

        // Настроим текст для количества ресурса
        TextMeshProUGUI[] texts = resourceItem.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 1)
        {
            texts[0].text = "0"; // Значение ресурса (без названия)
            // Сохраняем ссылку на текст количества
            resourceTexts[resourceType] = texts[0];
            resourceIcons[resourceType] = icon;  // Сохраняем иконку в словарь
            Debug.Log("Resource item created for: " + resourceType);
        }
        else
        {
            Debug.LogError("Not enough TextMeshProUGUI components in resource item prefab!");
        }
    }

    // Метод для получения иконки для ресурса
    private Sprite GetResourceIcon(ResourceManager.ResourceType resourceType)
    {
        int index = (int)resourceType;
        if (index >= 0 && index < resourceIconsArray.Length)
        {
            return resourceIconsArray[index];
        }
        else
        {
            Debug.LogWarning("Icon not found for resource type: " + resourceType);
            return null; // Возвращаем пустую иконку, если не нашли
        }
    }

    public void UpdateUI()
    {
        if (resourceTexts.Count == 0)
        {
            return;
        }

        foreach (var resourceType in resourceTexts.Keys)
        {
            // Проверяем, что ресурс существует в ResourceManager
            int amount = resourceManager.GetResource(resourceType);
            resourceTexts[resourceType].text = amount.ToString("00000"); // Формат 00001
        }
    }
}

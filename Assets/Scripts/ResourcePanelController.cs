using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelController : MonoBehaviour
{
    public GameObject resourcePrefab; // Префаб ресурса (иконка + текст)
    public Transform panel; // Основная панель для ресурсов
    public float expandSpeed = 2f; // Скорость анимации раскрытия

    private RectTransform panelRect; // RectTransform панели
    private bool isHovered = false; // Флаг, находится ли курсор над панелью
    private float targetHeight; // Целевая высота панели

    private Dictionary<string, GameObject> resourceItems = new Dictionary<string, GameObject>(); // Список отображаемых ресурсов

    void Start()
    {
        panelRect = panel.GetComponent<RectTransform>();

        // Добавляем ресурсы (для теста)
        AddResource("Honey", 100);
        AddResource("Iron", 50);
        AddResource("Copper", 75);
        AddResource("Stone", 30);
        AddResource("Oil", 10);
        AddResource("Uranium", 5);

        // Устанавливаем начальную высоту панели
        UpdatePanelHeight();
    }

    void Update()
    {
        // Плавное изменение высоты панели
        float currentHeight = panelRect.sizeDelta.y;
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * expandSpeed));
    }

    public void OnMouseEnter()
    {
        isHovered = true;
        UpdatePanelHeight();
    }

    public void OnMouseExit()
    {
        isHovered = false;
        UpdatePanelHeight();
    }

    private void UpdatePanelHeight()
    {
        int resourceCount = resourceItems.Count;
        int visibleRows = isHovered ? Mathf.CeilToInt(resourceCount / 2f) : 2; // Показываем 2 строки в свернутом состоянии
        targetHeight = visibleRows * 50; // Высота строки (настраивается)
    }

    public void AddResource(string name, int amount)
    {
        // Проверяем, если ресурс уже существует, обновляем его значение
        if (resourceItems.ContainsKey(name))
        {
            resourceItems[name].GetComponentInChildren<Text>().text = $"{name}: {amount}";
            return;
        }

        // Создаем новый элемент ресурса
        GameObject newItem = Instantiate(resourcePrefab, panel);
        newItem.GetComponentInChildren<Text>().text = $"{name}: {amount}";

        // Добавляем в словарь
        resourceItems[name] = newItem;

        // Обновляем высоту панели
        UpdatePanelHeight();
    }

    public void UpdateResource(string name, int amount)
    {
        // Обновляем значение ресурса, если он существует
        if (resourceItems.ContainsKey(name))
        {
            resourceItems[name].GetComponentInChildren<Text>().text = $"{name}: {amount}";
        }
    }
}

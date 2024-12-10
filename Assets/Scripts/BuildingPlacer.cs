using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Placement Settings")]
    public LayerMask placementLayer; // Слой поверхности, на которую можно ставить
    public LayerMask buildingLayer; // Слой зданий для проверки столкновений
    public Material validPlacementMaterial; // Материал при допустимом размещении
    public Material invalidPlacementMaterial; // Материал при недопустимом размещении

    [Header("Radial Grid Settings")]
    public int numberOfRings = 5;
    public float ringWidth = 5f;
    public int minAllowedRing = 3;
    public Vector3 centerPosition = Vector3.zero;
    public RadialGridVisualizer gridVisualizer;

    public List<int> sectorsPerRing = new List<int>(); // Количество секторов для каждого кольца

    private GameObject currentBuilding; // Текущий объект для размещения
    private MeshRenderer[] buildingRenderers; // Все рендеры здания
    private Material[] originalMaterials; // Исходные материалы здания
    private bool canPlace = true; // Флаг, можно ли размещать здание
    private int originalLayer; // Слой объекта до проверки

    public bool HasActivePlacement => currentBuilding != null;

    private void Start()
    {
        // Инициализация списка секторов, если он не задан
        if (sectorsPerRing == null || sectorsPerRing.Count == 0)
        {
            // По умолчанию задаём 12 секторов для каждого кольца
            for (int i = 0; i < numberOfRings; i++)
            {
                sectorsPerRing.Add(12);
            }
        }
    }

    private void Update()
    {
        if (HasActivePlacement)
        {
            FollowMouse();

            // if (Input.GetKeyDown(KeyCode.R)) // R для вращения на 90 градусов
            // {
            //     currentBuilding.transform.Rotate(Vector3.up, 90f);
            // }
            if (Input.GetMouseButtonDown(0) && canPlace) // ЛКМ для размещения
            {
                PlaceBuilding();
            }
            else if (Input.GetKeyDown(KeyCode.Escape)) // ESC для отмены
            {
                CancelPlacement();
            }
        }
    }

    public void StartPlacingBuilding(GameObject buildingPrefab)
    {
        CancelPlacement();

        currentBuilding = Instantiate(buildingPrefab);
        buildingRenderers = currentBuilding.GetComponentsInChildren<MeshRenderer>();

        // Сохраняем исходный слой
        originalLayer = currentBuilding.layer;

        // Сохраняем оригинальные материалы
        originalMaterials = new Material[buildingRenderers.Length];
        for (int i = 0; i < buildingRenderers.Length; i++)
        {
            originalMaterials[i] = buildingRenderers[i].material;
        }

        // Устанавливаем временный материал
        UpdatePlacementMaterial(validPlacementMaterial);

        // Показать сетку
        if (gridVisualizer != null)
        {
            gridVisualizer.ShowGrid();
        }
    }

    private void RotateBuildingTowardsCenter()
    {
        Vector3 directionToCenter = centerPosition - currentBuilding.transform.position;
        directionToCenter.y = 0; // Игнорируем вертикальную составляющую

        if (directionToCenter != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);

            // Если нужно добавить дополнительный поворот (например, здание должно стоять боком)
            float additionalAngle = 0f; // Установите нужный угол в градусах
            Quaternion additionalRotation = Quaternion.Euler(0, additionalAngle, 0);

            currentBuilding.transform.rotation = targetRotation * additionalRotation;
        }
    }

    private void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
        {
            // Получаем позицию курсора относительно центра
            Vector3 relativePosition = hit.point - centerPosition;

            // Преобразуем в полярные координаты
            float r = new Vector2(relativePosition.x, relativePosition.z).magnitude;
            float theta = Mathf.Atan2(relativePosition.z, relativePosition.x) * Mathf.Rad2Deg;
            if (theta < 0)
                theta += 360f; // Приводим угол в диапазон [0, 360]

            // Привязываем радиус к сетке
            int ringIndex = Mathf.Clamp(Mathf.RoundToInt(r / ringWidth), 1, numberOfRings);
            float snappedRadius = ringWidth * ringIndex;

            // Получаем количество секторов для текущего кольца
            int sectorsInRing = sectorsPerRing != null && sectorsPerRing.Count >= ringIndex
                ? sectorsPerRing[ringIndex - 1]
                : 12; // По умолчанию 12 секторов

            // Привязываем угол к сетке
            float sectorAngle = 360f / sectorsInRing;
            float snappedTheta = Mathf.Round(theta / sectorAngle) * sectorAngle;

            // Преобразуем обратно в декартовы координаты
            float x = snappedRadius * Mathf.Cos(snappedTheta * Mathf.Deg2Rad);
            float z = snappedRadius * Mathf.Sin(snappedTheta * Mathf.Deg2Rad);

            Vector3 snappedPosition = new Vector3(x, 0, z) + centerPosition;

            // Устанавливаем позицию здания
            currentBuilding.transform.position = snappedPosition;

            // Поднимаем здание так, чтобы оно стояло на поверхности
            Collider buildingCollider = currentBuilding.GetComponent<Collider>();
            if (buildingCollider != null)
            {
                float heightOffset = buildingCollider.bounds.extents.y;
                currentBuilding.transform.position += Vector3.up * (heightOffset + 0.001f);
            }

            // Поворачиваем здание к центру
            RotateBuildingTowardsCenter();

            // Проверяем допустимость размещения
            if (ringIndex < minAllowedRing) // Запрещённые зоны в пределах первых двух кругов
            {
                canPlace = false;
                UpdatePlacementMaterial(invalidPlacementMaterial);
            }
            else
            {
                CheckPlacementValidity();
            }
        }
    }

    private void CheckPlacementValidity()
    {
        if (currentBuilding == null) return;

        Collider buildingCollider = currentBuilding.GetComponent<Collider>();
        if (buildingCollider == null)
        {
            Debug.LogError($"Building prefab {currentBuilding.name} is missing a collider. Placement cannot proceed.");            canPlace = false;
            UpdatePlacementMaterial(invalidPlacementMaterial);
            return;
        }

        // Временно убираем объект из слоя зданий, чтобы он не проверял себя
        int previousLayer = currentBuilding.layer;
        currentBuilding.layer = LayerMask.NameToLayer("Ignore Raycast");

        Collider[] overlaps = Physics.OverlapBox(
            buildingCollider.bounds.center,
            buildingCollider.bounds.extents,
            currentBuilding.transform.rotation,
            buildingLayer
        );

        // Возвращаем объект на исходный слой
        currentBuilding.layer = previousLayer;

        canPlace = overlaps.Length == 0;

        UpdatePlacementMaterial(canPlace ? validPlacementMaterial : invalidPlacementMaterial);
    }

    private void UpdatePlacementMaterial(Material material)
    {
        if (buildingRenderers != null && material != null)
        {
            foreach (var renderer in buildingRenderers)
            {
                renderer.material = material;
            }
        }
    }

    private void PlaceBuilding()
    {
        if (currentBuilding != null)
        {
            // Восстанавливаем оригинальные материалы
            for (int i = 0; i < buildingRenderers.Length; i++)
            {
                buildingRenderers[i].material = originalMaterials[i];
            }

            currentBuilding.layer = originalLayer; // Убедимся, что слой вернулся

            // Проверяем, является ли здание ThermalDrill и вызываем метод проверки ресурсов
            ThermalDrill drill = currentBuilding.GetComponent<ThermalDrill>();
            if (drill != null)
            {
                drill.CheckResourceZone();
            }

            Building currentB = currentBuilding.GetComponent<Building>();
            FindObjectOfType<BuildingManager>().RegisterBuilding(currentB);
            
            currentBuilding = null; // Завершаем размещение

            // Скрыть сетку
            if (gridVisualizer != null)
            {
                gridVisualizer.HideGrid();
            }
        }
    }

    public void CancelPlacement()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;

            // Скрыть сетку
            if (gridVisualizer != null)
            {
                gridVisualizer.HideGrid();
            }
        }
    }
}
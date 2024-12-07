using UnityEngine;
using System.Collections.Generic;

public class RadialGridVisualizer : MonoBehaviour
{
    public int numberOfRings = 5; // Количество колец
    public float ringWidth = 5f; // Ширина одного кольца
    public List<int> sectorsPerRing = new List<int>(); // Количество секторов для каждого кольца
    public Vector3 centerPosition = Vector3.zero; // Центр сетки

    public Material lineMaterial; // Материал для линий
    public Color startColor = new Color(0, 1, 0, 0.8f); // Начальный цвет (с прозрачностью)
    public Color endColor = new Color(0, 1, 0, 0.1f); // Конечный цвет (градиент)
    public float lineWidth = 0.05f; // Толщина линий

    public Color restrictedAreaColor = new Color(1, 0, 0, 0.5f); // Цвет для запрещённой зоны (первые 2 кольца)

    private List<GameObject> lineObjects = new List<GameObject>(); // Список созданных линий
    private bool gridVisible = false; // Статус видимости сетки

    void Awake()
    {
        // Скрываем сетку при запуске
        SetGridVisibility(false);
    }

    // Показываем сетку
    public void ShowGrid()
    {
        if (!gridVisible)
        {
            DrawGrid(); // Рисуем сетку только один раз
            SetGridVisibility(true); // Делаем её видимой
        }
    }

    // Скрываем сетку
    public void HideGrid()
    {
        if (gridVisible)
        {
            SetGridVisibility(false); // Просто отключаем видимость
        }
    }

    // Очищаем текущую сетку (для пересоздания)
    void ClearGrid()
    {
        foreach (var obj in lineObjects)
        {
            if (obj != null)
            {
                Destroy(obj); // Удаляем игровые объекты
            }
        }
        lineObjects.Clear();
    }

    // Рисуем сетку
    void DrawGrid()
    {
        // Заполняем количество секторов по умолчанию, если не задано
        if (sectorsPerRing == null || sectorsPerRing.Count == 0)
        {
            sectorsPerRing = new List<int>();
            for (int i = 0; i < numberOfRings; i++)
            {
                sectorsPerRing.Add(12); // 12 секторов по умолчанию
            }
        }

        // Рисуем кольца
        for (int ringIndex = 1; ringIndex <= numberOfRings; ringIndex++)
        {
            float radius = ringWidth * ringIndex;
            Color ringColor = ringIndex <= 2 ? restrictedAreaColor : startColor; // Разные цвета для зон
            DrawCircle(radius, ringColor);

            int sectors = sectorsPerRing[ringIndex - 1];
            float sectorAngle = 360f / sectors;

            // Рисуем линии секторов
            for (int i = 0; i < sectors; i++)
            {
                float angle = sectorAngle * i;
                DrawSectorLine(angle, radius - ringWidth, radius, ringColor);
            }
        }
    }

    // Рисуем кольцо
    void DrawCircle(float radius, Color color)
    {
        int segments = 100; // Количество сегментов для круга
        GameObject circleObj = new GameObject("Circle_" + radius);
        circleObj.transform.SetParent(transform);
        LineRenderer line = circleObj.AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startColor = color;
        line.endColor = color * new Color(1, 1, 1, 0.5f); // Градиент
        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.loop = true;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (360f / segments) * i;
            float rad = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(rad) * radius;
            float z = Mathf.Sin(rad) * radius;
            line.SetPosition(i, new Vector3(x, 0, z));
        }

        circleObj.transform.position = centerPosition;
        lineObjects.Add(circleObj);
    }

    // Рисуем линии секторов
    void DrawSectorLine(float angle, float innerRadius, float outerRadius, Color color)
    {
        GameObject lineObj = new GameObject("SectorLine_" + angle + "_R" + outerRadius);
        lineObj.transform.SetParent(transform);
        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startColor = color;
        line.endColor = color * new Color(1, 1, 1, 0.5f); // Градиент
        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;
        line.useWorldSpace = false;

        float rad = Mathf.Deg2Rad * angle;

        // Вычисляем позиции внутреннего и внешнего радиусов
        float xInner = Mathf.Cos(rad) * innerRadius;
        float zInner = Mathf.Sin(rad) * innerRadius;
        float xOuter = Mathf.Cos(rad) * outerRadius;
        float zOuter = Mathf.Sin(rad) * outerRadius;

        line.SetPosition(0, new Vector3(xInner, 0, zInner));
        line.SetPosition(1, new Vector3(xOuter, 0, zOuter));

        lineObj.transform.position = centerPosition;
        lineObjects.Add(lineObj);
    }

    // Устанавливаем видимость сетки
    public void SetGridVisibility(bool isVisible)
    {
        foreach (var obj in lineObjects)
        {
            if (obj != null)
            {
                obj.SetActive(isVisible);
            }
        }
        gridVisible = isVisible;
    }

    // Обновляем сетку (пересоздание)
    public void UpdateGrid()
    {
        ClearGrid(); // Удаляем старые линии
        if (gridVisible)
        {
            DrawGrid(); // Рисуем новые линии
        }
    }
}

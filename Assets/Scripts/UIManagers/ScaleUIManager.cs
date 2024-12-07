using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScaleUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image pollutionBar;       // Полоса прогресса загрязненности
    public Image tensionBar;         // Полоса прогресса напряженности

    [Header("Scale Values")]
    public ScaleManager scaleManager; // Ссылка на ScaleManager

    private void Update()
    {
        UpdatePollutionUI();
        UpdateTensionUI();
    }

    private void UpdatePollutionUI()
    {
        float pollution = scaleManager.GetPollution();
        float maxPollution = 5000f; // Максимальное значение загрязненности

        // Обновляем заполнение полосы
        pollutionBar.fillAmount = pollution / maxPollution;
    }

    private void UpdateTensionUI()
    {
        float tension = scaleManager.GetTension();
        float maxTension = 100f; // Максимальное значение напряженности

        // Обновляем заполнение полосы
        tensionBar.fillAmount = tension / maxTension;
    }
}

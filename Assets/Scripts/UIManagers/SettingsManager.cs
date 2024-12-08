using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    public GameObject settingsPanel; // Панель настроек
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Slider contrastSlider;

    private void Start()
    {
        // Инициализация значений из сохранений
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0);
        contrastSlider.value = PlayerPrefs.GetFloat("Contrast", 0);

        // Привязка обработчиков событий
        volumeSlider.onValueChanged.AddListener(SetVolume);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        contrastSlider.onValueChanged.AddListener(SetContrast);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; // Останавливаем игру, если настройки из меню паузы
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value); // Сохраняем громкость
        PlayerPrefs.Save(); // Сохраняем данные
    }

    private void SetBrightness(float value)
    {
        // Логика для управления яркостью
        PlayerPrefs.SetFloat("Brightness", value);
        PlayerPrefs.Save(); // Сохраняем данные
    }

    private void SetContrast(float value)
    {
        // Логика для управления контрастностью
        PlayerPrefs.SetFloat("Contrast", value);
        PlayerPrefs.Save(); // Сохраняем данные
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Панель настроек
    private bool isSettingsOpen = false; // Флаг для проверки состояния настроек

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Загружает игровую сцену
    }

    public void OpenSettings()
    {
        if (!isSettingsOpen)
        {
            settingsPanel.SetActive(true);
            isSettingsOpen = true;
        }
        else
        {
            CloseSettings();
        }
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        isSettingsOpen = false;
    }

    public void OpenStore()
    {
        Debug.Log("Открыт магазин!"); // Код для открытия магазина
        // Добавьте здесь логику магазина
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Игра завершена!"); // Вывод в редакторе для отладки
    }
}

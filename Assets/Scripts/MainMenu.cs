using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Загружает игровую сцену
    }

    public void OpenSettings()
    {
        // Здесь добавьте код для открытия настроек
    }

    public void OpenStore()
    {
        // Здесь добавьте код для открытия магазина
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

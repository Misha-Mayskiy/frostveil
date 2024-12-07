using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Панель меню паузы

    void Update()
    {
        // Если нажата клавиша ESC, переключаем видимость меню
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuUI.activeSelf)
            {
                Resume();  // Возвращаемся в игру
            }
            else
            {
                Pause();  // Останавливаем игру и показываем меню
            }
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  // Останавливаем игру
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  // Возвращаем игру в нормальный режим
    }

    public void QuitToMainMenu()
    {
        // Добавьте сюда логику для перехода в главное меню, возможно, загрузка сцены
        // SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        // Добавьте логику для сохранения игры
    }

    public void OpenSettings()
    {
        // Откроется меню настроек
    }
}

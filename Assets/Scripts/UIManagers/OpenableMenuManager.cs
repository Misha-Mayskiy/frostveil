using UnityEngine;

public class OpenableMenuManager : MonoBehaviour
{
    [Header("Общие меню")]
    public GameObject pauseMenuUI;       // Главное меню паузы
    public GameObject settingsMenuUI;   // Меню настроек

    [Header("Индивидуальные меню")]
    public GameObject buildMenu;        // Меню построек
    public GameObject researchMenu;     // Меню исследований
    public GameObject quotaMenu;        // Меню квот
    public GameObject statsMenu;        // Меню статистики

    private GameObject activeMenu = null; // Текущее активное меню
    private bool isBuildMenuOpen = false; // Флаг для управления меню построек

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var buildingPlacer = FindObjectOfType<BuildingPlacer>();
            if (buildingPlacer != null && buildingPlacer.HasActivePlacement)
            {
                buildingPlacer.CancelPlacement(); // Отменяем размещение
            }
            else if (isBuildMenuOpen)
            {
                ToggleBuildMenu();
            }
            else if (activeMenu != null)
            {
                CloseActiveMenu();
            }
            else
            {
                TogglePauseMenu();
            }
        }
    }

    /// <summary>
    /// Открыть меню паузы или вернуться в игру
    /// </summary>
    private void TogglePauseMenu()
    {
        if (pauseMenuUI.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            OpenMenu(pauseMenuUI);
            Time.timeScale = 0f; // Останавливаем игровой процесс
        }
    }

    /// <summary>
    /// Возобновить игру
    /// </summary>
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        activeMenu = null;
        Time.timeScale = 1f; // Возвращаем игровой процесс в норму
    }

    /// <summary>
    /// Открыть определенное меню
    /// </summary>
    public void OpenMenu(GameObject menu)
    {
        CloseAllMenus(); // Закрываем все открытые меню
        menu.SetActive(true);
        activeMenu = menu;
    }

    /// <summary>
    /// Закрыть текущее активное меню
    /// </summary>
    public void CloseActiveMenu()
    {
        if (activeMenu != null)
        {
            activeMenu.SetActive(false);
            activeMenu = null;
        }
    }

    /// <summary>
    /// Закрыть все меню (включая паузу)
    /// </summary>
    private void CloseAllMenus()
    {
        buildMenu.SetActive(false);
        researchMenu.SetActive(false);
        quotaMenu.SetActive(false);
        statsMenu.SetActive(false);
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        activeMenu = null;
        isBuildMenuOpen = false; // Закрываем меню построек
    }

    /// <summary>
    /// Кнопка выхода в главное меню
    /// </summary>
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Сбрасываем время в нормальный режим
        // SceneManager.LoadScene("MainMenu"); // Загрузка главного меню
    }

    /// <summary>
    /// Кнопка открытия настроек
    /// </summary>
    public void OpenSettingsMenu()
    {
        OpenMenu(settingsMenuUI);
    }

    /// <summary>
    /// Кнопка сохранения игры
    /// </summary>
    public void SaveGame()
    {
        // Добавьте свою логику сохранения
        Debug.Log("Игра сохранена!");
    }

    // --- Меню построек ---
    public void ToggleBuildMenu()
    {
        if (isBuildMenuOpen)
        {
            CloseBuildMenu();
        }
        else
        {
            OpenBuildMenu();
        }
    }

    private void OpenBuildMenu()
    {
        CloseAllMenus();
        buildMenu.SetActive(true);
        isBuildMenuOpen = true;
    }

    private void CloseBuildMenu()
    {
        buildMenu.SetActive(false);
        isBuildMenuOpen = false;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager Instance;
    public GameObject sendResourcesButton; // Кнопка отправки ресурсов
    public Animator rocketAnimator; // Аниматор для космолёта
    public bool isFirstLaunch = true; // Флаг для первой отправки

    public Camera cinematicCamera; // Позиция камеры для первой кат-сцены
    public float transitionDuration = 2f; // Длительность перехода камеры
    public Camera mainCamera; // Основная камера
    public GameObject endDemoCanvas; // Canvas для окончания демонстрации

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        sendResourcesButton.SetActive(false);
    }

    public void CheckResourcesForLaunch()
    {
        Time.timeScale = 1f;
        sendResourcesButton.SetActive(true);
    }

    public void OnSendResourcesClicked()
    {
        sendResourcesButton.SetActive(false);

        if (isFirstLaunch)
        {
            StartCoroutine(StartFirstLaunchCinematic());
            isFirstLaunch = false;
        }
        else
        {
            PlayRocketLaunchAnimation();
        }
    }

    private IEnumerator StartFirstLaunchCinematic()
    {
        // Переход на черный экран
        CutSceneTransition.Instance.ShowBlackScreen();
        yield return new WaitForSeconds(transitionDuration);

        // Переключение на камеру кат-сцены
        mainCamera.gameObject.SetActive(false);
        cinematicCamera.gameObject.SetActive(true);

        // Убираем черный экран и запускаем анимацию ракеты
        CutSceneTransition.Instance.HideBlackScreen();
        PlayRocketLaunchAnimation();

        yield return new WaitForSeconds(5f);

        // Переход к диалогу перед бурей
        StartNextDialogue("StormWarning");
    }

    private void PlayRocketLaunchAnimation()
    {
        rocketAnimator.SetTrigger("Launch");
        Debug.Log("Ракета запускается...");
    }

    private void StartNextDialogue(string dialogueName)
    {
        FindAnyObjectByType<DialogueManager>().StartDialogue(dialogueName);
    }

    public void EndDemo()
    {
        StartCoroutine(EndDemoSequence());
    }

    private IEnumerator EndDemoSequence()
    {
        // Черный экран
        CutSceneTransition.Instance.ShowBlackScreen();
        yield return new WaitForSeconds(transitionDuration);

        // Активируем Canvas для окончания демонстрации
        endDemoCanvas.SetActive(true);

        // Убираем черный экран
        CutSceneTransition.Instance.HideBlackScreen();

        yield return new WaitForSeconds(10f); // Задержка перед возвратом в меню

        // Возвращаемся в меню
        SceneManager.LoadScene("MainMenu");
    }
}

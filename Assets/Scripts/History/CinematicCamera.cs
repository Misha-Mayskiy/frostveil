using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CinematicCamera : MonoBehaviour
{
    public Transform startPoint; // Начальная позиция камеры
    public Transform targetPoint; // Конечная позиция камеры
    public float duration = 5f; // Длительность движения камеры
    public float pauseAtEnd = 3f; // Пауза перед затемнением

    public Camera mainCamera; // Основная камера
    public GameObject uiCanvas; // Канвас для UI
    public Image fadeOverlay; // UI-элемент для затемнения (черный Image с Alpha)
    public GameObject CinematicCanvas;

    private float elapsedTime = 0f;
    private bool isPlaying = false;
    public CutSceneTransition cutSceneTrans;

    private void Start()
    {
        // Настраиваем начальную позицию
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }

        // Отключаем Main Camera и UI перед началом кат-сцены
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        if (uiCanvas != null)
            uiCanvas.SetActive(false);

        // Запускаем кат-сцену
        StartCinematic();
    }

    private void Update()
    {
        if (isPlaying)
        {
            PlayCinematic();
        }
    }

    public void StartCinematic()
    {
        elapsedTime = 0f;
        isPlaying = true;
        gameObject.SetActive(true); // Включаем кат-сценическую камеру
    }

    private void PlayCinematic()
    {
        if (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Плавное замедление камеры под конец
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);

            // Интерполяция позиции и вращения
            transform.position = Vector3.Lerp(startPoint.position, targetPoint.position, t);
            transform.rotation = Quaternion.Lerp(startPoint.rotation, targetPoint.rotation, t);
        }
        else
        {
            // Пауза перед затемнением
            StartCoroutine(EndCinematicWithFade());
            isPlaying = false;
        }
    }

    private IEnumerator EndCinematicWithFade()
    {
        // Пауза перед затемнением
        yield return new WaitForSeconds(pauseAtEnd);

        cutSceneTrans.ShowBlackScreen();

        yield return new WaitForSeconds(3f);

        // Завершаем кат-сцену
        OnCinematicEnd();
    }

    private void OnCinematicEnd()
    {
        // Включаем Main Camera и UI
        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        cutSceneTrans.HideBlackScreen();

        // Выключаем кат-сценическую камеру
        gameObject.SetActive(false);

        // Начинаем диалог с Михалычем
        FindObjectOfType<DialogueManager>().StartDialogue("IntroDialogue"); // Пример вызова начала диалога
    }
}

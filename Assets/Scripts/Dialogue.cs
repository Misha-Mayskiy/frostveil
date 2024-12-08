using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class Dialogue
    {
        public string characterName; // Имя персонажа
        public string message;       // Сообщение
    }

    public GameObject dialoguePanel; // Панель диалога
    public TMP_Text characterNameText;
    public TMP_Text dialogueText;
    public TMP_Text clickIndicatorText; // Ссылка на текст "Кликните, чтобы продолжить"

    public Dialogue[] dialogues; // Массив диалогов

    private int currentIndex = 0;
    private Coroutine indicatorCoroutine;

    private void Start()
    {
        dialoguePanel.SetActive(true);
        ShowDialogue();
    }

    private void Update()
    {
        // Переход к следующему диалогу при клике мыши
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            NextDialogue();
        }
    }

    private void ShowDialogue()
    {
        characterNameText.text = dialogues[currentIndex].characterName;
        dialogueText.text = dialogues[currentIndex].message;

        // Запуск мигающего индикатора
        if (indicatorCoroutine != null)
            StopCoroutine(indicatorCoroutine);

        clickIndicatorText.gameObject.SetActive(true);
        indicatorCoroutine = StartCoroutine(BlinkIndicator());
    }

    private void NextDialogue()
    {
        currentIndex++;
        clickIndicatorText.gameObject.SetActive(false); // Скрываем индикатор перед переходом
        if (indicatorCoroutine != null)
            StopCoroutine(indicatorCoroutine);

        if (currentIndex < dialogues.Length)
        {
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        // Запуск обучения
        // FindObjectOfType<TutorialManager>()?.StartTutorial();
    }

    private IEnumerator BlinkIndicator()
    {
        while (true)
        {
            clickIndicatorText.alpha = 1f; // Показать текст
            yield return new WaitForSeconds(0.5f);
            clickIndicatorText.alpha = 0f; // Скрыть текст
            yield return new WaitForSeconds(0.5f);
        }
    }
}

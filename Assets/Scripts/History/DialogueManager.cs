using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject uiCanvas;
    public GameObject dialoguePanel; // Панель для диалога
    public TMP_Text dialogueText; // Текст диалога
    public TMP_Text characterNameText; // Имя говорящего
    public Image characterImage; // Изображение персонажа

    public Sprite alexSprite; // Спрайт для Алекса
    public Sprite mihalychSprite; // Спрайт для Михалыча

    private Queue<DialogueLine> dialogueLines; // Очередь реплик
    private bool isDialogueActive = false; // Идёт ли диалог
    private string currentDialogueName;

    private void Start()
    {
        dialogueLines = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0)) // Проверка клика мыши
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(string fileName)
    {
        Time.timeScale = 1f;
        currentDialogueName = fileName;
        dialogueLines.Clear();

        // Загрузка диалога из текстового файла
        string[] lines = LoadDialogueFile(fileName);

        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length >= 2)
            {
                string character = parts[0].Trim();
                string text = parts[1].Trim();
                dialogueLines.Enqueue(new DialogueLine(character, text));
            }
        }

        Time.timeScale = 1f;
        uiCanvas.SetActive(false);
        dialoguePanel.SetActive(true);
        isDialogueActive = true;
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines.Dequeue();
        characterNameText.text = line.Character;

        // Устанавливаем спрайт персонажа
        if (line.Character == "АЛЕКС")
        {
            characterImage.sprite = alexSprite;
        }
        else if (line.Character == "МИХАЛЫЧ")
        {
            characterImage.sprite = mihalychSprite;
        }

        dialogueText.text = line.Text;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        uiCanvas.SetActive(true);
        isDialogueActive = false;

        // Проверяем, нужно ли запускать квест после завершения конкретного диалога
        CurrentDialogueTriggersQuest(currentDialogueName);

        Debug.Log("Диалог завершён.");
    }

    // Метод для проверки, запускает ли диалог квест
    private void CurrentDialogueTriggersQuest(string dialogueName)
    {
        if (dialogueName == "IntroDialogue")
        {
            QuestManager.Instance.StartQuest("NewHome");
        }
        else if (dialogueName == "ContinueTutorial")
        {
            QuestManager.Instance.StartQuest("GatherResources"); // Здесь запускается ваш квест "GatherResources"
        }
        else if (dialogueName == "StormWarning")
        {
            FindAnyObjectByType<PlatformManager>().EndDemo();
        }
    }

    private string[] LoadDialogueFile(string fileName)
    {
        TextAsset textFile = Resources.Load<TextAsset>($"Dialogues/{fileName}");
        if (textFile == null)
        {
            Debug.LogError($"Файл диалога {fileName} не найден!");
            return new string[0];
        }
        return textFile.text.Split('\n');
    }
}

public class DialogueLine
{
    public string Character { get; }
    public string Text { get; }

    public DialogueLine(string character, string text)
    {
        Character = character;
        Text = text;
    }
}

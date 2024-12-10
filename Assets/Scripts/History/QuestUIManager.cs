using UnityEngine;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance;

    [Header("UI Elements")]
    public GameObject questPanel; // Панель квеста
    public TMP_Text questTitleText; // Заголовок квеста
    public TMP_Text questDescriptionText; // Текущий пункт квеста

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

    public void ShowQuest(string title, string description)
    {
        if (questPanel != null) questPanel.SetActive(true);

        if (questTitleText != null) questTitleText.text = title;
        if (questDescriptionText != null) questDescriptionText.text = description;
    }

    public void UpdateDescription(string description)
    {
        if (questDescriptionText != null) questDescriptionText.text = description;
    }

    public void HideQuest()
    {
        if (questPanel != null) questPanel.SetActive(false);
    }
}

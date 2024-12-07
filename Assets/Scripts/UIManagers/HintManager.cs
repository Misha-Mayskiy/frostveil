using UnityEngine;
using TMPro;

public class HintsManager : MonoBehaviour
{
    public TMP_Text hintsText; // Текст в Scroll View для подсказок

    private string hints = "";

    public void AddHint(string newHint)
    {
        hints += newHint + "\n"; // Добавляем новую строку
        hintsText.text = hints;
    }

    public void ClearHints()
    {
        hints = ""; // Очищаем подсказки
        hintsText.text = hints;
    }
}

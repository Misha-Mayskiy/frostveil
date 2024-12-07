using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text dayText; // Оставляем управление днем здесь

    private void Start()
    {
        TimeManager.Instance.OnDayUpdated += UpdateTimeUI;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnDayUpdated -= UpdateTimeUI;
    }

    private void UpdateTimeUI(int hour, int minute, int day)
    {
        Debug.Log($"Time Updated: {hour}:{minute}, Day: {day}");
        timeText.text = $"{hour:00}:{minute:00}";
        dayText.text = $"День: {day}";
    }
}

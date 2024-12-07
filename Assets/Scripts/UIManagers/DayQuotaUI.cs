using UnityEngine;
using TMPro;

public class DayAndQuotaUI : MonoBehaviour
{
    public TMP_Text dayText;
    public TMP_Text quotaText;

    private void Start()
    {
        TimeManager.Instance.OnDayUpdated += UpdateDayUI;
    }

    private void UpdateDayUI(int hour, int minute, int day)
    {
        dayText.text = $"День: {day}";

        int daysUntilQuota = QuotaManager.Instance.GetDaysUntilNextQuota(day);
        quotaText.text = $"Дней до сдачи квоты: {daysUntilQuota}";
    }
}

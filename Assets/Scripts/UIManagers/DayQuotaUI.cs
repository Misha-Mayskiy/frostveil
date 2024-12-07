using UnityEngine;
using TMPro;

public class DayAndQuotaUI : MonoBehaviour
{
    public TMP_Text quotaText;

    private void Start()
    {
        TimeManager.Instance.OnDayUpdated += UpdateQuotaUI;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.OnDayUpdated -= UpdateQuotaUI;
    }

    private void UpdateQuotaUI(int hour, int minute, int day)
    {
        int daysUntilQuota = QuotaManager.Instance.GetDaysUntilNextQuota(day);
        quotaText.text = $"Дней до окончания задания: {daysUntilQuota}";
    }
}

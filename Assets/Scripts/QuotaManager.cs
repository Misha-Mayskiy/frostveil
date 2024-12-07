using UnityEngine;
using System; // Необходим для Action

public class QuotaManager : MonoBehaviour
{
    public static QuotaManager Instance;

    private int quotaInterval = 7;
    private int nextQuotaDay = 7;

    public event Action OnQuotaDeadline;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TimeManager.Instance.OnDayUpdated += CheckQuotaDeadline;
    }

    private void CheckQuotaDeadline(int hour, int minute, int day)
    {
        if (day >= nextQuotaDay)
        {
            nextQuotaDay += quotaInterval;
            OnQuotaDeadline?.Invoke();
        }
    }

    public int GetDaysUntilNextQuota(int currentDay)
    {
        return nextQuotaDay - currentDay;
    }
}

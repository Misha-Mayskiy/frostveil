using UnityEngine;
using TMPro;

public class ResidentUI : MonoBehaviour
{
    public TMP_Text totalResidentsText;
    public TMP_Text homelessResidentsText;
    public TMP_Text unfedResidentsText;
    // public TMP_Text freeWorkersText;     // Новое поле для свободных рабочих
    public TMP_Text assignedWorkersText; // Новое поле для занятых рабочих

    private void Start()
    {
        ResidentManager.Instance.OnResidentStatusUpdated += UpdateResidentUI;
    }

    private void OnDestroy()
    {
        ResidentManager.Instance.OnResidentStatusUpdated -= UpdateResidentUI;
    }

    private void UpdateResidentUI(int homeless, int unfed, int total)
    {
        totalResidentsText.text = $"Всего жителей: {total}";

        homelessResidentsText.text = homeless > 0 
            ? $"Нет жилья: {homeless}/{total}" 
            : "";

        unfedResidentsText.text = unfed > 0 
            ? $"Нехватило порций: {unfed}/{total}" 
            : "";

        // Обновляем информацию о рабочих
        int freeWorkers = ResidentManager.Instance.GetFreeWorkers();
        int assignedWorkers = ResidentManager.Instance.GetAssignedWorkers();

        assignedWorkersText.text = $"Занятый персонал: {assignedWorkers}/{freeWorkers + assignedWorkers}";
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public PlatformManager platformManager;

    private bool isQuestActive = false;
    private string currentQuestName;

    private Dictionary<ResourceManager.ResourceType, int> requiredResources;
    private Dictionary<ResourceManager.ResourceType, int> collectedResources;

    private Dictionary<ResourceManager.ResourceType, string> resourceNames = new Dictionary<ResourceManager.ResourceType, string>
    {
        { ResourceManager.ResourceType.Honey, "Мед" },
        { ResourceManager.ResourceType.Uranium, "Уран" },
        { ResourceManager.ResourceType.Iron, "Железо" },
        { ResourceManager.ResourceType.Stone, "Камень" },
        { ResourceManager.ResourceType.OrganicFood, "Органическая еда" },
        { ResourceManager.ResourceType.CookedFood, "Приготовленная еда" }
        // Добавьте остальные ресурсы, если нужно
    };

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

    public void StartQuest(string questName)
    {
        currentQuestName = questName;
        isQuestActive = true;

        if (questName == "NewHome")
        {
            StartNewHomeQuest();
        }
        else if (questName == "GatherResources")
        {
            StartGatherResourcesQuest();
        }
    }

    private void StartNewHomeQuest()
    {

        QuestUIManager.Instance.ShowQuest(
            "Квест: Новый дом",
            "Соберите 25 камня для строительства дома."
        );

        StartCoroutine(CheckResourcesRoutineForHome());
    }

    private IEnumerator CheckResourcesRoutineForHome()
    {
        while (isQuestActive)
        {
            if (ResourceManager.Instance.GetResource(ResourceManager.ResourceType.Stone) >= 25)
            {
                QuestUIManager.Instance.UpdateDescription("Постройте дом первого уровня.");
                StartCoroutine(CheckBuildingRoutineForHome());
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator CheckBuildingRoutineForHome()
    {
        ResidentManager residentManager = FindObjectOfType<ResidentManager>();
        int currentHoused = residentManager.HousedResidents;

        while (isQuestActive)
        {
            if (currentHoused < residentManager.HousedResidents)
            {
                CompleteQuest();
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void StartGatherResourcesQuest()
    {
        requiredResources = new Dictionary<ResourceManager.ResourceType, int>
        {
            { ResourceManager.ResourceType.Honey, 150 },
            { ResourceManager.ResourceType.Uranium, 20 },
            { ResourceManager.ResourceType.Iron, 100 }
        };

        collectedResources = new Dictionary<ResourceManager.ResourceType, int>
        {
            { ResourceManager.ResourceType.Honey, 0 },
            { ResourceManager.ResourceType.Uranium, 0 },
            { ResourceManager.ResourceType.Iron, 0 }
        };

        QuestUIManager.Instance.ShowQuest(
            "Квест: Дар Земли",
            "Соберите ресурсы:\n" + GetFormattedResourceList()
        );

        StartCoroutine(CheckResourcesRoutineForGathering());
    }

    private IEnumerator CheckResourcesRoutineForGathering()
    {
        while (isQuestActive)
        {
            foreach (var resource in requiredResources)
            {
                int currentAmount = ResourceManager.Instance.GetResource(resource.Key);
                collectedResources[resource.Key] = Mathf.Min(currentAmount, resource.Value);
            }

            UpdateQuestUI();

            if (IsQuestComplete("GatherResources"))
            {
                CompleteQuest();
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateQuestUI()
    {
        string description = "Соберите ресурсы:\n" + GetFormattedResourceList();
        QuestUIManager.Instance.UpdateDescription(description);
    }

    private string GetFormattedResourceList()
    {
        string formattedList = "";
        foreach (var resource in requiredResources)
        {
            string resourceName = resourceNames.ContainsKey(resource.Key) ? resourceNames[resource.Key] : resource.Key.ToString();
            formattedList += $"- {resourceName}: {collectedResources[resource.Key]} / {resource.Value}\n";
        }
        return formattedList;
    }

    public bool IsQuestComplete(string questName)
    {
        // Проверяем, завершён ли квест
        if (questName == "GatherResources")
        {
            return IsGatherResourcesComplete();
        }
        return false;
    }

    private bool IsGatherResourcesComplete()
    {
        foreach (var resource in requiredResources)
        {
            if (collectedResources[resource.Key] < resource.Value)
            {
                return false;
            }
        }

        platformManager.CheckResourcesForLaunch();
        return true;
    }

    private void CompleteQuest()
    {
        isQuestActive = false;
        QuestUIManager.Instance.HideQuest();

        if (currentQuestName == "NewHome")
        {
            FindObjectOfType<DialogueManager>().StartDialogue("ContinueTutorial");
        }

        Debug.Log($"Квест {currentQuestName} завершён!");
    }
}

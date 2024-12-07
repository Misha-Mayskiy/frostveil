using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingDescriptionHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject descriptionPanel;
    private Text descriptionText;
    private Text buildingNameText;
    private string description;
    private string nameBuilding;

    public void Setup(GameObject panel, string buildingName, string buildingDescription)
    {
        descriptionPanel = panel;
        buildingNameText = panel.transform.Find("BuildingNameText").GetComponent<Text>();
        descriptionText = panel.transform.Find("DescriptionText").GetComponent<Text>();
        description = buildingDescription;
        nameBuilding = buildingName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.SetActive(true);
        buildingNameText.text = nameBuilding;
        descriptionText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.SetActive(false);
    }
}

using UnityEngine;

public class BuildingClickHandler : MonoBehaviour
{
    public BuildingInfoManager buildingInfoManager; // Ссылка на BuildingInfoManager
    public GameObject uiCanvas;
    public GameObject mainCamera;

    private void Update()
    {
        if (uiCanvas.activeSelf && mainCamera.activeSelf && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int buildingLayer = LayerMask.GetMask("Building");
            Debug.Log("RAYCASTING TO SOMETHING");
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildingLayer))
            {
                var building = hit.collider.GetComponent<Building>();
                if (building != null)
                {
                    // Передаём здание в BuildingInfoManager
                    buildingInfoManager.SetBuilding(building);
                }
            }
        }
    }
}
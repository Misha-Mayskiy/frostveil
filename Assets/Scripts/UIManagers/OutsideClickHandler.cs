using UnityEngine;

public class OutsideClickHandler : MonoBehaviour
{
    public BuildingInfoManager buildingInfoManager;

    private bool isPointerOverPanel = false;

    // Этот метод вызывается, если курсор находится над панелью
    public void OnPointerEnter()
    {
        isPointerOverPanel = true;
    }

    // Этот метод вызывается, если курсор уходит с панели
    public void OnPointerExit()
    {
        isPointerOverPanel = false;
    }

    private void Update()
    {
        // Проверяем клик мыши
        if (Input.GetMouseButtonDown(0))
        {
            // Закрываем панель, только если клик произошёл не на панели
            if (!isPointerOverPanel)
            {
                buildingInfoManager.ClosePanel();
            }
        }
    }
}

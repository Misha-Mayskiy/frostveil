using UnityEngine;
using TMPro;

public class FoodUI : MonoBehaviour
{
    public TMP_Text cookedFoodText;

    private void Update()
    {
        int cookedFood = ResourceManager.Instance.GetResource(ResourceManager.ResourceType.CookedFood);
        cookedFoodText.text = $"Порции еды: {cookedFood}";
    }
}

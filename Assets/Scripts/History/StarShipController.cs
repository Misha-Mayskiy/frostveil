using UnityEngine;

public class RocketController : MonoBehaviour
{
    public Animator rocketAnimator; // Аниматор для космолёта

    private void Start()
    {
        if (rocketAnimator == null)
        {
            rocketAnimator = GetComponent<Animator>();
        }
    }

    // Метод для обработки события завершения анимации
    public void OnLaunchComplete()
    {
        Debug.Log("Анимация завершена");
        // Отключаем анимацию или выполняем другие действия
        gameObject.SetActive(false);
        rocketAnimator.enabled = false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CutSceneTransition : MonoBehaviour
{
    public static CutSceneTransition Instance;
    public GameObject cinematicCanvas;
    public Image fadeOverlay; // UI-элемент для затемнения (черный Image с Alpha)

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

    public void ShowBlackScreen()
    {
        cinematicCanvas.SetActive(true);
        StartCoroutine(Fade(1f));
    }

    public void HideBlackScreen()
    {
        StartCoroutine(Fade(0f));
        cinematicCanvas.SetActive(false);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float elapsedTime = 0f;
        float startAlpha = fadeOverlay.color.a;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime);
            fadeOverlay.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadeOverlay.color = new Color(0, 0, 0, targetAlpha);
    }
}

using UnityEngine;
using System.Collections;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Убедитесь, что объект не уничтожается при загрузке новой сцены
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Coroutine StartCoroutineStatic(IEnumerator coroutine)
    {
        return instance.StartCoroutine(coroutine);
    }
}

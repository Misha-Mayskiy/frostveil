using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Центр, вокруг которого будет вращаться камера
    public float rotationSpeed = 5f; // Скорость вращения камеры
    public float[] zoomLevels = { 5f, 10f, 15f }; // Уровни приближения
    // public float moveSpeed = 5f; // Скорость перемещения камеры
    public float zoomSpeed = 5f; // Скорость изменения уровня приближения
    // public float minZoomDistance = 2f; // Минимальное расстояние зума
    // public float maxZoomDistance = 20f; // Максимальное расстояние зума
    // public float moveSensitivity = 0.1f; // Чувствительность перемещения мыши

    private int currentZoomLevel = 3; // Текущий уровень приближения
    // private bool isRotating = false; // Флаг для отслеживания вращения
    // private bool isMoving = false; // Флаг для отслеживания перемещения
    private float currentZoomDistance; // Текущее расстояние зума
    private Plane movementPlane; // Плоскость для перемещения камеры

    private void Start()
    {
        currentZoomDistance = zoomLevels[currentZoomLevel];
        movementPlane = new Plane(Vector3.up, target.position);
    }

    private void Update()
    {
        HandleRotation();
        HandleZoom();
        // HandleMovement();
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(target.position, Vector3.up, -rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            if (scroll > 0)
            {
                currentZoomLevel = Mathf.Clamp(currentZoomLevel - 1, 0, zoomLevels.Length - 1);
            }
            else
            {
                currentZoomLevel = Mathf.Clamp(currentZoomLevel + 1, 0, zoomLevels.Length - 1);
            }
            currentZoomDistance = zoomLevels[currentZoomLevel];
        }

        if (Input.GetKeyDown(KeyCode.Z)) // Приближение
        {
            currentZoomLevel = Mathf.Clamp(currentZoomLevel - 1, 0, zoomLevels.Length - 1);
            currentZoomDistance = zoomLevels[currentZoomLevel];
        }

        if (Input.GetKeyDown(KeyCode.X)) // Отдаление
        {
            currentZoomLevel = Mathf.Clamp(currentZoomLevel + 1, 0, zoomLevels.Length - 1);
            currentZoomDistance = zoomLevels[currentZoomLevel];
        }

        Vector3 targetPosition = target.position - transform.forward * currentZoomDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomSpeed);
    }

    // private void HandleMovement()
    // {
    //     if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
    //     {
    //         isMoving = true;
    //     }
    //     else
    //     {
    //         isMoving = false;
    //     }

    //     if (isMoving)
    //     {
    //         Vector3 moveDirection = Vector3.zero;

    //         if (Input.GetKey(KeyCode.W))
    //         {
    //             moveDirection += transform.forward;
    //         }
    //         if (Input.GetKey(KeyCode.S))
    //         {
    //             moveDirection -= transform.forward;
    //         }
    //         if (Input.GetKey(KeyCode.A))
    //         {
    //             moveDirection -= transform.right;
    //         }
    //         if (Input.GetKey(KeyCode.D))
    //         {
    //             moveDirection += transform.right;
    //         }

    //         if (Input.GetMouseButton(1))
    //         {
    //             // Привязка перемещения к плоскости
    //             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //             if (movementPlane.Raycast(ray, out float enter))
    //             {
    //                 Vector3 hitPoint = ray.GetPoint(enter);
    //                 Vector3 targetPosition = hitPoint + moveDirection * moveSpeed * moveSensitivity * Time.deltaTime;
    //                 transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
    //             }
    //         }
    //         else
    //         {
    //             // Перемещение без привязки к плоскости
    //             transform.position += moveDirection * moveSpeed * Time.deltaTime;
    //         }
    //     }
    // }
}

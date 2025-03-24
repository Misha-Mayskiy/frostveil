using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // Центр, вокруг которого будет вращаться камера
    public float rotationSpeed = 5f; // Скорость вращения камеры
    public float[] zoomLevels = { 5f, 10f, 15f }; // Уровни приближения
    public float zoomSpeed = 5f; // Скорость изменения уровня приближения
    public float moveSpeed = 5f; // Скорость перемещения камеры
    public float fastMoveSpeed = 10f; // Скорость перемещения камеры при удержании Shift
    public float mouseSensitivity = 2f; // Чувствительность мыши

    private int currentZoomLevel = 3; // Текущий уровень приближения
    private float currentZoomDistance; // Текущее расстояние зума
    private bool isFreeMode = false; // Флаг для переключения в свободный режим

    private void Start()
    {
        currentZoomDistance = zoomLevels[currentZoomLevel];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFreeMode = !isFreeMode;
            if (isFreeMode)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (isFreeMode)
        {
            HandleFreeMode();
        }
        else
        {
            HandleRotation();
            HandleZoom();
        }
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

    private void HandleFreeMode()
    {
        // Обработка вращения камеры мышкой
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up, mouseX);
        transform.Rotate(Vector3.right, -mouseY);

        // Обработка перемещения камеры клавишами WASD
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }

        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;
        transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;
    }
}

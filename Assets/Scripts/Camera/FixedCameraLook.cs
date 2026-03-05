using UnityEngine;

public class FixedCameraLook : MonoBehaviour
{
    [Header("Sensitivity")]
    public float mouseSensitivity = 150f;

    [Header("Vertical Limits")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 30f;

    [Header("Horizontal Limits")]
    public float minHorizontalAngle = -60f;
    public float maxHorizontalAngle = 60f;

    public Transform cameraTransform;

    float verticalRotation = 0f;
    float horizontalRotation = 0f;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;

        horizontalRotation = Mathf.Clamp(horizontalRotation, minHorizontalAngle, maxHorizontalAngle);
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // Rotación horizontal (cuerpo)
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Rotación vertical (cabeza) - La cámara representa la cabeza. El movimiento vertical solo afecta a la cámara.
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
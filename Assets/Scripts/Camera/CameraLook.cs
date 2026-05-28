using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float touchSensitivity = 0.2f;
    [SerializeField] private float gyroSensitivity = 1f;
    [SerializeField] private float verticalClamp = 80f; // límite de mirar arriba/abajo

    private float rotationX; // rotación vertical acumulada
    private float rotationY; // rotación horizontal acumulada
    private Gyroscope gyro;

    private void Start()
    {
        // Inicializa la rotación con la rotación actual de la cámara
        rotationX = transform.eulerAngles.x;
        rotationY = transform.eulerAngles.y;

        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
        }
    }

    private void Update()
    {
        int controlMode = SettingsManager.Instance.CurrentSettings.controlMode;

        if (controlMode == 0)
            HandleTouch();
        else
            HandleGyro();
    }

    private void HandleTouch()
    {
        #if UNITY_EDITOR //Codigo que solo se ejecuta en Unity para facilitar las pruebas
                if (Input.GetMouseButton(0))
                {
                    rotationY += Input.GetAxis("Mouse X") * touchSensitivity * 10f;
                    rotationX -= Input.GetAxis("Mouse Y") * touchSensitivity * 10f;
                    rotationX = Mathf.Clamp(rotationX, -verticalClamp, verticalClamp);
                    transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
                    return;
                }
        #endif

        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Moved) return;

        rotationY += touch.deltaPosition.x * touchSensitivity;
        rotationX -= touch.deltaPosition.y * touchSensitivity;
        rotationX = Mathf.Clamp(rotationX, -verticalClamp, verticalClamp);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    private void HandleGyro()
    {
        if (!SystemInfo.supportsGyroscope) return;

        // Convierte la rotación del giroscopio al sistema de coordenadas de Unity
        Quaternion gyroRotation = new Quaternion(
             gyro.attitude.x,
             gyro.attitude.y,
            -gyro.attitude.z,
            -gyro.attitude.w
        );

        // Corrección de ejes para que funcione en orientación horizontal
        Quaternion correction = Quaternion.Euler(90f, 0f, 0f);
        Quaternion target = correction * gyroRotation;

        // Sustituyes la línea anterior por esta
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * gyroSensitivity);
    }

    public void ChangeCameraInput()
    {
        SettingsManager.Instance.CurrentSettings.controlMode = SettingsManager.Instance.CurrentSettings.controlMode == 0 ? 1 : 0;
    }
}

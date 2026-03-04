using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    float xRotation = 0f;

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (transform.parent != null)
            transform.parent.Rotate(Vector3.up * mouseX);
    }
}
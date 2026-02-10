using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A / D
        float v = Input.GetAxis("Vertical");   // W / S

        Vector3 direction = transform.right * h + transform.forward * v;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}

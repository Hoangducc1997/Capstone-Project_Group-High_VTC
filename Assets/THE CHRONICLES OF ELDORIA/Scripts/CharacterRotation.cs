using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    public Transform character;   // Kéo thả nhân vật vào đây
    public float rotationSpeed = 5f;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Khi nhấn giữ chuột trái
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0)) // Khi giữ chuột trái và kéo
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationY = delta.x * rotationSpeed * Time.deltaTime;
            character.Rotate(Vector3.up, -rotationY, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }
}

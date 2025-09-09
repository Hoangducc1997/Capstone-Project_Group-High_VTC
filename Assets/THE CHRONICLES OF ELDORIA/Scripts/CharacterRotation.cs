using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    public Transform character;        // Kéo thả nhân vật vào đây
    public float dragRotationSpeed = 5f;    // Tốc độ xoay khi kéo chuột
    public float returnRotationSpeed = 2f;  // Tốc độ quay về gốc

    private Vector3 lastMousePosition;
    private bool returnToOrigin = false;
    private Quaternion originalRotation; // rotation ban đầu

    void Start()
    {
        // Lưu lại rotation ban đầu của nhân vật khi game bắt đầu
        originalRotation = character.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Khi nhấn giữ chuột trái
        {
            lastMousePosition = Input.mousePosition;
            returnToOrigin = false; // Ngừng quay về gốc nếu người chơi xoay lại
        }
        else if (Input.GetMouseButton(0)) // Khi giữ chuột trái và kéo
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationY = delta.x * dragRotationSpeed * Time.deltaTime;
            character.Rotate(Vector3.up, -rotationY, Space.World);

            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // Khi thả chuột
        {
            returnToOrigin = true;
        }

        // Xử lý quay về rotation ban đầu
        if (returnToOrigin)
        {
            character.rotation = Quaternion.Slerp(
                character.rotation,
                originalRotation,
                Time.deltaTime * returnRotationSpeed
            );

            // Khi đã gần bằng rotation gốc thì dừng lại
            if (Quaternion.Angle(character.rotation, originalRotation) < 0.1f)
            {
                character.rotation = originalRotation;
                returnToOrigin = false;
            }
        }
    }
}

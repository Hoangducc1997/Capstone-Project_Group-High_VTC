using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
     [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float runSpeed = 5f;
    public float rotationSpeed = 30f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    private float turnSmoothVelocity;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform _playerTransform;
    private PlayerController PlayerController;
    public PlayerMovement(PlayerController playerController)
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
        _playerTransform = playerController.transform;
        controller = playerController.GetCharacterController();
        PlayerController = playerController;
    }

    /*public void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetButtonDown("Jump");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            _playerTransform.rotation = Quaternion.RotateTowards(_playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (jump && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * PlayerConst.Gravity);

        velocity.y += PlayerConst.Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }*/

    public void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetButtonDown("Jump");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Set animator blend parameters
        PlayerController.GetPlayerAnimation().SetAnimationType("MoveX", horizontal);
        PlayerController.GetPlayerAnimation().SetAnimationType("MoveY", vertical);

        // Set isMove based on whether there's input
        bool isMoving = direction.magnitude >= 0.1f;
        PlayerController.GetPlayerAnimation().SetAnimationType("isMove", isMoving);

        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            // _playerTransform.rotation = Quaternion.RotateTowards(_playerTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float speed = isRunning ? runSpeed : walkSpeed;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        if (jump && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * PlayerConst.Gravity);

        velocity.y += PlayerConst.Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

}

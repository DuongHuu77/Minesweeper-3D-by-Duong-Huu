using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    public float MoveSpeed = 5f;
    public float SprintSpeed = 8f;
    public float Gravity = -9.81f;
    public float JumpHeight = 1.5f;

    [Header("Cài đặt Camera")]
    public Camera PlayerCamera;
    public float LookSensitivity = 2f;
    public float TopLookLimit = 80f;
    public float BottomLookLimit = -80f;

    private CharacterController _characterController;
    private Vector3 _velocity;
    private float _verticalRotation;

    void Start()
    {
        _characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleMovement()
    {
        bool isGrounded = _characterController.isGrounded;

        if (isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        float speed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
        }

        _velocity.y += Gravity * Time.deltaTime;

        Vector3 finalMove = move * speed + _velocity;
        _characterController.Move(finalMove * Time.deltaTime);
    }

    private void HandleRotation()
    {
        // 1. Xoay ngang: Xoay toàn bộ thân nhân vật trái/phải theo chuột
        float mouseX = Input.GetAxis("Mouse X") * LookSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        // 2. Xoay dọc: Chỉ xoay Camera lên/xuống (Thân nhân vật không xoay theo)
        float mouseY = Input.GetAxis("Mouse Y") * LookSensitivity;
        _verticalRotation -= mouseY;
        // Giới hạn góc nhìn để không bị lộn ngược camera
        _verticalRotation = Mathf.Clamp(_verticalRotation, TopLookLimit, BottomLookLimit);
        if (PlayerCamera != null)
        {
            PlayerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }

    }
}
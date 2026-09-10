using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 0.08f;
    [SerializeField] private float keyboardTurnSpeed = 90f;
    [SerializeField] private float maxPitch = 80f;

    private CharacterController controller;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction turnAction;

    private float verticalVelocity;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        PlayerInput input = GetComponent<PlayerInput>();

        moveAction = input.actions.FindAction("Move", true);
        lookAction = input.actions.FindAction("Look", true);
        turnAction = input.actions.FindAction("Turn", true);
    }

    private void Update()
    {
        HandleCursor();

        if (GameModeController.IsUIOpen)
            return;

        Move();
        Look();
    }

    private void Move()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 direction =
            transform.right * input.x +
            transform.forward * input.y;

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        if (controller.isGrounded &&
            verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = direction * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Look()
    {
        Vector2 mouse = lookAction.ReadValue<Vector2>();
        float keyboard = turnAction.ReadValue<float>();

        float yaw =
            mouse.x * mouseSensitivity +
            keyboard *
            keyboardTurnSpeed *
            Time.deltaTime;

        transform.Rotate(0f, yaw, 0f);

        pitch -= mouse.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        cameraTransform.localRotation =
            Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleCursor()
    {
        if (GameModeController.IsUIOpen)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame &&
            Cursor.lockState != CursorLockMode.Locked)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 5.0f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private float _jumpForce = 0f; // si no usas salto, déjalo en 0

    [Header("Look Settings")]
    [SerializeField] private float _mouseSensitivity = 2.0f;
    [SerializeField] private float _verticalLookLimit = 80.0f;

    [Header("Component References")]
    [SerializeField] private Transform _cameraTransform;

    private CharacterController _characterController;
    private PlayerInputActions _inputActions;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _xRotation = 0f;

    private Vector3 _velocity; // para gravedad

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        if (_cameraTransform == null)
        {
            Debug.LogError("Error: Asigna la cámara al FirstPersonController.", this);
            enabled = false;
            return;
        }

        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();

        // Move
        _inputActions.Player.Move.performed += OnMoveInput;
        _inputActions.Player.Move.canceled += OnMoveInput;

        // Look  (corrección: antes estabas usando Move)
        _inputActions.Player.Look.performed += OnLookInput;
        _inputActions.Player.Look.canceled += OnLookInput;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMoveInput;
        _inputActions.Player.Move.canceled -= OnMoveInput;

        _inputActions.Player.Look.performed -= OnLookInput;
        _inputActions.Player.Look.canceled -= OnLookInput;

        _inputActions.Player.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnLookInput(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        // Dirección en plano XZ
        Vector3 move = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        if (move.sqrMagnitude > 1f) move.Normalize(); // evita boost diagonal

        // aplica velocidad horizontal
        Vector3 horizontal = move * _movementSpeed;

        // gravedad simple
        if (_characterController.isGrounded)
        {
            if (_velocity.y < 0) _velocity.y = -2f; // pegado al suelo
            // si quisieras salto:
            // if (_inputActions.Player.Jump.WasPerformedThisFrame()) _velocity.y = Mathf.Sqrt(_jumpForce * -2f * _gravity);
        }
        _velocity.y += _gravity * Time.deltaTime;

        // mover
        _characterController.Move((horizontal + _velocity) * Time.deltaTime);
    }

    private void HandleLook()
    {
        // NOTA: para Input System (delta), evita multiplicar por Time.deltaTime
        float mouseX = _lookInput.x * _mouseSensitivity;
        float mouseY = _lookInput.y * _mouseSensitivity;

        // Yaw (girar cuerpo)
        transform.Rotate(Vector3.up * mouseX);

        // Pitch (girar cámara)
        _xRotation -= mouseY; // invierte si prefieres estilo distinto (usar +=)
        _xRotation = Mathf.Clamp(_xRotation, -_verticalLookLimit, _verticalLookLimit);
        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private Camera _camera; // mejor que Camera.main
    [SerializeField] private LayerMask _interactionMask = ~0; // por defecto, todo

    private PlayerInputActions _inputActions;

    // Para evitar spam de logs/UI
    private IInteractable _lastSeen;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();

        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("Asigna una cámara al PlayerInteractor.", this);
                enabled = false;
            }
        }
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _inputActions.Player.Interact.performed -= OnInteract;
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        DetectAndShowFeedback();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
            }
        }
    }

    private void DetectAndShowFeedback()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        IInteractable seen = null;

        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionMask, QueryTriggerInteraction.Ignore))
        {
            hit.collider.TryGetComponent<IInteractable>(out seen);
        }

        // Cambió el objetivo enfocado
        if (!ReferenceEquals(seen, _lastSeen))
        {
            if (seen != null)
            {
                Debug.Log("Interactuable a la vista: " + ((Component)seen).name);
                // TODO: mostrar UI "E — Interactuar"
            }
            else
            {
                // TODO: ocultar UI
            }
            _lastSeen = seen;
        }
    }
}

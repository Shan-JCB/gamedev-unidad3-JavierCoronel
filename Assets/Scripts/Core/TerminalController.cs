using UnityEngine;

public class TerminalController : MonoBehaviour, IInteractable
{
    [Header("Light Reference")]
    public Light terminalLight; // Asignar en el Inspector

    [Header("State & Colors")]
    [SerializeField] private bool _isActive = false;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;

    private void Reset()
    {
        if (terminalLight == null)
            terminalLight = GetComponent<Light>();
    }

    private void Start()
    {
        if (terminalLight != null)
            terminalLight.color = _isActive ? activeColor : inactiveColor;
    }

    public void Interact()
    {
        if (terminalLight == null)
        {
            Debug.LogWarning("[Terminal] Asigna una 'Point Light' en el Inspector.");
            return;
        }

        // Cambiar estado visual
        _isActive = !_isActive;
        terminalLight.color = _isActive ? activeColor : inactiveColor;

        Debug.Log($"[Terminal] Estado del sistema: {(_isActive ? "Activo" : "Inactivo")}");

        // === INTEGRACION CON GAMEEVENTS ===
        if (_isActive)
        {
            Debug.Log("[Terminal] Activado ? Disparando evento OnObjectiveActivated()");
            GameEvents.TriggerObjectiveActivated();
        }

        // Opcional: evita doble activación
        // GetComponent<Collider>().enabled = false;
        // enabled = false;
    }
}

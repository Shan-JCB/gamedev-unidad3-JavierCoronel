using UnityEngine;

public class TerminalController : MonoBehaviour, IInteractable
{
    [Header("Light Reference")]
    public Light terminalLight; // arrastrar en el Inspector

    [Header("State & Colors")]
    [SerializeField] private bool _isActive = false;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;

    private void Reset()
    {
        // Si el objeto ya tiene una Light (p.e., si pusiste el script en la Point Light),
        // intenta autoasignarla al crear el componente.
        if (terminalLight == null) terminalLight = GetComponent<Light>();
    }

    private void Start()
    {
        // Asegura color inicial coherente con el estado
        if (terminalLight != null)
            terminalLight.color = _isActive ? activeColor : inactiveColor;
    }

    public void Interact()
    {
        if (terminalLight == null)
        {
            Debug.LogWarning("TerminalController: Asigna una 'Point Light' en el Inspector.");
            return;
        }

        _isActive = !_isActive;
        terminalLight.color = _isActive ? activeColor : inactiveColor;

        Debug.Log($"Estado del sistema: {(_isActive ? "Activo" : "Inactivo")}");
    }
}
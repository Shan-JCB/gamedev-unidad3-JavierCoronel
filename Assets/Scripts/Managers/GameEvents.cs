using System;

/// <summary>
/// Contenedor estático para eventos globales del juego (Observer).
/// NO referencia a GameObjects. Puro C#.
/// </summary>
public static class GameEvents
{
    /// <summary>
    /// Se dispara cuando un objetivo/terminal es activado.
    /// </summary>
    public static event Action OnObjectiveActivated;

    /// <summary>
    /// Invoca el evento de forma segura.
    /// </summary>
    public static void TriggerObjectiveActivated()
    {
        OnObjectiveActivated?.Invoke();
    }
}

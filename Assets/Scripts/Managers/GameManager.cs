using System;
using System.Collections;          // Necesario para las corrutinas
using UnityEngine;

/// <summary>
/// Orquesta el estado principal del juego (Playing, Victory, Defeat).
/// Patrón Singleton para acceso global único. Úsalo con moderación.
/// </summary>
public enum GameState
{
    Playing,
    Victory,
    Defeat
}

public sealed class GameManager : MonoBehaviour
{
    // --- Singleton ---
    public static GameManager Instance { get; private set; }

    // Estado actual (solo lectura externa)
    public GameState CurrentState { get; private set; } = GameState.Playing;

    // (Opcional, útil para UI u otros sistemas)
    public static event Action<GameState> OnGameStateChanged;

    // --- Gameplay ---
    [Header("Gameplay Settings")]
    [SerializeField] private int _objectivesToWin = 3;
    private int _objectivesCompleted = 0;

    // --- Timer ---
    [Header("Timer Settings")]
    [SerializeField] private float _timeLimit = 60f;
    private bool _isTimerRunning = false;

    private void Awake()
    {
        // Asegura una sola instancia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Descomentar si debe persistir entre escenas
    }

    private void OnEnable()
    {
        GameEvents.OnObjectiveActivated += HandleObjectiveActivated;
    }

    private void OnDisable()
    {
        GameEvents.OnObjectiveActivated -= HandleObjectiveActivated;
    }

    private void Start()
    {
        // Estado inicial del juego
        ChangeState(GameState.Playing);
        StartCoroutine(CountdownTimer()); // Inicia el contador regresivo
    }

    private void HandleObjectiveActivated()
    {
        if (CurrentState != GameState.Playing) return;

        _objectivesCompleted++;
        Debug.Log($"[GameManager] Objetivo completado. Progreso: {_objectivesCompleted}/{_objectivesToWin}");

        if (_objectivesCompleted >= _objectivesToWin)
        {
            ChangeState(GameState.Victory);
        }
    }

    /// <summary>
    /// Cambia el estado del juego de manera centralizada.
    /// </summary>
    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;

        ExitState(CurrentState);
        CurrentState = newState;
        Debug.Log($"[GameManager] New state: {CurrentState}");
        OnGameStateChanged?.Invoke(CurrentState);
        EnterState(CurrentState);
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                _isTimerRunning = true;
                break;

            case GameState.Victory:
                StartCoroutine(VictorySequence());
                break;

            case GameState.Defeat:
                StartCoroutine(DefeatSequence());
                break;
        }
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                _isTimerRunning = false; // detener el temporizador si cambia de estado
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
        }
    }

    // ======================================================
    // ================ CORRUTINAS PRINCIPALES ==============
    // ======================================================

    /// <summary>
    /// Corrutina que gestiona la secuencia de eventos cuando el jugador gana.
    /// </summary>
    private IEnumerator VictorySequence()
    {
        _isTimerRunning = false; // detener temporizador
        Debug.Log("[GameManager] SECUENCIA DE VICTORIA INICIADA");

        // 1?? Desactivar control del jugador
        var playerController = FindFirstObjectByType<FirstPersonController>();
        if (playerController != null)
            playerController.enabled = false;

        // 2?? Esperar un segundo
        yield return new WaitForSeconds(1f);

        // 3?? Mostrar panel de victoria
        Debug.Log("[GameManager] Mostrando UI de Victoria...");
        if (UIManager.Instance != null)
            UIManager.Instance.ShowVictoryPanel();

        // 4?? Esperar unos segundos antes de volver al menú
        yield return new WaitForSeconds(3f);

        // 5?? Regresar al menú principal
        Debug.Log("[GameManager] Volviendo al Menú Principal...");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.victoryPanel.SetActive(false);
            UIManager.Instance.ChangeState(UIManager.Instance.MainMenuState);
        }
    }

    /// <summary>
    /// Corrutina que maneja la secuencia de derrota.
    /// </summary>
    private IEnumerator DefeatSequence()
    {
        Debug.Log("[GameManager] SECUENCIA DE DERROTA INICIADA");

        // 1?? Desactivar control del jugador
        var playerController = FindFirstObjectByType<FirstPersonController>();
        if (playerController != null)
            playerController.enabled = false;

        // 2?? Mostrar panel de derrota si existe
        yield return new WaitForSeconds(0.5f);
        if (UIManager.Instance != null)
            UIManager.Instance.ShowDefeatPanel();

        // 3?? Esperar antes de volver al menú
        yield return new WaitForSeconds(3f);

        // 4?? Regresar al menú
        Debug.Log("[GameManager] Regresando al Menú Principal (Derrota)");
        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.victoryPanel.activeSelf)
                UIManager.Instance.victoryPanel.SetActive(false);

            UIManager.Instance.ChangeState(UIManager.Instance.MainMenuState);
        }
    }

    /// <summary>
    /// Corrutina del temporizador de cuenta atrás.
    /// </summary>
    private IEnumerator CountdownTimer()
    {
        _isTimerRunning = true;
        float remainingTime = _timeLimit;

        while (_isTimerRunning && remainingTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            remainingTime--;

            Debug.Log($"[GameManager] Tiempo restante: {remainingTime}s");

            // (Opcional) Actualiza la UI si tienes un texto de tiempo
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTimer(remainingTime);
        }

        // Si el tiempo llega a 0 y el jugador no ganó
        if (remainingTime <= 0f && CurrentState == GameState.Playing)
        {
            Debug.Log("[GameManager] Tiempo agotado ? Derrota");
            ChangeState(GameState.Defeat);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (name != "GameManager") name = "GameManager";
    }
#endif
}

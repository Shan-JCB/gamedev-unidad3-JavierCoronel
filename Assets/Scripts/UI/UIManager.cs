using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Para la carga de escenas

/// <summary>
/// Gestiona los estados de la UI y las transiciones entre ellos
/// Utiliza el Patrón de Diseño State, arquitectura limpia y escalable
/// Implementa un Singleton para un acceso global sencillo
/// </summary>
public class UIManager : MonoBehaviour
{
    // Singleton Pattern
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject inGameHudPanel;
    public GameObject optionsPanel;

    // Estados de la UI
    private UIState _currentState;
    public MainMenuState MainMenuState { get; private set; }
    public InGameState InGameState { get; private set; }
    public PauseMenuState PauseMenuState { get; private set; }
    public OptionsState OptionsState { get; private set; }


    public bool IsPlayerInputEnabled { get; private set; } = false;

    public void SetPlayerInput(bool enabled)
    {
        IsPlayerInputEnabled = enabled;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Evitar pantallas residuales
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (inGameHudPanel) inGameHudPanel.SetActive(false);

        // Inicializa estados
        MainMenuState = new MainMenuState(this);
        InGameState = new InGameState(this);
        PauseMenuState = new PauseMenuState(this);
    }

    private void Start()
    {
        // El estado inicial al arrancar el juego
        ChangeState(MainMenuState);
    }

    private void Update()
    {
        // Lógica para pausar el juego
        Keyboard keyboard = Keyboard.current;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (_currentState == InGameState)
            {
                ChangeState(PauseMenuState);
            }
            else if (_currentState == PauseMenuState)
            {
                ChangeState(InGameState);
            }
        }
    }

    public void ChangeState(UIState newState)
    {
        // Salir del estado actual si existe
        _currentState?.Exit();

        // Entrar en el nuevo estado
        _currentState = newState;
        _currentState.Enter();
    }

    // Métodos para los botones de la UI
    public void OnPlayButtonClicked()
    {
        ChangeState(InGameState);
    }

    public void OnResumeButtonClicked()
    {
        ChangeState(InGameState);
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Saliendo del juego...");
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnOptionsButtonClicked()
    {
        // Detecta de qué estado viene (MainMenu o Pause)
        UIState previous = _currentState;
        OptionsState = new OptionsState(this, previous);
        ChangeState(OptionsState);
    }

    public void OnReturnFromOptionsClicked()
    {
        if (OptionsState != null)
            OptionsState.ReturnToPreviousState();
    }

}
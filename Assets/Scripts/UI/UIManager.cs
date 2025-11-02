using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Para la carga de escenas
using System.Threading.Tasks;       // Para async/await
using UnityEngine.UI;               // Para el Slider
using TMPro;                        // Para el texto del temporizador (si usas TextMeshPro)

/// <summary>
/// Gestiona los estados de la UI y las transiciones entre ellos.
/// Utiliza el Patrón de Diseño State, arquitectura limpia y escalable.
/// Implementa un Singleton para un acceso global sencillo.
/// </summary>
public class UIManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject inGameHudPanel;
    public GameObject optionsPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;             // ?? Nuevo panel de derrota
    public GameObject loadingScreenPanel;
    public Slider loadingBar;

    [Header("HUD Elements")]
    public TMP_Text timerText;                 // ?? Texto opcional para mostrar tiempo

    // --- Estados de la UI ---
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

        // Ocultar pantallas residuales
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (inGameHudPanel) inGameHudPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        if (loadingScreenPanel) loadingScreenPanel.SetActive(false);

        // Inicializar estados
        MainMenuState = new MainMenuState(this);
        InGameState = new InGameState(this);
        PauseMenuState = new PauseMenuState(this);
    }

    private void Start()
    {
        // Estado inicial al arrancar el juego
        ChangeState(MainMenuState);
    }

    private void Update()
    {
        // Lógica para pausar/reanudar
        Keyboard keyboard = Keyboard.current;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            if (_currentState == InGameState)
                ChangeState(PauseMenuState);
            else if (_currentState == PauseMenuState)
                ChangeState(InGameState);
        }
    }

    // =====================
    // == MÉTODOS PRINCIPALES ==
    // =====================

    public void ChangeState(UIState newState)
    {
        // Limpia paneles activos
        HideAllPanels();

        // Salir del estado actual si existe
        _currentState?.Exit();

        // Entrar en el nuevo estado
        _currentState = newState;
        _currentState.Enter();
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (inGameHudPanel) inGameHudPanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        if (loadingScreenPanel) loadingScreenPanel.SetActive(false);
    }

    // =====================
    // == BOTONES DE LA UI ==
    // =====================

    // Botón "Jugar"
    public async void OnPlayButtonClicked()
    {
        // Mostrar pantalla de carga
        loadingScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync("Level_001");
        sceneLoadOperation.allowSceneActivation = false;

        // Progreso asíncrono
        while (!sceneLoadOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoadOperation.progress / 0.9f);
            loadingBar.value = progress;

            if (sceneLoadOperation.progress >= 0.9f)
                sceneLoadOperation.allowSceneActivation = true;

            await Task.Yield();
        }

        loadingScreenPanel.SetActive(false);
        ChangeState(InGameState);
    }

    // Botón "Reanudar"
    public void OnResumeButtonClicked()
    {
        ChangeState(InGameState);
    }

    // Botón "Salir"
    public void OnExitButtonClicked()
    {
        Debug.Log("[UIManager] Saliendo del juego...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Botón "Opciones"
    public void OnOptionsButtonClicked()
    {
        UIState previous = _currentState;
        OptionsState = new OptionsState(this, previous);
        ChangeState(OptionsState);
    }

    // Botón "Volver desde Opciones"
    public void OnReturnFromOptionsClicked()
    {
        if (OptionsState != null)
            OptionsState.ReturnToPreviousState();
    }

    // =====================
    // == MÉTODOS DE PANELES ==
    // =====================

    public void ShowVictoryPanel()
    {
        if (inGameHudPanel != null)
            inGameHudPanel.SetActive(false);
        if (victoryPanel != null)
            victoryPanel.SetActive(true);
    }

    public void ShowDefeatPanel()
    {
        if (inGameHudPanel != null)
            inGameHudPanel.SetActive(false);
        if (defeatPanel != null)
            defeatPanel.SetActive(true);
    }

    // =====================
    // == HUD: TEMPORIZADOR ==
    // =====================

    public void UpdateTimer(float seconds)
    {
        if (timerText == null) return;

        int intSeconds = Mathf.CeilToInt(seconds);
        timerText.text = $"Tiempo: {intSeconds}s";

        // Color de advertencia
        if (seconds <= 10)
            timerText.color = Color.red;
        else
            timerText.color = Color.white;
    }
}

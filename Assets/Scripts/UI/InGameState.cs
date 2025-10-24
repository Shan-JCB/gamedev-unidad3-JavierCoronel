using UnityEngine;

public class InGameState : UIState
{
    public InGameState(UIManager uiManager) : base(uiManager) { }

    public override void Enter()
    {
        Debug.Log("Entrando al estado de En Juego");
        m_uiManager.inGameHudPanel.SetActive(true);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_uiManager.SetPlayerInput(true); // Habilitar control del jugador
    }

    public override void Exit()
    {
        Debug.Log("Saliendo del estado de En Juego");
        m_uiManager.inGameHudPanel.SetActive(false);
    }
}
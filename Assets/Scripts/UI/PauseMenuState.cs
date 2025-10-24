using UnityEngine;

public class PauseMenuState : UIState
{
    public PauseMenuState(UIManager uiManager) : base(uiManager) { }

    public override void Enter()
    {
        Debug.Log("Entrando al estado de En Pausa");
        m_uiManager.pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_uiManager.SetPlayerInput(false); // Bloquear control del jugador
    }


    public override void Exit()
    {
        Debug.Log("Saliendo del estado de En Pausa");
        m_uiManager.pauseMenuPanel.SetActive(false);
    }
}
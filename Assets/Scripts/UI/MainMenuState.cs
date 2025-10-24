using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : UIState
{
    public MainMenuState(UIManager uiManager) : base(uiManager) { }

    // MainMenuState
    public override void Enter()
    {
        Debug.Log("Entrando al estado de Menú Principal");
        m_uiManager.mainMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_uiManager.SetPlayerInput(false); // Bloquear control del jugador
    }


    public override void Exit()
    {
        Debug.Log("Saliendo del estado de Menú Principal");
        m_uiManager.mainMenuPanel.SetActive(false);
    }
}

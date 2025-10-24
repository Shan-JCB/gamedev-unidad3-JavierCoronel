using UnityEngine;

public class OptionsState : UIState
{
    private UIState _previousState; // Guarda de dónde venimos

    public OptionsState(UIManager uiManager, UIState previousState) : base(uiManager)
    {
        _previousState = previousState;
    }

    public override void Enter()
    {
        Debug.Log("Entrando al estado de Opciones");
        m_uiManager.optionsPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_uiManager.SetPlayerInput(false);
    }

    public override void Exit()
    {
        Debug.Log("Saliendo del estado de Opciones");
        m_uiManager.optionsPanel.SetActive(false);
    }

    // Llamado por el botón "Volver"
    public void ReturnToPreviousState()
    {
        if (_previousState != null)
            m_uiManager.ChangeState(_previousState);
    }
}

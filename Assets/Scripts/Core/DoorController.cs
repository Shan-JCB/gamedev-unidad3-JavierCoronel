using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour, IInteractable
{
    private bool _isOpen = false;

    public void Interact()
    {
        _isOpen = !_isOpen;
        Debug.Log(_isOpen ? "La puerta se ha ABIERTO." : "La puerta se ha CERRADO.");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootChestController : MonoBehaviour, IInteractable
{
    private bool _isOpened = false;

    public void Interact()
    {
        if (_isOpened)
        {
            Debug.Log("Este cofre ya se ha abierto.");
            return;
        }

        _isOpened = true;
        Debug.Log("¡Has abierto el cofre y encotrado un tesoro Muy Bien PlayerFacio!");
    }
}
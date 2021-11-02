using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LogInputs : MonoBehaviour
{
    private void Awake()
    {
        Debug.LogError("======================");
    }

    public void DisplayName(InputAction.CallbackContext context)
    {
        Debug.LogError(context.action.name);
    }
}

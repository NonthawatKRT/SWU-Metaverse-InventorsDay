using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleConsole : MonoBehaviour
{
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleConsoleVisibility();
        }
    }

    public void ToggleConsoleVisibility()
    {
        
    }
}

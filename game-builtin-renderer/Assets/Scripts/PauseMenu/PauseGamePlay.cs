using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PauseGamePlay : MonoBehaviour
{
    bool isPaused;

    [SerializeField]
    GameObject menu;

    private void Start() 
    {
        isPaused = false;
        //menu.SetActive(false);
    }

    public void TogglePause(InputAction.CallbackContext context)
    {        
        if(!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            menu.SetActive(true);
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1;
            menu.SetActive(false);
        }
    }
}

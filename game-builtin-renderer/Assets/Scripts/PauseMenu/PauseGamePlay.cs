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


    [SerializeField] private UnityEvent OnPause;
    [SerializeField] private UnityEvent OnResume; 

    private void Start() 
    {
        isPaused = false;
        //menu.SetActive(false);
    }

    public void TogglePause(InputAction.CallbackContext context)
    {
        if (!context.performed) return; 
        
        if(!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            menu.SetActive(true);
            OnPause?.Invoke();
        }
        else
        {
            isPaused = false;
            Time.timeScale = 1;
            menu.SetActive(false);
            OnResume?.Invoke();
        }
    }
}

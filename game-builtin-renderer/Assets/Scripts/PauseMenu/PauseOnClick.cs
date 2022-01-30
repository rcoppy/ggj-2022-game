using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PauseOnClick : MonoBehaviour
{
    bool isPaused;

    [SerializeField]
    GameObject menu;

    private void Start() 
    {
        isPaused = true;
    }

    public void ExitPause()
    {        
        isPaused = false;
        Time.timeScale = 1;
        menu.SetActive(false);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeSceneMainMenu : MonoBehaviour
{


    // Update is called once per frame
    public void ChangeScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene ("SplashScreen", LoadSceneMode.Single); 
    }
}

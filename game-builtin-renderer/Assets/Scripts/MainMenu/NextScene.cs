using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public void Skip()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }
}
